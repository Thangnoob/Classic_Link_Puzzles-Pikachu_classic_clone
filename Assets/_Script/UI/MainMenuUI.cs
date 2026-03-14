using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        Time.timeScale = 1f; 
        playButton.onClick.AddListener(() =>
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        continueButton.onClick.AddListener(() => {
            //Load scene main menu
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        });
    }
}
