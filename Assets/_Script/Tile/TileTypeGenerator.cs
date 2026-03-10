using System.Collections.Generic;
using UnityEngine;

public class TileTypeGenerator : MonoBehaviour
{
    [Header("Skin Configuration")]
    [SerializeField] private TileSkinSO currentSkin; // Kéo TileSkinSO vào đây để đổi skin

    public List<int> GenerateTypes(int totalTiles)
    {
        if (totalTiles % 2 != 0)
        {
            Debug.LogError("Total tiles phải chẵn!");
            return new List<int>();
        }

        int pairCount = totalTiles / 2;

        int availableTypes = currentSkin.TileSprites.Length;

        List<int> types = new List<int>();

        // Random type từ 0 đến availableTypes-1, lặp lại nhiều lần nếu cần
        for (int i = 0; i < pairCount; i++)
        {
            int type = Random.Range(0, availableTypes);
            types.Add(type);
            types.Add(type);
        }

        ShuffleList(types);
        return types;
    }

    public Sprite[] GetTileSprites()
    {
        return currentSkin != null ? currentSkin.TileSprites : null;
    }

    public Sprite GetSpriteForType(int type)
    {
        return currentSkin?.GetSprite(type);
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

#if UNITY_EDITOR
    // Optional: Button để test đổi skin runtime (chỉ Editor)
    [ContextMenu("Test Change Skin")]
    private void TestChangeSkin()
    {
        Debug.Log("Current skin: " + (currentSkin != null ? currentSkin.name : "None"));
    }
#endif
}