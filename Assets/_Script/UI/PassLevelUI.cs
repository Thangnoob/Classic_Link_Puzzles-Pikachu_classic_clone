using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class PassLevelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI matchScoreText;
    [SerializeField] private TextMeshProUGUI shuffleScoreText;
    [SerializeField] private TextMeshProUGUI timeScoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private Button nextLevelButton;

    private void Start()
    {
        ScoreManager.Instance.OnScoreUpdated += ScoreManager_OnScoreUpdated;
        nextLevelButton.onClick.AddListener(OnNextLevel);
        Hide();
    }

    private void ScoreManager_OnScoreUpdated(object sender, EventArgs e)
    {
        Time.timeScale = 0f;
        RefreshScoreDisplay();
        Show();
    }
 
    public void RefreshScoreDisplay()
    {
        if (ScoreManager.Instance == null) return;

        int match = ScoreManager.Instance.GetMatchScore();
        int shuffle = ScoreManager.Instance.GetShuffleScore();
        int time = ScoreManager.Instance.GetTimeScore();
        int total = ScoreManager.Instance.GetTotalScore();

        if (matchScoreText != null)
            matchScoreText.text = "Điểm nối được: " + match.ToString();
        if (shuffleScoreText != null)
            shuffleScoreText.text = "Điểm lượt đổi: " + shuffle.ToString();
        if (timeScoreText != null)
            timeScoreText.text = "Điểm thời gian: " + time.ToString();
        if (totalScoreText != null)
            totalScoreText.text = "TỔNG: " +total.ToString();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnNextLevel()
    {
        Time.timeScale = 1f;
        Hide();

        if (LevelManager.Instance == null)
            return;

        int current = LevelManager.Instance.CurrentLevelIndex;
        if (current >= 7)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameOverScene);
            return;
        }

        Debug.Log("Pass level UI current level: " + current);
        LevelManager.Instance.LoadLevel(current);
    }
}
