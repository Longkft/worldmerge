using UnityEngine;
using DG.Tweening;

public class ItemController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ItemUI uiItem;
    [SerializeField] private ItemButton btnItem;

    public bool IsLocked { get; private set; } // GridManager dùng (Logic Data)

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
    private int zIndexToClick = 0;

    // --- Biến cờ để chặn click ---
    private bool _isMoving = false;

    private void Awake()
    {
        // Lưu vị trí hiện tại để nếu kéo sai còn biết đường quay về
        _startDragPos = transform.position;

        this.zIndex = (int)transform.position.z;
        this.zIndexToClick = this.zIndex - 2; // cho lên trên
    }

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
        if (_isMoving) return;

        // 1. Lưu vị trí hiện tại để nếu kéo sai còn biết đường quay về
        /*_startDragPos = transform.position;*/

        // 2. Đưa layer lên cao nhất để không bị các item khác che
        /*if (uiItem) uiItem.SetSortingOrder(this.zIndex);*/ // Giả sử hàm này bạn đã viết trong ItemUI

        // 3. Tính Offset: Giữ khoảng cách giữa tâm vật và con trỏ chuột
        _dragOffset = transform.position - GetWorldMousePos();
    }

    public void OnDrag()
    {
        if (_isMoving) return;

        // Di chuyển vật theo chuột (cộng thêm offset để không bị giật)
        Vector3 pos = GetWorldMousePos() + _dragOffset;
        transform.position = new Vector3(pos.x, pos.y, this.zIndexToClick);
    }

    // Thêm Getter để GridManager dùng
    public SlotNode GetOwnerSlot() => _ownerSlot;

    public void OnEndDrag()
    {
        if (_isMoving) return;
        // 1. Trả lại layer cũ (Ví dụ về 0 hoặc theo slot)
        /*if (uiItem) uiItem.SetSortingOrder(this.zIndex);*/

        // 2. Logic kiểm tra thả vào Slot nào (giữ nguyên logic cũ của bạn)
        CheckDropTarget();
    }

    private void CheckDropTarget()
    {
        Collider2D hit = Physics2D.OverlapPoint(transform.position, slotLayer);

        if (hit != null)
        {
            SlotNode targetSlot = hit.GetComponent<SlotNode>();

            // Nếu trúng slot và slot đó không phải là slot của chính mình
            if (targetSlot != null && targetSlot != _ownerSlot)
            {
                // Lấy cái item đang ngồi ở slot đích ra
                ItemController targetItem = targetSlot.GetController();

                // Nếu slot đích CÓ item VÀ item đó ĐÃ BỊ KHÓA -> Cấm đổi
                if (targetItem != null && targetItem.IsLocked)
                {
                    Debug.Log("Slot đích đã hoàn thành, không thể đổi chỗ!");

                    // Quay về chỗ cũ (Abort)
                    MoveToOwnerPosition();
                    return;
                }


                // GỌI SWAP TỪ GRID MANAGER, Nếu ok (trống hoặc item chưa khóa) thì mới cho Swap
                GridManager.Instance.OnSwapItem(this, targetSlot);
                return; // Kết thúc, việc di chuyển do GridManager lo
            }
        }

        // Nếu thả trượt hoặc thả vào chỗ cũ -> Về chỗ cũ
        MoveToOwnerPosition();
    }

    public void MoveToOwnerPosition(System.Action onComplete = null)
    {
        Vector3 targetPos;

        if (_ownerSlot != null)
        {
            Vector3 posSlot = _ownerSlot.transform.position;
            targetPos = new Vector3(posSlot.x, posSlot.y, this.zIndexToClick);
        }
        else
        {
            targetPos = _startDragPos;
        }

        // --- DOTWEEN LOGIC ---
        _isMoving = true; // Khóa input

        transform.DOMove(targetPos, 0.3f)
            .SetEase(Ease.OutCubic) // Hiệu ứng lướt mượt
            .OnComplete(() =>
            {
                _isMoving = false; // Mở khóa khi bay xong

                // Đảm bảo vị trí chính xác tuyệt đối (tránh sai số float)
                transform.position = new Vector3(targetPos.x, targetPos.y, this.zIndex);

                onComplete?.Invoke();
            });
    }

    public void LockItemComplete() // tắt btn, đổi bg
    {
        if (IsLocked) return;

        // 1. Đánh dấu về mặt Logic (để GridManager biết)
        IsLocked = true;

        // 2. Cập nhật UI
        if (uiItem) uiItem.SetCompletedState();

        // 3. TẮT COMPONENT NÚT BẤM (Cách bạn muốn)
        // Việc này sẽ chặn vĩnh viễn việc kéo thả mà không cần check if trong OnBeginDrag
        if (btnItem != null)
        {
            btnItem.enabled = false;
            // Lưu ý: Đảm bảo ItemButton kế thừa MonoBehaviour thì mới có .enabled
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