using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.MainMenuScene);
            ScoreManager.ResetScoreProgress();
            LevelManager.ResetLevelProgress();
        });
    }

    private void OnEnable()
    {
        float totalScore = ScoreManager.GetSavedTotalScoreStatic();
        totalScoreText.text = totalScore.ToString();
    }
}
