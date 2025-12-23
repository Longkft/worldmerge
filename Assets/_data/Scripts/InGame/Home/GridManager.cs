using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [Header("Settings")]
    [SerializeField] private int columns = 4;
    [SerializeField] private float xSpacing = 1.1f;
    [SerializeField] private float ySpacing = 1.1f;

    [Header("References")]
    [SerializeField] private Transform container;      // Chứa Slot
    [SerializeField] private Transform containerItem;  // Chứa Item

    // Quản lý danh sách các Slot đã xây xong
    private List<SlotNode> _spawnedSlots = new List<SlotNode>();
    private List<ItemController> _spawnedItems = new List<ItemController>();

    public void GenerateGrid(MergeLevel levelData)
    {
        ClearGrid();

        if (levelData == null || levelData.groups == null) return;

        // --- CHUẨN BỊ DATA ---
        List<ItemData> allItemsToSpawn = new List<ItemData>();
        foreach (var group in levelData.groups)
        {
            foreach (string word in group.words)
            {
                allItemsToSpawn.Add(new ItemData(word, group.id, group.result));
            }
        }
        ShuffleList(allItemsToSpawn);

        // Tính toán kích thước lưới
        // Lưu ý: Nếu muốn lưới luôn cố định (ví dụ luôn là 4x6 dù chỉ có 10 item), 
        // bạn có thể set cứng số rows thay vì tính toán theo item count.
        int totalItems = allItemsToSpawn.Count;
        int rows = Mathf.CeilToInt((float)totalItems / columns); // Tự giãn nở theo số item

        // Hoặc fix cứng row nếu muốn map luôn to: 
        // int rows = 6; 

        float gridWidth = (columns - 1) * xSpacing;
        float gridHeight = (rows - 1) * ySpacing;
        Vector2 startPos = new Vector2(-gridWidth / 2, gridHeight / 2);

        // ============================================================
        // PHA 1: XÂY BÀN CỜ (SPAWN SLOTS)
        // ============================================================

        // Chúng ta tạo đủ số lượng Slot cho lưới (rows * columns)
        // Kể cả khi không có Item, Slot vẫn nằm đó.
        int totalSlots = rows * columns;

        for (int i = 0; i < totalSlots; i++)
        {
            int row = i / columns;
            int col = i % columns;

            float posX = startPos.x + (col * xSpacing);
            float posY = startPos.y - (row * ySpacing);
            Vector3 spawnPos = new Vector3(posX, posY, 0);

            // Tạo Slot
            SlotNode newSlot = Instantiate(PrefabManager.Instance.ItemSlot, spawnPos, Quaternion.identity, container).GetComponent<SlotNode>();
            newSlot.name = $"Slot_{row}_{col}";
            // newSlot.Init(row, col);

            // Lưu vào list để tí nữa dùng
            _spawnedSlots.Add(newSlot);
        }

        // ============================================================
        // PHA 2: RẢI ITEM VÀO SLOT (SPAWN ITEMS)
        // ============================================================

        for (int i = 0; i < allItemsToSpawn.Count; i++)
        {
            // Kiểm tra: Nếu số item nhiều hơn số slot thì dừng (tránh lỗi)
            if (i >= _spawnedSlots.Count) break;

            // Lấy cái Slot tương ứng tại vị trí i ra
            SlotNode targetSlot = _spawnedSlots[i];

            // Tạo Item tại vị trí của Slot đó
            GameObject newItemObj = Instantiate(PrefabManager.Instance.ItemGame, targetSlot.transform.position, Quaternion.identity, containerItem);
            newItemObj.name = $"Item_{allItemsToSpawn[i].word}";

            // Setup và Liên kết
            ItemController ctrl = newItemObj.GetComponent<ItemController>();
            if (ctrl != null)
            {
                // Truyền Data và cái Slot đã lấy được ở trên vào
                ctrl.Setup(allItemsToSpawn[i], targetSlot);
                _spawnedItems.Add(ctrl);
            }
        }

        Debug.Log($"Đã tạo Grid {columns}x{rows}. Slots: {_spawnedSlots.Count}, Items: {allItemsToSpawn.Count}");
    }

    private void ClearGrid()
    {
        foreach (Transform child in container) Destroy(child.gameObject);
        foreach (Transform child in containerItem) Destroy(child.gameObject);
        _spawnedSlots.Clear(); // Nhớ clear list slot
        _spawnedItems.Clear();
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int r = Random.Range(i, list.Count);
            list[i] = list[r];
            list[r] = temp;
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