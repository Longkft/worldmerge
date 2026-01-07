using UnityEngine;

public class ToolController : MonoBehaviour
{
    [SerializeReference] ToolUI _toolUI;

    private void OnEnable()
    {
        // Đăng ký lắng nghe sự kiện thay đổi Level
        // Hàm SetUiLevelWord giờ nhận vào 1 số int (level mới)
        if (HomeManager.Instance != null)
        {
            HomeManager.Instance.OnHintChanged += SetUiToolHint;
            SetUiToolHint(HomeManager.Instance.HintCount);

            HomeManager.Instance.OnSearchChanged += SetUiToolSearch;
            SetUiToolSearch(HomeManager.Instance.SearchCount);
        }
    }

    private void OnDisable()
    {
        if (HomeManager.Instance != null)
        {
            HomeManager.Instance.OnHintChanged -= SetUiToolHint;
            HomeManager.Instance.OnSearchChanged -= SetUiToolSearch;
        }
    }

    public void SetUiToolHint(int numberHint)
    {
        _toolUI.setValueHint(numberHint);
    }

    public void SetUiToolSearch(int numberSearch)
    {
        _toolUI.setValueSearch(numberSearch);
    }
}
