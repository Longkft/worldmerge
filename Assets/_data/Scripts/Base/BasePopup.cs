using UnityEngine;
using System;

public abstract class BasePopup : MonoBehaviour
{
    // Callback này sẽ báo cho PopupManager biết: "Tao tắt rồi, cho thằng sau lên đi"
    protected Action _onCloseCallback;

    // Hàm Show chuẩn cho Queue
    public virtual void Show(Action onCloseCallback)
    {
        this._onCloseCallback = onCloseCallback;
        // Logic hiện visual sẽ viết ở lớp con
    }

    // Hàm Hide chuẩn
    public abstract void Hide();
}