using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button continueButton;

    public EventHandler OnContinueButtonClicked;
    private const string CurrentLevelIndexKey = "CurrentLevelIndexKey";

    private void Awake()
    {
        Time.timeScale = 1f; 
        playButton.onClick.AddListener(() =>
        {
            // New game → start at level 0 and overwrite saved progress
            PlayerPrefs.SetInt(CurrentLevelIndexKey, 0);
            PlayerPrefs.Save();
            SceneLoader.IsContinue = false;
            SceneLoader.RequestedLevelIndex = 0;
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        continueButton.onClick.AddListener(() => {
            int savedIndex = PlayerPrefs.GetInt(CurrentLevelIndexKey, 0);
            Debug.Log("Current level" + savedIndex);
            SceneLoader.IsContinue = true;
            SceneLoader.RequestedLevelIndex = savedIndex;
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
            OnContinueButtonClicked?.Invoke(this, EventArgs.Empty); 
        });
    }

   
}
