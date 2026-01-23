using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [Header("Settings")]
    public GamePlayMode targetMode; // Chọn Word hoặc Math ở Inspector
    public Slider sliderUI;

    private void OnEnable()
    {
        // Đăng ký sự kiện
        HomeManager.OnUpdateProgress += HandleProgressUpdate;

        // Mẹo: Gọi refresh ngay khi UI bật lên để đảm bảo luôn đúng data mới nhất
        if (HomeManager.Instance != null)
        {
            HomeManager.Instance.RefreshHomeProgress(targetMode);
        }
    }

    private void OnDisable()
    {
        // Hủy đăng ký
        HomeManager.OnUpdateProgress -= HandleProgressUpdate;
    }

    private void HandleProgressUpdate(GamePlayMode mode, float progress)
    {
        // Chỉ nhận đúng Mode của mình
        if (mode == this.targetMode && sliderUI != null)
        {
            sliderUI.value = progress;
        }
    }
}