using UnityEngine;

public class popupTutorial : BasePopup
{
    [SerializeField] FxView _fx;

    // Override lại hàm Show của BasePopup
    public override void Show(System.Action onCloseCallback)
    {
        // Lưu callback của Manager lại (quan trọng!)
        base.Show(onCloseCallback);

        // Gọi FxView để chạy hiệu ứng hiện
        if (_fx != null)
        {
            _fx.ShowFx(() =>
            {
                // Có thể làm gì đó khi hiện xong (ví dụ phát âm thanh)
                Debug.Log("Popup Setting Open Complete");
            });
        }
    }

    // Gọi hàm này khi bấm nút X
    public override void Hide()
    {
        if (_fx != null)
        {
            // Gọi FxView chạy hiệu ứng ẩn
            _fx.HideFX(() =>
            {
                Debug.Log("Popup Setting Closed");

                // [CỰC KỲ QUAN TRỌNG]
                // Báo cho Manager biết là popup này đã tắt hẳn -> Manager sẽ hiện popup tiếp theo
                _onCloseCallback?.Invoke();
            });
        }
        else
        {
            // Fallback nếu không có FX
            gameObject.SetActive(false);
            _onCloseCallback?.Invoke();
        }
    }
}
