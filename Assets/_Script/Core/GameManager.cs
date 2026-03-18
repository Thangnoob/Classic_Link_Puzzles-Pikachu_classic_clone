using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnTileSelected;
    public event EventHandler OnMatchSuccess;
    public event EventHandler OnMatchFailure;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;
    public event EventHandler OnGameStart;
    public event EventHandler OnLevelPassed;

    [Header("References")]
    [SerializeField] private MatchPathfinder matchPathfinder;
    [SerializeField] private PathRenderer pathRenderer;

    private Tile firstSelected;
    private bool isPlaying;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameTimerManager.Instance.OnTimeOver += GameTimerManager_OnTimeOver;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelLoaded += LevelManager_OnLevelLoaded;
            LevelManager.Instance.LoadStartLevel();
        }

        StartLevel();
    }

    private void Update()
    {
        if (!isPlaying) return;
    }

    // =========================
    // LEVEL FLOW
    // =========================
    private void StartLevel()
    {
        OnGameStart?.Invoke(this, EventArgs.Empty);
        isPlaying = true;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StartLevel(
            ShuffleManager.Instance.ShuffleRemaining
        );

        ShuffleManager.Instance.Initialize();
    }

    private void LevelManager_OnLevelLoaded(object sender, EventArgs e)
    {
        Time.timeScale = 1f;
        StartLevel();
    }

    // =========================
    // TIME FLOW
    // =========================

    private void GameTimerManager_OnTimeOver(object sender, System.EventArgs e)
    {
        isPlaying = false;
        Debug.Log("Hết thời gian!");
        // Tùy bạn xử lý: hiện popup thua, dừng input, v.v.

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SaveTotalScoreWithoutCurrentLevel();
    }

    // =========================
    // TILE INTERACTION
    // =========================
    public void OnTileClicked(Tile clicked)
    {
        if (!isPlaying) return;

        OnTileSelected?.Invoke(this, EventArgs.Empty);
        if (firstSelected == null)
        {
            firstSelected = clicked;
            firstSelected.SetSelected(true);
            return;
        }

        if (firstSelected == clicked)
        {
            firstSelected.SetSelected(false);
            firstSelected = null;
            return;
        }

        // tạm thời highlight tile thứ hai
        clicked.SetSelected(true);

        // Kiểm tra match
        if (matchPathfinder.TryGetPath(firstSelected, clicked, out var path))
        {
            Debug.Log("MATCH SUCCESS!");

            // Vẽ line đỏ theo path
            pathRenderer.DrawConnection(path);

            GridManager.Instance.RemoveTile(firstSelected);
            GridManager.Instance.RemoveTile(clicked);   

            GridManager.Instance.ApplyGravityForBoard();

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddMatch();

            OnMatchSuccess?.Invoke(this, EventArgs.Empty);

            // Delay 1 frame để gravity cập nhật grid xong rồi mới check pair
            StartCoroutine(CheckPairAfterGravity());
        }
        else
        {
            // Nếu không nối được thì bỏ chọn tile thứ hai
            clicked.SetSelected(false);
            OnMatchFailure?.Invoke(this, EventArgs.Empty);
        }

        if (firstSelected != null)
        {
            firstSelected.SetSelected(false);
        }

        firstSelected = null;
    }

    // =========================
    // GAME FLOW
    // =========================
    private IEnumerator CheckPairAfterGravity()
    {
        yield return null;
        if (!matchPathfinder.HasAnyValidPair())
        {
            Debug.Log("Auto Shuffle (no valid pair)");
            GridManager.Instance.ShuffleGrid(true); // ← ensure valid pair
        }

        if (GridManager.Instance.GetActiveTiles().Count == 0)
        {
            isPlaying = false;
            Debug.Log("Win!");

            OnLevelPassed?.Invoke(this, EventArgs.Empty);

            if (ScoreManager.Instance != null && GameTimerManager.Instance != null)
                ScoreManager.Instance.CompleteLevel(GameTimerManager.Instance.TimeRemaining);

            ShuffleManager.Instance.AddBonus(
                LevelManager.Instance.CurrentLevel.manualShuffleBonus
            );

            LevelManager.Instance?.CompleteLevel();
        }
    }

    // =========================
    // SHUFFLE
    // =========================
    public void ManualShuffle()
    {
        if (!isPlaying) return;

        if (!ShuffleManager.Instance.TryUseShuffle())
        {
            Debug.Log("Hết lượt shuffle!");
            return;
        }

        ScoreManager.Instance?.AddShuffleUsed();
        GridManager.Instance.ShuffleGrid(true);

        Debug.Log($"Shuffle còn lại: {ShuffleManager.Instance.ShuffleRemaining}");
    }

    // =========================
    // PAUSE SYSTEM
    // =========================
    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPlaying = false;
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        isPlaying = true;
        OnGameUnPaused?.Invoke(this, EventArgs.Empty);
    }

}