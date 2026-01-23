using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [SerializeField] private Transform containerCompleted;  // Chứa Item

    // Quản lý danh sách các Slot đã xây xong
    private List<SlotNode> _spawnedSlots = new List<SlotNode>();
    private List<ItemController> _spawnedItems = new List<ItemController>();

    private float VEC_UNDER_MAP = 0.2f;

    private bool _isGameFinished = false; // Cờ đánh dấu game đã kết thúc chưa

    // lưu lại số hàng (tính toán ở GenerateGrid)
    private int _totalRows;

    public event Action<int> OnNumberRowsChanged;
    private int _numberRowsCompleted;
    // xem số hàng đã được completed là bao nhiêu
    public int NumberRowsCompleted
    {
        get => _numberRowsCompleted;
        set
        {
            // Chỉ cập nhật nếu giá trị mới khác giá trị cũ (tối ưu)
            if (_numberRowsCompleted != value)
            {
                _numberRowsCompleted = value;

                // BẮN SỰ KIỆN NGAY LẬP TỨC!
                // Bất kỳ ai đăng ký lắng nghe sẽ được gọi
                OnNumberRowsChanged?.Invoke(_numberRowsCompleted);
            }
        }
    }

    private void OnDisable()
    {
        this.ClearGrid();
    }

    public void GenerateGrid(MergeLevel levelData)
    {
        ClearGrid();

        _isGameFinished = false;

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
        Vector2 startPos = new Vector2(-gridWidth / 2, (gridHeight / 2) - this.VEC_UNDER_MAP);

        // ============================================================
        // PHA 1: XÂY BÀN CỜ (SPAWN SLOTS)
        // ============================================================

        // Chúng ta tạo đủ số lượng Slot cho lưới (rows * columns)
        // Kể cả khi không có Item, Slot vẫn nằm đó.
        int totalSlots = rows * columns;
        // Lưu lại số hàng để dùng cho CheckMatch
        _totalRows = rows;

        for (int i = 0; i < totalSlots; i++)
        {
            int row = i / columns;
            int col = i % columns;

            float posX = startPos.x + (col * xSpacing);
            float posY = startPos.y - (row * ySpacing);
            Vector3 spawnPos = new Vector3(posX, posY, 2);

            // Tạo Slot
            SlotNode newSlot = Instantiate(PrefabManager.Instance.ItemSlot, spawnPos, Quaternion.identity, container).GetComponent<SlotNode>();
            newSlot.name = $"Slot_{row}_{col}";
            newSlot.Init(row, col);

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
            Vector3 pos = targetSlot.transform.position;
            Vector3 spawnPos = new Vector3(pos.x, pos.y, 0);

            // Tạo Item tại vị trí của Slot đó
            GameObject newItemObj = Instantiate(PrefabManager.Instance.ItemGame, spawnPos, Quaternion.identity, containerItem);
            newItemObj.name = $"Item_{allItemsToSpawn[i].word}";

            // Setup và Liên kết
            ItemController ctrl = newItemObj.GetComponent<ItemController>();
            if (ctrl != null)
            {
                // Truyền Data và cái Slot đã lấy được ở trên vào
                ctrl.Setup(allItemsToSpawn[i], targetSlot);

                // --- Slot biết Item ---
                targetSlot.LinkController(ctrl);

                _spawnedItems.Add(ctrl);
            }
        }

        Debug.Log($"Đã tạo Grid {columns}x{rows}. Slots: {_spawnedSlots.Count}, Items: {allItemsToSpawn.Count}");
    }

    private void ClearGrid()
    {
        foreach (Transform child in container) Destroy(child.gameObject);
        foreach (Transform child in containerItem) Destroy(child.gameObject);
        foreach (Transform child in containerCompleted) Destroy(child.gameObject);
        _spawnedSlots.Clear(); // Nhớ clear list slot
        _spawnedItems.Clear();

        this.NumberRowsCompleted = 0;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int r = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[r];
            list[r] = temp;
        }
    }

    /*// Vẽ Gizmos để căn chỉnh trong Scene View (khi chưa Play)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        // Vẽ giả lập 24 ô (4*6)
        int demoCount = columns * 6;
        int rows = Mathf.CeilToInt((float)demoCount / columns);
        float gridWidth = (columns - 1) * xSpacing;
        float gridHeight = (rows - 1) * ySpacing;
        Vector2 startPos = new Vector2(-gridWidth / 2, (gridHeight / 2) - this.VEC_UNDER_MAP);

        for (int i = 0; i < demoCount; i++)
        {
            int row = i / columns;
            int col = i % columns;
            float posX = startPos.x + (col * xSpacing);
            float posY = startPos.y - (row * ySpacing);

            // Vẽ ô vuông đại diện
            Gizmos.DrawWireCube(new Vector3(posX, posY, 0), new Vector3(1, 1, 0));
        }
    }*/

    // Thêm hàm này vào GridManager
    public void OnSwapItem(ItemController itemDrag, SlotNode targetSlot)
    {
        // 1. Lấy thông tin
        SlotNode sourceSlot = itemDrag.GetOwnerSlot(); // Slot cũ của thằng đang kéo
        ItemController itemTarget = targetSlot.GetController(); // Thằng đang nằm ở slot đích

        // --- LƯU LẠI INDEX HÀNG TRƯỚC KHI SWAP ---
        int sourceRowIndex = sourceSlot.Row;
        int targetRowIndex = targetSlot.Row;

        // 2. LOGIC SWAP DỮ LIỆU

        // Slot đích -> nhận Item đang kéo
        targetSlot.LinkController(itemDrag);

        // Slot nguồn -> nhận Item đích (nếu có) hoặc để trống
        if (itemTarget != null)
        {
            sourceSlot.LinkController(itemTarget);
            // Bảo thằng bị đổi chỗ bay về nhà mới (Slot nguồn)
            itemTarget.MoveToOwnerPosition();
        }
        else
        {
            // Nếu slot đích vốn trống -> Slot nguồn giờ thành trống
            sourceSlot.LinkController(null);
        }

        // 3. LOGIC VISUAL
        // Bảo thằng đang kéo bay về nhà mới (Slot đích)
        /*itemDrag.MoveToOwnerPosition();*/

        // 4. --- CHỈ CHECK 2 HÀNG LIÊN QUAN ---

        // Check hàng cũ
        System.Action runCheckLogic = () =>
        {
            Debug.Log($"Swap visual xong: {sourceSlot.name} <-> {targetSlot.name}. Giờ mới check!");

            // Check hàng cũ
            CheckSpecificRow(sourceRowIndex);

            // Check hàng mới (nếu khác hàng cũ)
            if (sourceRowIndex != targetRowIndex)
            {
                CheckSpecificRow(targetRowIndex);
            }
        };

        itemDrag.MoveToOwnerPosition(runCheckLogic);

        Debug.Log($"Swap thành công: {sourceSlot.name} <-> {targetSlot.name}");
    }

    // --- HÀM MỚI: CHỈ CHECK 1 HÀNG CỤ THỂ ---
    private async void CheckSpecificRow(int rowIndex)
    {
        List<ItemController> rowItems = new List<ItemController>();
        string firstGroupId = "";
        string firstgroupName = "";
        bool isRowFullAndSame = true;

        // Duyệt qua các cột trong hàng rowIndex này thôi
        for (int col = 0; col < columns; col++)
        {
            // Tính index phẳng
            int index = rowIndex * columns + col;

            if (index >= _spawnedSlots.Count)
            {
                isRowFullAndSame = false;
                break;
            }

            SlotNode slot = _spawnedSlots[index];
            ItemController item = slot.GetController();

            // 1. Nếu ô trống -> Fail
            if (item == null)
            {
                isRowFullAndSame = false;
                break;
            }

            // 2. Nếu đã Locked -> Bỏ qua logic check (coi như hàng này xong rồi hoặc hỏng)
            // Lưu ý: Nếu muốn check lại cả hàng đã xong thì bỏ dòng này, 
            // nhưng thường xong rồi thì ko cần check nữa.
            if (item.IsLocked)
            {
                isRowFullAndSame = false;
                break;
            }

            // 3. Logic so sánh ID
            if (col == 0)
            {
                firstGroupId = item.Data.groupId;
            }
            else
            {
                if (item.Data.groupId != firstGroupId)
                {
                    isRowFullAndSame = false;
                    break;
                }
            }

            rowItems.Add(item);
        }

        /*this.NumberRowsCompleted = 6; // dùng để check
        Debug.Log("this.NumberRowsCompleted: " + this.NumberRowsCompleted);*/

        // KẾT QUẢ
        if (isRowFullAndSame && rowItems.Count >= columns)
        {
            Debug.Log($"Hàng {rowIndex} hoàn thành nhóm: {firstGroupId}");

            firstgroupName = rowItems[0].Data.groupName;

            foreach (var item in rowItems)
            {
                item.LockItemComplete();
            }

            // CHUẨN BỊ DATA
            string title = firstgroupName; // VD: COLOR
            string content = "";
            for (int i = 0; i < rowItems.Count; i++)
            {
                content += rowItems[i].Data.word + (i < rowItems.Count - 1 ? ", " : "");
            }

            // TÍNH VỊ TRÍ ĐỂ ĐÈ LÊN
            // Tìm index của Slot đầu tiên trong hàng này
            int firstSlotIndex = rowIndex * columns;

            // Kiểm tra an toàn
            if (firstSlotIndex < _spawnedSlots.Count)
            {
                // Lấy vị trí Y của cái SLOT (Slot không bao giờ di chuyển)
                float fixedY = _spawnedSlots[firstSlotIndex].transform.position.y;

                // Set vị trí spawn (X=0, Y=theo Slot, Z=-2 để đè lên item nằm im)
                Vector3 spawnPos = new Vector3(0, fixedY, -1);

                GameObject completedRowPrefab = PrefabManager.Instance.ItemCompleted;
                // 3. SINH RA THANH MỚI
                if (completedRowPrefab != null)
                {
                    var rowObj = Instantiate(completedRowPrefab, spawnPos, Quaternion.identity, containerCompleted);
                    rowObj.GetComponent<RowCompletedView>().Setup(title, content);

                    // eff
                    rowObj.GetComponent<RowCompletedView>().ActiveEff();
                }
            }

            this.NumberRowsCompleted++;

            // ================================================================
            // LOGIC TUTORIAL
            // ================================================================

            // Kiểm tra: Nếu chưa từng hiện Tutorial (giá trị 0) thì mới hiện
            if (PlayerPrefs.GetInt("TUTORIAL_MERGE_SHOWN", 0) == 0)
            {
                /*// 1. Lấy Tiêu đề (Tên nhóm)
                string title = firstGroupId;

                // 2. Tạo nội dung: Nối tên các item lại (VD: "RED, GREEN, BLUE")
                string content = "";
                for (int i = 0; i < rowItems.Count; i++)
                {
                    // Cộng dồn tên item, thêm dấu phẩy nếu chưa phải thằng cuối
                    content += rowItems[i].Data.word + (i < rowItems.Count - 1 ? ", " : "");
                }*/

                // 3. Lấy tọa độ Y của hàng vừa ăn (Lấy item đầu tiên làm mốc)
                // Vì các item cùng hàng ngang nhau nên lấy thằng nào cũng được
                float itemWorldY = rowItems[0].transform.position.y;

                // 4. Gọi PopupManager hiện Tutorial
                if (PopupManager.Instance != null)
                {
                    PopupManager.Instance.ShowPopupTutorial(title, content, itemWorldY);
                }

                // 5. Lưu lại trạng thái đã xem (Set thành 1) để lần sau không hiện nữa
                PlayerPrefs.SetInt("TUTORIAL_MERGE_SHOWN", 1);
                PlayerPrefs.Save();
            }
            // ================================================================
        }

        // Chặn ngay từ đầu nếu game đã xong
        if (_isGameFinished) return;
        if (this.NumberRowsCompleted == Config.NUMBER_ENDGAME)
        {
            AudioManager.Instance.PlaySFX(SoundType.Win_Level);

            await Utils.AwaitTime(2, this.destroyCancellationToken);

            Debug.Log("End Game");
            _isGameFinished = true;
            PopupManager.Instance.ShowPopupEndGame();
        }
    }
}