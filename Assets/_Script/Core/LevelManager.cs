using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public event EventHandler OnLevelLoaded;

    [Header("References")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    [Header("Levels")]
    [SerializeField] private LevelDataSO[] levels;
    [SerializeField] private int startLevelIndex = 0;

    private int currentLevelIndex;
    private int totalShuffleBonusEarned = 0;
    private const string CurrentLevelIndexKey = "CurrentLevelIndexKey";
    private const string TotalShuffleBonusEarnedKey = "TotalShuffleBonusEarnedKey";

    public int CurrentLevelIndex => currentLevelIndex;
    public LevelDataSO CurrentLevel => (levels != null && levels.Length > 0) ? levels[currentLevelIndex] : null;
    public int TotalShuffleBonusEarned => Mathf.Max(0, totalShuffleBonusEarned);

    private void Awake()
    {
        Instance = this;
        LoadProgress();
    }

    public int GetManualShuffleLimit(int baseManualShuffle, int maxCap)
    {
        int baseValue = Mathf.Max(0, baseManualShuffle);
        int cap = Mathf.Max(0, maxCap);
        return Mathf.Min(cap, baseValue + TotalShuffleBonusEarned);
    }

    public void AddShuffleBonusForLevel(int levelIndex)
    {
        if (levels == null || levels.Length == 0) return;
        if (levelIndex < 0 || levelIndex >= levels.Length) return;

        int bonus = Mathf.Max(0, levels[levelIndex].manualShuffleBonus);
        totalShuffleBonusEarned += bonus;

        SaveProgress();
    }


    public void LoadStartLevel()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("LevelManager: chưa cấu hình mảng levels!");
            return;
        }

        int requested = startLevelIndex;
        if (SceneLoader.IsContinue)
            requested = SceneLoader.RequestedLevelIndex;

        int clamped = Mathf.Clamp(requested, 0, levels.Length - 1);
        LoadLevel(clamped);
    }

    public void SaveCurrentLevelIndex(int index)
    {
        currentLevelIndex = Mathf.Max(0, index);
        SaveProgress();
    }

    public void LoadLevel(int index)
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogError("LevelManager: chưa cấu hình mảng levels!");
            return;
        }

        if (index < 0 || index >= levels.Length)
        {
            Debug.LogError($"LevelManager: index level {index} không hợp lệ!");
            return;
        }

        currentLevelIndex = index;
        Debug.Log("Level Manager current level: " + currentLevelIndex);
        LevelDataSO level = levels[currentLevelIndex];

        // Grid
        GridManager.Instance.Initialize(level.cols, level.rows);
        GridManager.Instance.SetGravityMode(level.gravityMode);

        // Timer
        GameTimerManager.Instance.SetDuration(level.levelDuration);
        GameTimerManager.Instance.SetTimeBonus(level.timeBonusPerMatch);
        GameTimerManager.Instance.SetTimeBonusCombo(level.timeBounusCombo); 

        // Background
        if (backgroundRenderer != null && level.backgroundSprite != null)
        {
            backgroundRenderer.sprite = level.backgroundSprite;
        }

        if (level.bgm != null) MusicManager.Instance.PlayBGM(level.bgm);

        OnLevelLoaded?.Invoke(this, EventArgs.Empty);
    }
    private void LoadProgress()
    {
        currentLevelIndex = PlayerPrefs.GetInt(CurrentLevelIndexKey, 0);
        totalShuffleBonusEarned = Mathf.Max(0, PlayerPrefs.GetInt(TotalShuffleBonusEarnedKey, 0));
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(CurrentLevelIndexKey, currentLevelIndex);
        PlayerPrefs.SetInt(TotalShuffleBonusEarnedKey, totalShuffleBonusEarned);
        PlayerPrefs.Save();
    }

    public static void ResetLevelProgress()
    {
        PlayerPrefs.SetInt(CurrentLevelIndexKey, 0);
        PlayerPrefs.Save();
        Debug.Log("LevelManager: Level đã được reset!");
    }
}