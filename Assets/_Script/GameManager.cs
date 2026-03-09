using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] private int cols = 16;
    [SerializeField] private int rows = 9;
    [SerializeField] private float tileWidth = 1f;
    [SerializeField] private float tileHeight = 1f;
    [SerializeField] private Vector2 gridOffset = Vector2.zero;

    [Header("Prefabs and Assets")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Sprite[] tileSprites;

    private Tile[,] grid;
    private List<Tile> tileList = new List<Tile>();

    private float startX;
    private float startY;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        grid = new Tile[cols, rows];
    }

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        // Clear old grid
        foreach (Tile tile in tileList)
        {
            if (tile != null)
                Destroy(tile.gameObject);
        }

        tileList.Clear();

        List<int> types = GenerateTileTypes();
        int index = 0;

        // Calculate grid center
        startX = -(cols * tileWidth) / 2f + tileWidth / 2f + gridOffset.x;
        startY = (rows * tileHeight) / 2f - tileHeight / 2f + gridOffset.y;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                SpawnTile(col, row, types[index]);
                index++;
            }
        }
    }

    private Tile SpawnTile(int col, int row, int type)
    {
        Vector3 pos = GetWorldPosition(col, row);

        GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
        Tile tile = tileObj.GetComponent<Tile>();

        tile.SetGridPos(new Vector2Int(col, row));
        tile.SetType(type, tileSprites);

        grid[col, row] = tile;
        tileList.Add(tile);

        return tile;
    }

    private Vector3 GetWorldPosition(int col, int row)
    {
        float x = startX + col * tileWidth;
        float y = startY - row * tileHeight;

        return new Vector3(x, y, 0);
    }

    private List<int> GenerateTileTypes()
    {
        List<int> types = new List<int>();

        int totalTiles = cols * rows;
        int pairCount = totalTiles / 2;

        for (int i = 0; i < pairCount; i++)
        {
            int type = Random.Range(0, tileSprites.Length);

            types.Add(type);
            types.Add(type);
        }

        ShuffleList(types);

        return types;
    }

    // Fisher-Yates shuffle
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public Tile GetTile(int col, int row)
    {
        if (col < 0 || col >= cols || row < 0 || row >= rows)
            return null;

        return grid[col, row];
    }
}