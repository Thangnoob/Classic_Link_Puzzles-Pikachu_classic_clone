using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler
{
    private int type; //ID type of image (0-71 for 72 images)
    private SpriteRenderer spriteRenderer;
    private Vector2Int gridPos;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.cyan;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
    }

    public void SetType(int newType, Sprite[] tileSprites)
    {
        type = newType;
        spriteRenderer.sprite = tileSprites[newType % tileSprites.Length];
    }

    public void SetGridPos(Vector2Int newGridPos)
    {
        this.gridPos = newGridPos;
    }
    public void SetSelected(bool value)
    {
        spriteRenderer.color = value ? selectedColor : normalColor;
    }

    public Vector2Int GridPos => gridPos;
    public int Type => type;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTileClicked(this);
            Debug.Log($"Tile clicked via Pointer at: {gridPos}, type: {type}");
        }
    }
}
