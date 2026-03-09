using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int type; //ID type of image (0-71 for 72 images)
    private SpriteRenderer spriteRenderer;
    private Vector2Int gridPos;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    private void OnMouseDown()
    {
        Debug.Log($"Tile clicked at grid position: {gridPos}, type: {type}");   
    }
}
