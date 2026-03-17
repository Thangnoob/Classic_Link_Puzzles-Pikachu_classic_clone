using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI highScoreText;

    public EventHandler OnContinueButtonClicked;
    private void Awake()
    {
        Time.timeScale = 1f; 
        playButton.onClick.AddListener(() =>
        {
            ResetData();
            SceneLoader.IsContinue = false;
            SceneLoader.RequestedLevelIndex = 0;
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        continueButton.onClick.AddListener(() => {
            int savedIndex = LevelManager.GetCurrentLevel();
            Debug.Log("Current level" + savedIndex);
            SceneLoader.IsContinue = true;
            SceneLoader.RequestedLevelIndex = savedIndex;
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
            OnContinueButtonClicked?.Invoke(this, EventArgs.Empty); 
        });
    }

    private void Start()
    {
        int currentLevel = LevelManager.GetCurrentLevel();
        if (currentLevel == 0)
        {
            VisibleContinueButton();
        }
        UpdateHighScoreText();
    }

    private void ResetData()
    {
        LevelManager.ResetLevelProgress();
        ScoreManager.ResetScoreProgress();
    }

    private void VisibleContinueButton()
    {
        continueButton.gameObject.SetActive(false);
    }

    private void UpdateHighScoreText()
    {
        float highScore = ScoreManager.GetHighScoreStatic();
        highScoreText.text = "Điểm cao nhất:\n" + highScore;
    }
}
