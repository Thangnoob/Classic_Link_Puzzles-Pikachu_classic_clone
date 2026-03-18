using UnityEngine;

public class ShuffleManager : MonoBehaviour
{
    public static ShuffleManager Instance { get; private set; }

    private const string ShuffleRemainingKey = "ShuffleRemainingKey";

    [Header("Base Shuffle Number Settings")]
    [SerializeField] private int MAX_SHUFFLE = 7;
    [SerializeField] private int START_SHUFFLE = 3;

    private int shuffleRemaining;
    public int ShuffleRemaining => shuffleRemaining;

    private void Awake()
    {
        Instance = this;
        Load();
    }

    // =========================
    // INIT GAME
    // =========================
    public void InitializeIfFirstTime()
    {
        if (!PlayerPrefs.HasKey(ShuffleRemainingKey))
        {
            shuffleRemaining = START_SHUFFLE;
            Save();
        }
    }

    // =========================
    // USE SHUFFLE
    // =========================
    public bool TryUseShuffle()
    {
        if (shuffleRemaining <= 0)
            return false;

        shuffleRemaining--;
        Save();
        return true;
    }

    // =========================
    // PROGRESSION
    // =========================
    public void AddBonus(int bonus)
    {
        shuffleRemaining = Mathf.Min(
            MAX_SHUFFLE,
            shuffleRemaining + Mathf.Max(0, bonus)
        );

        Save();
    }

    // =========================
    // SAVE / LOAD
    // =========================
    private void Load()
    {
        shuffleRemaining = PlayerPrefs.GetInt(ShuffleRemainingKey, START_SHUFFLE);
    }

    private void Save()
    {
        PlayerPrefs.SetInt(ShuffleRemainingKey, shuffleRemaining);
        PlayerPrefs.Save();
    }
    // =========================
    // STATIC HELPER
    //==========================
    public static void ResetShuffleRemaining()
    {
        PlayerPrefs.SetInt(ShuffleRemainingKey, 0);
        PlayerPrefs.Save();
    }
}