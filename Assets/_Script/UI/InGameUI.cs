using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI shuffleText;
    [SerializeField] private Image timerBarImage;
    [SerializeField] private Button shuffleButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI currentLevelText;

    private void Awake()
    {
        shuffleButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ManualShuffle();
            }
        });
        pauseButton.onClick.AddListener(() => {
            GameManager.Instance.PauseGame();
        });
    }

    private void Start()
    {
        LevelManager.Instance.OnLevelLoaded += Instance_OnLevelLoaded;
    }

    private void Instance_OnLevelLoaded(object sender, System.EventArgs e)
    {
        UpdateTotalScoreProgressLabel();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        UpdateShuffleUI();
        UpdateTimerBar();   
    }


    private void UpdateShuffleUI()
    {
        if (shuffleText != null)
        {
            shuffleText.text = $"{ShuffleManager.Instance.ShuffleRemaining}";
        }
    }

    private void UpdateTimerBar()
    {
        timerBarImage.fillAmount = GameTimerManager.Instance.TimeNormalized;
    }

    private void UpdateTotalScoreProgressLabel()
    {
        int currentLevel = LevelManager.Instance.CurrentLevelIndex + 1;
        currentLevelText.text = "Màn " + currentLevel.ToString() + "/7";
    }
}
