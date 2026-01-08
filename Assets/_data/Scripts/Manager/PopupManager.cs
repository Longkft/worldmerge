using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    // --- QUEUE SYSTEM ---
    private Queue<Action> _popupQueue = new Queue<Action>();
    private bool _isShowing = false; // Đang có popup nào hiện không?

    // --- POPUP REFERENCES ---
    private popupSetting _popupSetting = null;
    private popupEndGame _popupEndGame = null;

    // Hàm Lazy Load Setting
    public popupSetting GetPopupSetting()
    {
        if (this._popupSetting == null)
        {
            // Kiểm tra an toàn PrefabManager
            if (PrefabManager.Instance != null && PrefabManager.Instance.popupSetting != null)
            {
                this._popupSetting = Instantiate(PrefabManager.Instance.popupSetting, this.transform).GetComponent<popupSetting>();
                this._popupSetting.gameObject.SetActive(false); // Sinh ra thì ẩn đi đã
            }
            else
            {
                Debug.LogError("PrefabManager chưa sẵn sàng hoặc chưa kéo Prefab popupSetting!");
                return null;
            }
        }
        return this._popupSetting;
    }
    public popupEndGame GetPopupEndGame()
    {
        if (this._popupEndGame == null)
        {
            if (PrefabManager.Instance != null && PrefabManager.Instance.popupEndGame != null)
            {
                this._popupEndGame = Instantiate(PrefabManager.Instance.popupEndGame, this.transform).GetComponent<popupEndGame>();
                this._popupEndGame.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("PrefabManager chưa sẵn sàng hoặc chưa kéo Prefab popupEndGame!");
                return null;
            }
        }
        return this._popupEndGame;
    }

    // --- CORE QUEUE LOGIC ---

    // 1. Thêm lệnh Show vào hàng đợi
    private void AddToQueue(Action showAction)
    {
        _popupQueue.Enqueue(showAction);
        this.CheckQueue(); // Kiểm tra xem có được hiện luôn không
    }

    // 2. Kiểm tra hàng đợi và hiện popup tiếp theo
    private void CheckQueue()
    {
        Debug.Log("_isShowing: " + _isShowing + "_popupQueue.Count: " + _popupQueue.Count);
        // Nếu đang có thằng diễn HOẶC hàng đợi rỗng -> Thôi
        if (_isShowing || _popupQueue.Count == 0) return;

        // Lấy lệnh tiếp theo ra
        Action nextPopupAction = _popupQueue.Dequeue();

        _isShowing = true; // Khóa sân khấu
        nextPopupAction?.Invoke();
    }

    // 3. Callback khi một popup tắt hẳn
    private void OnPopupClosed()
    {
        _isShowing = false; // Mở sân khấu
        this.CheckQueue(); // Mời thằng tiếp theo
    }

    // --- PUBLIC FUNCTIONS ---
    // --- SHOW POPUP ---
    public void ShowPopupSetting()
    {
        // Gói việc hiện popup lại thành 1 Action và ném vào Queue
        this.AddToQueue(() =>
        {
            var popup = this.GetPopupSetting();
            if (popup != null)
            {
                // Truyền hàm OnPopupClosed vào để khi nào tắt nó báo lại
                popup.Show(this.OnPopupClosed);
            }
            else
            {
                // Nếu lỗi không tạo được popup -> Báo đóng luôn để không kẹt Queue
                this.OnPopupClosed();
            }
        });
    }

    public void ShowPopupEndGame()
    {
        // Gói việc hiện popup lại thành 1 Action và ném vào Queue
        this.AddToQueue(() =>
        {
            var popup = this.GetPopupEndGame();
            if (popup != null)
            {
                // Truyền hàm OnPopupClosed vào để khi nào tắt nó báo lại
                popup.Show(this.OnPopupClosed);
            }
            else
            {
                // Nếu lỗi không tạo được popup -> Báo đóng luôn để không kẹt Queue
                this.OnPopupClosed();
            }
        });
    }

    // --- HIDE POPUP ---
    public void HidePopupSetting()
    {
        var popup = this.GetPopupSetting();
        if (popup != null)
        {
            popup.Hide();
        }
    }
}