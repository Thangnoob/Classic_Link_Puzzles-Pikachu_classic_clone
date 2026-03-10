using UnityEngine;

public static class GridLayoutCalculator
{
    public static void Calculate(
        int cols, int rows,
        float margin,
        out float tileWidth, out float tileHeight,
        out float startX, out float startY,
        Vector2 gridOffset = default)
    {
        //tính chiều rộng màn hình thế giới dựa trên orthographicSize và aspect ratio (tỷ lệ màn hình) của camera
        float screenWidthWorld = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        //chiều rộng khả dụng sau khi trừ đi margin ở hai bên
        float availableWidth = screenWidthWorld - (2f * margin);

        //tính chiều dài màn hình thế giới dựa trên orthographicSize (chiều cao của camera)
        float screenHeightWorld = Camera.main.orthographicSize * 2f;
        //chiều dài khả dụng sau khi trừ đi margin ở hai bên
        float availableHeight = screenHeightWorld - (2f * margin);

        //fix theo chiều hẹp hơn để đảm bảo tất cả các ô vuông đều
        float tileSize = Mathf.Min(availableWidth / cols, availableHeight / rows);
        tileWidth = tileSize;
        tileHeight = tileSize;

        //tính vị trí bắt đầu để căn giữa lưới
        startX = -(cols * tileWidth) / 2f + tileWidth / 2f + gridOffset.x;
        startY = (rows * tileHeight) / 2f - tileHeight / 2f + gridOffset.y;
    }

    public static Vector3 GetWorldPosition(int col, int row, float tileWidth, float tileHeight, float startX, float startY)
    {
        float x = startX + col * tileWidth;
        float y = startY - row * tileHeight;
        return new Vector3(x, y, 0);
    }
}