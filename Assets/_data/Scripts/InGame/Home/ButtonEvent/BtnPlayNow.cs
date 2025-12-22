using UnityEngine;
using UnityEngine.EventSystems;

public class BtnPlayNow : BaseTouch
{
    protected override void OnTouchStart(PointerEventData data)
    {
        UiManager.Instance.SceneGamePlay();
        GridManager.Instance.GenerateGrid(ReadJson.Instance.GetLevelData(HomeManager.Instance.levelCurrent - 1));
    }
}
