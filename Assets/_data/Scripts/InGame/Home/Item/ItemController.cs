using UnityEngine;

public class ItemController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ItemUI uiItem;
    [SerializeField] private ItemButton btnItem;

    // Dữ liệu
    public ItemData Data { get; private set; }
    private SlotNode _ownerSlot;

    [Header("Settings")]
    // Kéo chọn Layer "Slot" vào đây trong Inspector
    [SerializeField] private LayerMask slotLayer;

    // Biến tính toán kéo thả
    private Vector3 _dragOffset;
    private Vector3 _startDragPos; // Lưu vị trí gốc để nếu thả sai thì bay về
    private int zIndex = 0;

    // --- SETUP ---
    public void Setup(ItemData data, SlotNode ownerSlot)
    {
        this.Data = data;
        this._ownerSlot = ownerSlot;

        if (uiItem != null)
        {
            uiItem.InitUi();
            uiItem.SetDataText(data.word);
        }

        if (btnItem != null)
        {
            btnItem.Init(this); // Kết nối với nút bấm
        }
    }

    public void SetOwnerSlot(SlotNode slot)
    {
        _ownerSlot = slot;
    }

    // --- CÁC HÀM XỬ LÝ KÉO THẢ (Được gọi từ ItemButton) ---

    public void OnBeginDrag()
    {
        // 1. Lưu vị trí hiện tại để nếu kéo sai còn biết đường quay về
        _startDragPos = transform.position;

        // 2. Đưa layer lên cao nhất để không bị các item khác che
        if (uiItem) uiItem.SetSortingOrder(this.zIndex); // Giả sử hàm này bạn đã viết trong ItemUI

        // 3. Tính Offset: Giữ khoảng cách giữa tâm vật và con trỏ chuột
        _dragOffset = transform.position - GetWorldMousePos();
    }

    public void OnDrag()
    {
        // Di chuyển vật theo chuột (cộng thêm offset để không bị giật)
        transform.position = GetWorldMousePos() + _dragOffset;
        this.zIndex = (int)transform.position.z;
    }

    public void OnEndDrag()
    {
        // 1. Trả lại layer cũ (Ví dụ về 0 hoặc theo slot)
        if (uiItem) uiItem.SetSortingOrder(this.zIndex);

        // 2. Logic kiểm tra thả vào Slot nào (giữ nguyên logic cũ của bạn)
        CheckDropTarget();
    }

    private void CheckDropTarget()
    {
        // QUAN TRỌNG: Thêm tham số slotLayer vào hàm OverlapPoint
        // Lúc này tia Raycast sẽ nhìn xuyên qua Item và chỉ chặn lại khi gặp Slot
        Collider2D hit = Physics2D.OverlapPoint(transform.position, slotLayer);

        /*Debug.Log($"hit: {hit.name}");*/

        if (hit != null)
        {
            // Vì đã lọc layer Slot rồi, nên cái hit chắc chắn là Slot (hoặc null)
            SlotNode targetSlot = hit.GetComponent<SlotNode>();

            Debug.Log($"targetSlot: {targetSlot.name}");
            if (targetSlot != null && targetSlot != _ownerSlot)
            {
                Debug.Log($"Thả vào slot: {targetSlot.name}");

                // GỌI LOGIC SWAP Ở ĐÂY
                // GameManager.Instance.SwapSlots(_ownerSlot, targetSlot);
                return;
            }
        }

        // Không trúng Slot nào -> Về chỗ cũ
        MoveToOwnerPosition();
    }

    public void MoveToOwnerPosition()
    {
        if (_ownerSlot != null)
        {
            // Reset về vị trí của Slot
            transform.position = _ownerSlot.transform.position;
        }
        else
        {
            // Fallback: Về vị trí lúc bắt đầu kéo
            transform.position = _startDragPos;
        }
    }

    // --- HÀM QUAN TRỌNG NHẤT: LẤY TỌA ĐỘ CHUỘT CHUẨN ---
    private Vector3 GetWorldMousePos()
    {
        Vector3 screenPoint = Input.mousePosition;

        // BẮT BUỘC: Tính khoảng cách Z từ Camera đến vật
        // Nếu Camera Z = -10, Vật Z = 0 -> distance = 10
        screenPoint.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPoint);

        // Khóa Z lại bằng Z của vật để nó không bay lung tung
        worldPos.z = transform.position.z;

        return worldPos;
    }
}