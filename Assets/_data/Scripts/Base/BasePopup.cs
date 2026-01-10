using UnityEngine;
using DG.Tweening; // Cần thiết để kill tween

public class BasePopup : MonoBehaviour
{
    // Callback gốc được Manager truyền vào khi Show
    protected System.Action _onCloseCallback;

    public virtual void Show(System.Action onCloseCallback)
    {
        this._onCloseCallback = onCloseCallback;
        this.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        // Các lớp con (popupEndGame...) sẽ Override hàm này để chạy Animation riêng
        // Mặc định thì cứ tắt bụp
        this.gameObject.SetActive(false);
        _onCloseCallback?.Invoke();
    }

    // --- DÙNG CHUNG CHO TẤT CẢ POPUP ---
    // Hàm này nhận lệnh từ Nút Home -> Gán hành động về Home -> Gọi Hide
    public void GracefulClose(System.Action onAnimFinished)
    {
        // 1. An toàn: Kill tween cũ trên object này
        DOTween.Kill(this.transform);

        // 2. GHI ĐÈ Callback cũ
        // Thay vì báo cáo "Đóng xong rồi", ta đổi thành "Về Home đi"
        this._onCloseCallback = () =>
        {
            onAnimFinished?.Invoke();
        };

        // 3. Gọi hàm Hide()
        // Do tính đa hình (Polymorphism), nó sẽ gọi hàm Hide() CỦA LỚP CON
        // (Tức là nó vẫn chạy Animation đẹp của popupEndGame hay popupSetting)
        this.Hide();
    }
}