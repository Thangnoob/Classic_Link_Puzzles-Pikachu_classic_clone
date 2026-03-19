using System;
using UnityEngine;

public class GameTimerManager : MonoBehaviour
{
    public static GameTimerManager Instance { get; private set; }

    public event EventHandler OnTimeOver;

    private float timeRemaining;
    private float levelDuration = 180f;

    private float timeBonus;

    private float comboCount = 0;
    private float timeBonusCombo;

    private float matchTimeCheck = 0f;

    public float LevelDuration => levelDuration;     
    public float TimeRemaining => timeRemaining;
    public float TimeNormalized => levelDuration > 0f ? Mathf.Clamp01(timeRemaining / levelDuration) : 0f;

    private void Update()
    {
        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                OnTimeOver?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void Start()
    {
        Instance = this;
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnMatchSuccess += GameManager_OnMatchSuccess;
    }

    private void GameManager_OnMatchSuccess(object sender, EventArgs e)
    {
        if (Time.time - matchTimeCheck < 3f)
        {
            if (comboCount == 5)
            {
                timeRemaining += timeBonusCombo;
            }
            timeRemaining += timeBonus;
            comboCount++;
        }
        else
        {
            timeRemaining += timeBonus;
            comboCount = 0;
        }   

        matchTimeCheck = Time.time;
        
    }

    private void GameManager_OnGameStart(object sender, System.EventArgs e)
    {
        timeRemaining = levelDuration;
    }

    public void SetDuration(float newLevelDuration)
    {
        this.levelDuration = newLevelDuration;
    }

    public void SetTimeBonus(float newLevelDuration)
    {
        this.timeBonus = newLevelDuration;
    }

    public void SetTimeBonusCombo(float newLevelDuration)
    {
        this.timeBonusCombo = newLevelDuration;
    }
}
