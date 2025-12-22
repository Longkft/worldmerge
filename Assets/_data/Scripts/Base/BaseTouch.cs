using UnityEngine;
using UnityEngine.EventSystems; // Bắt buộc để dùng các Interface

// Kế thừa các Interface của Unity để bắt sự kiện
public class BaseTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
{
    [Header("Base Touch Settings")]
    [SerializeField] protected bool isInteractable = true; // Biến để bật tắt tương tác nếu cần

    // Properties để các class con check trạng thái
    public bool IsDragging { get; private set; }
    public bool IsTouching { get; private set; }

    // --- 1. TOUCH START (Khi ngón tay vừa chạm vào) ---
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable) return;

        IsTouching = true;
        IsDragging = false;
        OnTouchStart(eventData);
    }

    // --- 3. TOUCH END (Khi nhấc ngón tay lên) ---
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable) return;

        IsTouching = false;
        IsDragging = false;
        OnTouchEnd(eventData);
    }

    // --- 4. CLICK (Chạm và nhấc lên nhanh tại chỗ - giống Tap) ---
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInteractable) return;

        // Chỉ gọi click nếu không phải là đang kéo (tránh conflict logic)
        if (!IsDragging)
        {
            OnTap(eventData);
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // Mặc định để trống, con thích làm gì thì làm
    }

    // 2. Sửa OnDrag: Thêm 'virtual'
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isInteractable) return;

        IsDragging = true;
        OnTouchMove(eventData); // Gọi hàm logic game
    }

    // 3. Thêm IEndDragHandler
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // Mặc định để trống
    }

    // --- CÁC HÀM VIRTUAL ĐỂ CLASS CON OVERRIDE ---
    protected virtual void OnTouchStart(PointerEventData data) { }
    protected virtual void OnTouchMove(PointerEventData data) { }
    protected virtual void OnTouchEnd(PointerEventData data) { }
    protected virtual void OnTap(PointerEventData data) { }
}