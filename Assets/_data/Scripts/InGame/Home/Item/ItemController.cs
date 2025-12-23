using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private ItemUI uiItem;
    [SerializeField] private ItemButton btnItem;

    public ItemData Data { get; private set; }

    // Slot "chủ nhân" hiện tại (Logic)
    private SlotNode _ownerSlot;
    private Vector3 _dragOffset;

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
            btnItem.Init(this);
        }
    }

    // Gọi khi GridManager tạo ra item, hoặc khi Swap xong
    public void SetOwnerSlot(SlotNode slot)
    {
        _ownerSlot = slot;
    }

    // --- LOGIC KÉO THẢ (Được gọi từ ItemButton) ---

    public void OnBeginDrag()
    {
        // 1. Đưa layer lên cao nhất (ví dụ +100)
        if (uiItem) uiItem.SetSortingOrder(uiItem.GetDefaultSortingOrder() + 100);

        // 2. Tính offset để kéo mượt (không bị giật về tâm chuột)
        Vector3 mousePos = GetWorldMousePos();
        _dragOffset = transform.position - mousePos;
    }

    public void OnDrag()
    {
        // Di chuyển theo chuột
        Vector3 mousePos = GetWorldMousePos();
        transform.position = mousePos + _dragOffset;
    }

    public void OnEndDrag()
    {
        // 1. Trả lại layer cũ
        if (uiItem) uiItem.SetSortingOrder(uiItem.GetDefaultSortingOrder());

        // 2. Bắn Raycast kiểm tra xem thả vào đâu
        CheckDropTarget();
    }

    private void CheckDropTarget()
    {
        // Raycast tại vị trí hiện tại của Item
        Collider2D hit = Physics2D.OverlapPoint(transform.position);

        if (hit != null)
        {
            // Tìm component SlotNode trên vật bị va chạm
            SlotNode targetSlot = hit.GetComponent<SlotNode>();

            // Nếu trúng slot khác và slot đó không phải là slot hiện tại của mình
            if (targetSlot != null && targetSlot != _ownerSlot)
            {
                // GỌI SWAP DATA
                // Lưu ý: Hàm này sẽ hoán đổi Data giữa _ownerSlot và targetSlot
                // Sau khi swap, Visual sẽ tự cập nhật nội dung chữ
                /*GameManager.Instance.SwapSlots(_ownerSlot, targetSlot);*/ // tạm tắt

                // Swap xong thì nhiệm vụ kết thúc, ItemController này sẽ ở yên tại chỗ (về mặt logic)
                // nhưng hiển thị nội dung mới. Vị trí visual cần bay về slot chủ.
            }
        }

        // Dù swap hay không, visual cũng phải bay về vị trí của Slot chủ nhân
        MoveToOwnerPosition();
    }

    public void MoveToOwnerPosition()
    {
        if (_ownerSlot != null)
        {
            // Nếu có DOTween
            // transform.DOMove(_ownerSlot.transform.position, 0.2f);

            // Nếu không dùng plugin:
            transform.position = _ownerSlot.transform.position;
        }
    }

    private Vector3 GetWorldMousePos()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        pos.z = 0;
        return pos;
    }
}