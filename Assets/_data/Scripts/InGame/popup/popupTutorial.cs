using UnityEngine;
using TMPro;

public class popupTutorial : BasePopup
{
    [Header("Components")]
    [SerializeField] FxView _fx;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI txtTitle;   // Hiển thị tên nhóm (COLOR...)
    [SerializeField] private TextMeshProUGUI txtContent; // Hiển thị list item (GREEN, RED...)
    [SerializeField] private RectTransform contentBox;

    private Canvas parentCanvas;

    private float _targetWorldY;

    //Hàm này để Manager gọi ngay khi tạo popup
    public void SetCanvas(Canvas canvas)
    {
        this.parentCanvas = canvas;
    }

    public void ShowTutorial(System.Action onCloseCallback, string title, string content, float worldY)
    {
        // 1. Setup nội dung
        if (txtTitle) txtTitle.text = title;
        if (txtContent) txtContent.text = content;

        _targetWorldY = worldY;

        // 2. Tính toán vị trí UI dựa trên World Y
        UpdatePosition();

        // 3. Gọi hàm Show gốc (để chạy FxView)
        this.Show(onCloseCallback);
    }

    private void UpdatePosition()
    {
        if (contentBox == null || parentCanvas == null) return;

        // Giả sử item nằm ở X=0 trong World, chỉ lấy Y
        Vector3 targetWorldPos = new Vector3(0, _targetWorldY, 0);

        // Lấy Camera chính (quay vào bàn cờ)
        Camera worldCam = Camera.main;

        // 1. Chuyển từ World -> Screen Point
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(worldCam, targetWorldPos);

        // 2. Chuyển từ Screen Point -> Local Point trong Canvas
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            screenPoint,
            parentCanvas.worldCamera, // Camera của UI (nếu Overlay thì null cũng được)
            out localPos
        );

        // 3. Gán vị trí (Giữ X = 0 giữa màn hình, chỉ thay đổi Y)
        contentBox.anchoredPosition = new Vector2(0, localPos.y);
    }

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
