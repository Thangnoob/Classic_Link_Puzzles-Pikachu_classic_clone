using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private LevelDataSO[] levels;
    [SerializeField] private SpriteRenderer backgroundRenderer;

    private int currentLevelIndex;

    public LevelDataSO CurrentLevel => levels[currentLevelIndex];

    private void Awake()
    {
        Instance = this;
    }   

    public void LoadLevel(int index)
    {
        currentLevelIndex = index;
        LevelDataSO level = levels[index];

        GridManager.Instance.Initialize(level.cols, level.rows);
        GridManager.Instance.SetGravityMode(level.gravityMode);

        GameTimerManager.Instance.SetDuration(level.levelDuration);

        if (backgroundRenderer != null && level.backgroundSprite != null)
            backgroundRenderer.sprite = level.backgroundSprite;


    }
}