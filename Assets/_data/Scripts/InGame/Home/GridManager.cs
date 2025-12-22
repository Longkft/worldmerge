using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [Header("Settings")]
    [SerializeField] private int columns = 4; // Đặt mặc định là 4
    [SerializeField] private float xSpacing = 1.3f; // Khoảng cách ngang
    [SerializeField] private float ySpacing = 1.3f; // Khoảng cách dọc

    [Header("References")]
    [SerializeField] private Transform container;
    [SerializeField] private Transform containerItem;

    private List<ItemController> _spawnedItems = new List<ItemController>();

    public void GenerateGrid(MergeLevel levelData)
    {
        // 1. Dọn dẹp màn cũ
        ClearGrid();

        if (levelData == null || levelData.groups == null)
        {
            Debug.LogError("❌ Level Data bị null hoặc không có groups!");
            return;
        }

        // 2. Chuẩn bị dữ liệu: Chuyển đổi từ Group -> List ItemData phẳng
        List<ItemData> allItemsToSpawn = new List<ItemData>();

        foreach (var group in levelData.groups)
        {
            foreach (string word in group.words)
            {
                // Tạo data gói gọn: Từ vựng + ID Nhóm + Tên Nhóm (Kết quả)
                ItemData data = new ItemData(word, group.id, group.result);
                allItemsToSpawn.Add(data);
            }
        }

        // 3. Trộn ngẫu nhiên danh sách (Shuffle)
        ShuffleList(allItemsToSpawn);

        // 4. Tính toán kích thước lưới để căn giữa màn hình (0,0)
        int totalCount = allItemsToSpawn.Count;
        int rows = Mathf.CeilToInt((float)totalCount / columns);

        // Công thức tính độ rộng/cao của cả khối grid
        float gridWidth = (columns - 1) * xSpacing;
        float gridHeight = (rows - 1) * ySpacing;

        // Điểm bắt đầu (Góc trên-trái)
        Vector2 startPos = new Vector2(-gridWidth / 2, gridHeight / 2);

        // 5. Vòng lặp sinh ra Item
        for (int i = 0; i < totalCount; i++)
        {
            // Tính dòng và cột hiện tại
            int row = i / columns;
            int col = i % columns;

            // Tính tọa độ World Space
            float posX = startPos.x + (col * xSpacing);
            float posY = startPos.y - (row * ySpacing); // Trừ Y vì đi từ trên xuống
            Vector3 spawnPos = new Vector3(posX, posY, 0);

            // --- Lấy Prefab từ Singleton PrefabManager ---
            if (PrefabManager.Instance == null || PrefabManager.Instance.ItemGame == null)
            {
                Debug.LogError("❌ Lỗi: Chưa setup PrefabManager hoặc chưa gắn ItemGame!");
                return;
            }

            GameObject newObj = Instantiate(PrefabManager.Instance.ItemGame, spawnPos, Quaternion.identity, container);

            // Đặt tên cho dễ nhìn trong Hierarchy
            newObj.name = $"Item_{i}_{allItemsToSpawn[i].word}";

            // Setup dữ liệu cho ItemController
            ItemController ctrl = newObj.GetComponent<ItemController>();
            if (ctrl != null)
            {
                ctrl.Setup(allItemsToSpawn[i]);
                _spawnedItems.Add(ctrl);
            }
        }

        Debug.Log($"✅ Đã tạo Grid {columns}x{rows} với {totalCount} items.");
    }

    private void ClearGrid()
    {
        foreach (var item in _spawnedItems)
        {
            if (item != null) Destroy(item.gameObject);
        }
        _spawnedItems.Clear();
    }

    /// Thuật toán Fisher-Yates để trộn danh sách
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    // Vẽ Gizmos để căn chỉnh trong Scene View (khi chưa Play)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        // Vẽ giả lập 24 ô (4*6)
        int demoCount = columns * 6;
        int rows = Mathf.CeilToInt((float)demoCount / columns);
        float gridWidth = (columns - 1) * xSpacing;
        float gridHeight = (rows - 1) * ySpacing;
        Vector2 startPos = new Vector2(-gridWidth / 2, gridHeight / 2);

        for (int i = 0; i < demoCount; i++)
        {
            int row = i / columns;
            int col = i % columns;
            float posX = startPos.x + (col * xSpacing);
            float posY = startPos.y - (row * ySpacing);

            // Vẽ ô vuông đại diện
            Gizmos.DrawWireCube(new Vector3(posX, posY, 0), new Vector3(1, 1, 0));
        }
    }
}
