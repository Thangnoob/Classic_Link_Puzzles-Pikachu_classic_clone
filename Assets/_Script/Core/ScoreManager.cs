using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

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
        Instance = this;
    }

    /// <summary>Gọi khi bắt đầu màn (LevelManager load xong / GameManager StartLevel).</summary>
    public void StartLevel()
    {
        matchCount = 0;
        shuffleUsedCount = 0;
        lastMatchScore = lastShuffleScore = lastTimeScore = lastTotalScore = 0;
    }

    /// <summary>Gọi mỗi khi match thành công 1 cặp.</summary>
    public void AddMatch()
    {
        matchCount++;
    }

    /// <summary>Gọi mỗi khi người chơi dùng 1 lần shuffle thủ công.</summary>
    public void AddShuffleUsed()
    {
        shuffleUsedCount++;
    }

    /// <summary>Gọi khi hoàn màn (win). Tính và lưu điểm thành phần + tổng để PassLevelUI lấy.</summary>
    /// <param name="timeRemainingSeconds">Số giây còn lại khi hoàn màn (từ GameTimerManager.TimeRemaining)</param>
    public void CompleteLevel(float timeRemainingSeconds)
    {
        lastMatchScore = matchCount * matchPointsPerPair;
        lastShuffleScore = shuffleUsedCount * shufflePointsPerUse;
        lastTimeScore = Mathf.RoundToInt(timeRemainingSeconds * timePointsPerSecond);
        lastTotalScore = lastMatchScore + lastShuffleScore + lastTimeScore;

        OnScoreUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Điểm tổng cuối (sau khi gọi CompleteLevel).</summary>
    public int GetTotalScore() => lastTotalScore;

    /// <summary>Điểm từ match (số cặp * 100).</summary>
    public int GetMatchScore() => lastMatchScore;

    /// <summary>Điểm từ shuffle (số lần dùng * shufflePointsPerUse).</summary>
    public int GetShuffleScore() => lastShuffleScore;

    /// <summary>Điểm từ thời gian (giây còn lại * 20).</summary>
    public int GetTimeScore() => lastTimeScore;
}
