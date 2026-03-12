using UnityEngine;

[CreateAssetMenu(fileName = "NewTileSkin", menuName = "Pikachu/Tile Skin", order = 1)]
public class TileSkinSO : ScriptableObject
{
    [Tooltip("Mảng sprites tương ứng với type 0 → n. Số lượng phải >= số pairs tối đa (ví dụ 72 sprites cho 36 pairs)")]
    [SerializeField] private Sprite[] tileSprites;

    public Sprite[] TileSprites => tileSprites;

    // Optional: Getter an toàn
    public Sprite GetSprite(int type)
    {
        if (tileSprites == null || tileSprites.Length == 0)
        {
            Debug.LogWarning("TileSkinSO chưa có sprite nào!");
            return null;
        }
        return tileSprites[type % tileSprites.Length]; // Cycle nếu type vượt quá
    }

    // Optional: Kiểm tra số lượng sprites hợp lệ
    public bool IsValidForPairs(int pairCount)
    {
        return tileSprites != null && tileSprites.Length >= pairCount;
    }
}