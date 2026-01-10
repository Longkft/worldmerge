using UnityEngine;
using UnityEngine.EventSystems;

public class BtnHome : BaseTouch
{
    protected override void OnTouchStart(PointerEventData data)
    {
        Debug.Log("Btn Home Clicked");

        // Tìm thằng đang hiện để tắt
        if (PopupManager.Instance != null)
        {
            PopupManager.Instance.GoHomeWithGracefulExit();
        }
        else
        {
            // Gọi về HomeManager để set Mode
            HomeManager.Instance.ReturnToHome();
        }
    }
}