using UnityEngine;

public class PanelGameplayController : Singleton<PanelGameplayController>
{
    [SerializeReference] PanelGameplayUI _panelGameplayUI;
    [SerializeReference] ToolController _tool;

    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện thay đổi Level
        // Hàm SetUiLevelWord giờ nhận vào 1 số int (level mới)
        if (HomeManager.Instance != null)
        {
            /*HomeManager.Instance.OnLevelChanged += SetUiLevel;*/

            // Lấy level đang thực sự chơi (Session)
            SetUiLevel(HomeManager.Instance.CurrentPlayingLevel);
        }

        if (GridManager.Instance != null)
        {
            GridManager.Instance.OnNumberRowsChanged += setGroundEnd;

            // Cập nhật ngay lập tức trạng thái hiện tại (phòng khi UI bật sau khi data đã load)
            setGroundEnd(GridManager.Instance.NumberRowsCompleted);
        }
    }

    private void OnDisable()
    {
        /*if (HomeManager.Instance != null)
        {
            HomeManager.Instance.OnLevelChanged -= SetUiLevel;
        }*/

        if (GridManager.Instance != null)
        {
            GridManager.Instance.OnNumberRowsChanged -= setGroundEnd;


        }
    }

    // Sửa hàm này nhận tham số int để khớp với Action<int>
    public void SetUiLevel(int newLevel)
    {
        _panelGameplayUI.setLevelUI(newLevel);
    }

    public void setGroundEnd(int numberGround)
    {
        _panelGameplayUI.setGroundEnd(numberGround);
    }
}
