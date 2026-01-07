using UnityEngine;
using UnityEngine.EventSystems;

public class BtnSetting : BaseTouch
{
    protected override void OnTouchStart(PointerEventData data)
    {
        PopupManager.Instance.ShowPopupSetting();
    }
}
