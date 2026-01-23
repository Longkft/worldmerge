using TMPro;
using UnityEngine;

// [ĐỔI TÊN] Đổi thành ModeLevelUI để dùng chung cho cả Word và Math
public class ModeLevelUI : MonoBehaviour
{
    [Header("Settings")]
    // [THÊM] Biến này để chọn xem UI này hiển thị cho chế độ nào
    [SerializeField] private GamePlayMode targetMode;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI levelWord;

    private void OnEnable()
    {
        if (HomeManager.Instance == null) return;

        // [SỬA] Logic đăng ký sự kiện dựa trên targetMode
        if (targetMode == GamePlayMode.Word)
        {
            // Lắng nghe sự kiện của Word
            HomeManager.Instance.OnWordLevelChanged += SetUiLevelText;

            // Cập nhật ngay lập tức level hiện tại của Word
            SetUiLevelText(HomeManager.Instance.WordLevelIndex);
        }
        else // Math
        {
            // Lắng nghe sự kiện của Math
            HomeManager.Instance.OnMathLevelChanged += SetUiLevelText;

            // Cập nhật ngay lập tức level hiện tại của Math
            SetUiLevelText(HomeManager.Instance.MathLevelIndex);
        }
    }

    private void OnDisable()
    {
        if (HomeManager.Instance == null) return;

        // [SỬA] Hủy đăng ký đúng sự kiện để tránh lỗi
        if (targetMode == GamePlayMode.Word)
        {
            HomeManager.Instance.OnWordLevelChanged -= SetUiLevelText;
        }
        else
        {
            HomeManager.Instance.OnMathLevelChanged -= SetUiLevelText;
        }
    }

    // [SỬA] Hàm cập nhật UI chung
    private void SetUiLevelText(int newLevel)
    {
        if (levelWord != null)
        {
            levelWord.text = $"Lv.{newLevel}";
            // Debug.Log($"UI {targetMode} Updated: Lv.{newLevel}");
        }
    }
}