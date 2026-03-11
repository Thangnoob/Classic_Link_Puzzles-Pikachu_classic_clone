using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const int SOUND_VOLUME_MAX = 10;

    public static SoundManager Instance { get; private set; }

    private static int soundVolume = 6;

    public event EventHandler OnSoundVolumeChanged;

    [SerializeField] private AudioClip selectAudioClip;
    [SerializeField] private AudioClip connectSuccessAudioClip;
    [SerializeField] private AudioClip connectFailAudioClip;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.Instance.OnTileSelected += GameManager_OnTileSelected;
        GameManager.Instance.OnMatchSuccess += GameManager_OnMatchSuccess;
        GameManager.Instance.OnMatchFailure += GameManager_OnMatchFailure;
    }

    private void GameManager_OnTileSelected(object sender, System.EventArgs e)
    {
        AudioSource.PlayClipAtPoint(selectAudioClip, Camera.main.transform.position, GetSoundVolume());
    }

    private void GameManager_OnMatchSuccess(object sender, System.EventArgs e)
    {
        AudioSource.PlayClipAtPoint(connectSuccessAudioClip, Camera.main.transform.position, GetSoundVolume());
    }

    private void GameManager_OnMatchFailure(object sender, System.EventArgs e)
    {
        AudioSource.PlayClipAtPoint(connectFailAudioClip, Camera.main.transform.position, GetSoundVolume());
    }

    public void ChangeSoundVolume()
    {
        soundVolume = (soundVolume + 1) % SOUND_VOLUME_MAX;
        OnSoundVolumeChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetSoundVolume()
    {
        return soundVolume;
    }

    public float GetSoundVolumeNormalized()
    {
        return ((float)soundVolume) / SOUND_VOLUME_MAX;
    }
}
