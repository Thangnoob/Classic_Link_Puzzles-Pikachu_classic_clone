using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GridManager gridManager;
    [SerializeField] private MatchPathfinder matchPathfinder;

    private Tile firstSelected;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gridManager.Initialize(16, 9);
    }

    public void OnTileClicked(Tile clicked)
    {
        if (firstSelected == null)
        {
            firstSelected = clicked;
            Debug.Log($"Đã chọn tile: {clicked.GridPos}");
            return;
        }

        if (firstSelected == clicked)
        {
            firstSelected = null;
            return;
        }

        // Kiểm tra match
        if (matchPathfinder.CanConnect(firstSelected, clicked))
        {
            Debug.Log("MATCH SUCCESS!");
            gridManager.RemoveTile(firstSelected);
            gridManager.RemoveTile(clicked);

            // Nếu hết cặp → shuffle đảm bảo còn đường
            if (!matchPathfinder.HasAnyValidPair())
            {
                Debug.Log("Hết cặp → Auto Shuffle...");
                gridManager.ShuffleGrid(true); // ← ensure valid pair
            }
        }
        else
        {
            Debug.Log("Không nối được");
        }

        firstSelected = null;
    }
}