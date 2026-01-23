using UnityEngine;
using UnityEngine.EventSystems;

public class BtnPlayNow : BaseTouch
{
    [Header("Cấu hình Nút")]
    // [THÊM] Chọn Word hoặc Math ở Inspector
    [SerializeField] private GamePlayMode modeToPlay;

    protected override void OnTouchStart(PointerEventData data)
    {
        // HomeManager.StartGameAtLevel đã gọi chuyển Scene rồi

        // [SỬA] Phân loại logic click dựa trên modeToPlay
        if (modeToPlay == GamePlayMode.Word)
        {
            // Nếu là nút Word -> Gọi hàm chơi Word
            HomeManager.Instance.PlayWordMode();
        }
        else
        {
            // Nếu là nút Math -> Gọi hàm chơi Math
            HomeManager.Instance.PlayMathMode();
        }

        // Lưu ý: PlayWordMode/PlayMathMode bên HomeManager đã bao gồm logic:
        // 1. Set SelectedGamePlayMode
        // 2. Chuyển Scene
        // 3. Generate Grid
    }
}