using TMPro;
using UnityEngine;

public class WordModeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI levelWord;

    private int level = -1;
    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện thay đổi Level
        // Hàm SetUiLevelWord giờ nhận vào 1 số int (level mới)
        if (HomeManager.Instance != null)
        {
            HomeManager.Instance.OnLevelChanged += SetUiLevelWord;

            // Cập nhật ngay lập tức trạng thái hiện tại (phòng khi UI bật sau khi data đã load)
            SetUiLevelWord(HomeManager.Instance.LevelCurrent);
        }
    }
    private void OnDisable()
    {
        if (HomeManager.Instance != null)
        {
            HomeManager.Instance.OnLevelChanged -= SetUiLevelWord;
        }
    }

    // Sửa hàm này nhận tham số int để khớp với Action<int>
    public void SetUiLevelWord(int newLevel)
    {
        levelWord.text = $"Lv.{newLevel}";
        Debug.Log("UI Updated Level: " + newLevel);
    }
}
