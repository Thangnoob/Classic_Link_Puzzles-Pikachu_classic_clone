using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Pikachu/Level Data SO")]
public class LevelDataSO : ScriptableObject
{
    [Header("Board Size")]
    public int cols = 16;
    public int rows = 9;

    [Header("Time & Shuffle")]
    public float levelDuration = 180f;
    public int manualShuffleBonus = 3;
    public float timeBonusPerMatch = 2f;
    public float timeBounusCombo = 4f;

    [Header("Visual")]
    public Sprite backgroundSprite;
    public AudioClip bgm;

    [Header("Tile / Movement")]
    public int tileTypeCount = 24;
    public GravityMode gravityMode = GravityMode.None;
}

