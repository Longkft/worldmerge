using UnityEngine;
using UnityEngine.EventSystems;

public class BtnPlayNow : BaseTouch
{
    protected override void OnTouchStart(PointerEventData data)
    {
        UiManager.Instance.SceneGamePlay();
        HomeManager.Instance.PlayMaxLevel();
    }
}
