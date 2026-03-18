using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private const string TotalScoreKey = "TotalScoreKey";
    private const string HighScoreKey = "HighScoreKey";

    [Header("Score Weights")]
    [Tooltip("Điểm mỗi cặp match (100 → 7200 khi hoàn thành 72 cặp)")]
    [SerializeField] private int matchPointsPerPair = 100;

    [Tooltip("Điểm mỗi lần shuffle đã dùng")]
    [SerializeField] private int shufflePointsPerUse = 500;

    [Tooltip("Điểm mỗi giây còn lại khi hoàn màn")]
    [SerializeField] private int timePointsPerSecond = 20;

    [Header("Runtime (read-only)")]
    [SerializeField] private int matchCount;
    [SerializeField] private int shuffleUsedCount;
    [SerializeField] private int levelStartingShuffle;
    [SerializeField] private int totalScoreSaved;

    private int lastMatchScore;
    private int lastShuffleScore;
    private int lastTimeScore;
    private int lastTotalScore;

    public int MatchCount => matchCount;
    public int ShuffleUsedCount => shuffleUsedCount;
    public int LastMatchScore => lastMatchScore;
    public int LastShuffleScore => lastShuffleScore;
    public int LastTimeScore => lastTimeScore;
    public int LastTotalScore => lastTotalScore;

    public event EventHandler OnScoreUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            totalScoreSaved = PlayerPrefs.GetInt(TotalScoreKey, 0);
        } else
        {
            Destroy(gameObject);
        }
    }
    // =========================
    // LEVEL LIFECYCLE
    // =========================
    public void StartLevel(int startingShuffle)
    {
        matchCount = 0;
        shuffleUsedCount = 0;
        levelStartingShuffle = Mathf.Max(0, startingShuffle);

        lastMatchScore = 0;
        lastShuffleScore = 0;
        lastTimeScore = 0;
        lastTotalScore = 0;
    }
    // =========================
    // RUNTIME TRACKING
    // =========================
    public void AddMatch()
    {
        matchCount++;
    }

    public void AddShuffleUsed()
    {
        shuffleUsedCount++;
    }

    // =========================
    // SCORE CALCULATION
    // =========================
    public void CompleteLevel(float timeRemainingSeconds)
    {
        lastMatchScore = matchCount * matchPointsPerPair;
        int shuffleRemaining = Mathf.Max(0, levelStartingShuffle - shuffleUsedCount);
        lastShuffleScore = shuffleRemaining * shufflePointsPerUse;
        lastTimeScore = Mathf.RoundToInt(timeRemainingSeconds * timePointsPerSecond);
        lastTotalScore = lastMatchScore + lastShuffleScore + lastTimeScore;

        totalScoreSaved += lastTotalScore;
        PlayerPrefs.SetInt(TotalScoreKey, totalScoreSaved);
        UpdateHighScore(totalScoreSaved);
        PlayerPrefs.Save();

        OnScoreUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void SaveTotalScoreWithoutCurrentLevel()
    {
        PlayerPrefs.GetInt(TotalScoreKey, 0);
        PlayerPrefs.Save();
    }

    // =========================
    // HIGH SCORE
    // =========================
    private void UpdateHighScore(int newScore)
    {
        float oldScore = PlayerPrefs.GetInt(HighScoreKey);
        Debug.Log("Score Manager: Old score" + oldScore);
        if (oldScore < newScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, newScore);
            PlayerPrefs.Save();
        }

        float scoreDebug = PlayerPrefs.GetInt(HighScoreKey, 0);
        Debug.Log("Score Manager: High score" + scoreDebug);
    }

    // =========================
    // GETTERS (PERSISTENT)
    // =========================
    public int GetTotalScore() => lastTotalScore;
    public int GetMatchScore() => lastMatchScore;
    public int GetShuffleScore() => lastShuffleScore;
    public int GetTimeScore() => lastTimeScore;

    // =========================
    // GETTERS (PERSISTENT)
    // =========================
    public int GetSavedTotalScore() => totalScoreSaved;

    public static int GetSavedTotalScoreStatic()
    {
        return PlayerPrefs.GetInt(TotalScoreKey, 0);
    }

    public static int GetHighScoreStatic()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    // =========================
    // RESET
    // =========================
    public static void ResetScoreProgress()
    {
        PlayerPrefs.SetInt(TotalScoreKey, 0);
        PlayerPrefs.Save();
        Debug.Log("ScoreManager: Điểm tiến trình đã reset!");
    }
}
