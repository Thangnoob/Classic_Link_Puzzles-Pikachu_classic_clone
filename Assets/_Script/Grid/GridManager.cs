using System.Collections.Generic;
using UnityEngine;

public enum GravityMode
{
    None,
    Down,
    Up,
    Left,
    Right,
    ToCenterHorizontal,
    ToCenterVertical
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private TileTypeGenerator typeGenerator;
    [SerializeField] private MatchPathfinder matchPathfinder;

    [Header("Grid Settings")]
    [SerializeField] private float margin = 0.4f;
    [SerializeField] private Vector2 gridOffset = Vector2.zero;
    [SerializeField] private GravityMode gravityMode = GravityMode.None;

    private Tile[,] grid;
    private List<Tile> tileList = new List<Tile>();
    private int cols, rows;
    private float tileWidth, tileHeight;
    private float startX, startY;

    public int Cols => cols;
    public int Rows => rows;

    public void SetGravityMode(GravityMode mode) => gravityMode = mode;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(int cols, int rows)
    {
        this.cols = cols + 2;
        this.rows = rows + 2;

        grid = new Tile[this.cols, this.rows];

        GridLayoutCalculator.Calculate(this.cols, this.rows, margin, out tileWidth, out tileHeight, out startX, out startY, gridOffset);
        GenerateGrid(cols, rows);
    }

    private void GenerateGrid(int playableCols, int playableRows)
    {
        ClearGrid();

        List<int> types = typeGenerator.GenerateTypes(playableCols * playableRows);
        int index = 0;

        for (int row = 1; row <= playableRows; row++)
        {
            for (int col = 1; col <= playableCols; col++)
            {
                SpawnTile(col, row, types[index]);
                index++;
            }
        }
    }

    private void SpawnTile(int col, int row, int type)
    {
        Vector3 pos = GridLayoutCalculator.GetWorldPosition(col, row, tileWidth, tileHeight, startX, startY);
        GameObject tileObj = Instantiate(tilePrefab, pos, Quaternion.identity, transform);

        SpriteRenderer sr = tileObj.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            float spriteWidth = sr.sprite.bounds.size.x;
            float spriteHeight = sr.sprite.bounds.size.y;
            float scaleX = tileWidth / spriteWidth;
            float scaleY = tileHeight / spriteHeight;
            tileObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }

        Tile tile = tileObj.GetComponent<Tile>();
        tile.SetGridPos(new Vector2Int(col, row));
        tile.SetType(type, typeGenerator.GetTileSprites());

        grid[col, row] = tile;
        tileList.Add(tile);
    }

    private void ClearGrid()
    {
        foreach (var tile in tileList)
            if (tile != null) Destroy(tile.gameObject);
        tileList.Clear();
    }

    public Tile GetTile(int col, int row)
    {
        if (col < 0 || col >= cols || row < 0 || row >= rows) return null;
        return grid[col, row];
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return GridLayoutCalculator.GetWorldPosition(
            gridPos.x,
            gridPos.y,
            tileWidth,
            tileHeight,
            startX,
            startY
        );
    }

    public List<Tile> GetActiveTiles() => tileList;

    public void RemoveTile(Tile tile)
    {
        if (tile == null) return;
        Vector2Int pos = tile.GridPos;
        grid[pos.x, pos.y] = null;
        tileList.Remove(tile);
        tile.gameObject.SetActive(false);

        ApplyGravityAfterRemove(pos);
    }

    private void ApplyGravityAfterRemove(Vector2Int removedPos)
    {
        switch (gravityMode)
        {
            case GravityMode.Down:
                ApplyGravityDown(removedPos.x);
                break;
            case GravityMode.Up:
                ApplyGravityUp(removedPos.x);
                break;
            case GravityMode.Left:
                ApplyGravityLeft(removedPos.y);
                break;
            case GravityMode.Right:
                ApplyGravityRight(removedPos.y);
                break;
            case GravityMode.ToCenterHorizontal:
                ApplyGravityToCenterHorizontal();
                break;
            case GravityMode.ToCenterVertical:
                ApplyGravityToCenterVertical();
                break;
            case GravityMode.None:
            default:
                break;
        }
    }

    // Các hàm gravity chi tiết
    // Lưu ý: playable rows/cols là từ 1..(rows-2 / cols-2), viền 0 và rows-1/cols-1 là border rỗng

    private void ApplyGravityDown(int col)
    {
        // Gom tất cả tile trong cột col (trên hàng 1..rows-2) xuống phía dưới
        List<Tile> columnTiles = new List<Tile>();
        for (int r = 1; r <= rows - 2; r++)
        {
            Tile t = grid[col, r];
            if (t != null)
            {
                columnTiles.Add(t);
                grid[col, r] = null;
            }
        }

        int targetRow = 1;
        foreach (var t in columnTiles)
        {
            Vector3 pos = GridLayoutCalculator.GetWorldPosition(col, targetRow, tileWidth, tileHeight, startX, startY);
            t.transform.position = pos;
            t.SetGridPos(new Vector2Int(col, targetRow));
            grid[col, targetRow] = t;
            targetRow++;
        }
    }

    private void ApplyGravityUp(int col)
    {
        // Gom tất cả tile trong cột col (1..rows-2) lên phía trên (sát rows-2)
        List<Tile> columnTiles = new List<Tile>();
        for (int r = 1; r <= rows - 2; r++)
        {
            Tile t = grid[col, r];
            if (t != null)
            {
                columnTiles.Add(t);
                grid[col, r] = null;
            }
        }

        int targetRow = rows - 2;
        for (int i = columnTiles.Count - 1; i >= 0; i--)
        {
            Tile t = columnTiles[i];
            Vector3 pos = GridLayoutCalculator.GetWorldPosition(col, targetRow, tileWidth, tileHeight, startX, startY);
            t.transform.position = pos;
            t.SetGridPos(new Vector2Int(col, targetRow));
            grid[col, targetRow] = t;
            targetRow--;
        }
    }

    private void ApplyGravityLeft(int row)
    {
        // Gom tất cả tile trong hàng row (1..cols-2) sang trái
        List<Tile> rowTiles = new List<Tile>();
        for (int c = 1; c <= cols - 2; c++)
        {
            Tile t = grid[c, row];
            if (t != null)
            {
                rowTiles.Add(t);
                grid[c, row] = null;
            }
        }

        int targetCol = 1;
        foreach (var t in rowTiles)
        {
            Vector3 pos = GridLayoutCalculator.GetWorldPosition(targetCol, row, tileWidth, tileHeight, startX, startY);
            t.transform.position = pos;
            t.SetGridPos(new Vector2Int(targetCol, row));
            grid[targetCol, row] = t;
            targetCol++;
        }
    }

    private void ApplyGravityRight(int row)
    {
        // Gom tất cả tile trong hàng row (1..cols-2) sang phải (sát cols-2)
        List<Tile> rowTiles = new List<Tile>();
        for (int c = 1; c <= cols - 2; c++)
        {
            Tile t = grid[c, row];
            if (t != null)
            {
                rowTiles.Add(t);
                grid[c, row] = null;
            }
        }

        int targetCol = cols - 2;
        for (int i = rowTiles.Count - 1; i >= 0; i--)
        {
            Tile t = rowTiles[i];
            Vector3 pos = GridLayoutCalculator.GetWorldPosition(targetCol, row, tileWidth, tileHeight, startX, startY);
            t.transform.position = pos;
            t.SetGridPos(new Vector2Int(targetCol, row));
            grid[targetCol, row] = t;
            targetCol--;
        }
    }

    private void ApplyGravityToCenterHorizontal()
    {
        // Dồn từ 2 bên trái/phải vào giữa theo từng hàng
        int playableMinCol = 1;
        int playableMaxCol = cols - 2;

        for (int row = 1; row <= rows - 2; row++)
        {
            List<Tile> rowTiles = new List<Tile>();
            for (int c = playableMinCol; c <= playableMaxCol; c++)
            {
                Tile t = grid[c, row];
                if (t != null)
                {
                    rowTiles.Add(t);
                    grid[c, row] = null;
                }
            }

            int left = (playableMinCol + playableMaxCol) / 2;
            int right = left + 1;
            bool placeLeft = true;

            foreach (var t in rowTiles)
            {
                int targetCol;
                if (placeLeft)
                {
                    targetCol = left;
                    left--;
                }
                else
                {
                    targetCol = right;
                    right++;
                }
                placeLeft = !placeLeft;

                if (targetCol < playableMinCol || targetCol > playableMaxCol)
                    continue;

                Vector3 pos = GridLayoutCalculator.GetWorldPosition(targetCol, row, tileWidth, tileHeight, startX, startY);
                t.transform.position = pos;
                t.SetGridPos(new Vector2Int(targetCol, row));
                grid[targetCol, row] = t;
            }
        }
    }

    private void ApplyGravityToCenterVertical()
    {
        // Dồn từ trên/dưới vào giữa theo từng cột
        int playableMinRow = 1;
        int playableMaxRow = rows - 2;

        for (int col = 1; col <= cols - 2; col++)
        {
            List<Tile> columnTiles = new List<Tile>();
            for (int r = playableMinRow; r <= playableMaxRow; r++)
            {
                Tile t = grid[col, r];
                if (t != null)
                {
                    columnTiles.Add(t);
                    grid[col, r] = null;
                }
            }

            int bottom = (playableMinRow + playableMaxRow) / 2;
            int top = bottom + 1;
            bool placeBottom = true;

            foreach (var t in columnTiles)
            {
                int targetRow;
                if (placeBottom)
                {
                    targetRow = bottom;
                    bottom--;
                }
                else
                {
                    targetRow = top;
                    top++;
                }
                placeBottom = !placeBottom;

                if (targetRow < playableMinRow || targetRow > playableMaxRow)
                    continue;

                Vector3 pos = GridLayoutCalculator.GetWorldPosition(col, targetRow, tileWidth, tileHeight, startX, startY);
                t.transform.position = pos;
                t.SetGridPos(new Vector2Int(col, targetRow));
                grid[col, targetRow] = t;
            }
        }
    }

    public void ShuffleGrid(bool ensureValidPair = false)
    {
        var remaining = GetActiveTiles();
        if (remaining.Count < 2) return;

        // Lấy danh sách vị trí hiện tại của các tile còn lại
        List<Vector2Int> positions = new List<Vector2Int>();
        foreach (var t in remaining) positions.Add(t.GridPos);

        int attempts = 0;
        const int maxAttempts = 20;

        do
        {
            ShuffleList(positions); // Fisher-Yates

            // Di chuyển tile sang vị trí mới
            for (int i = 0; i < remaining.Count; i++)
            {
                Tile tile = remaining[i];
                Vector2Int oldPos = tile.GridPos;
                Vector2Int newPos = positions[i];

                grid[oldPos.x, oldPos.y] = null;
                tile.transform.position = GridLayoutCalculator.GetWorldPosition(newPos.x, newPos.y, tileWidth, tileHeight, startX, startY);
                tile.SetGridPos(newPos);
                grid[newPos.x, newPos.y] = tile;
            }

            attempts++;
        }
        while (ensureValidPair && !matchPathfinder.HasAnyValidPair() && attempts < maxAttempts);

        if (ensureValidPair && attempts >= maxAttempts)
            Debug.LogWarning("Không tìm thấy layout có match sau 20 lần shuffle!");
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}