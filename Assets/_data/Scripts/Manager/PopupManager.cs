using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    [Header("References")]
    // Kéo cái Canvas to nhất (hoặc Canvas chứa popup) vào đây
    [SerializeField] private Canvas _mainCanvas;

    // --- QUEUE SYSTEM ---
    private Queue<Action> _popupQueue = new Queue<Action>();
    private bool _isShowing = false; // Đang có popup nào hiện không?

    // --- POPUP REFERENCES ---
    private popupSetting _popupSetting = null;
    private popupEndGame _popupEndGame = null;
    private popupTutorial _popupTutorial = null;

    // --- THEO DÕI POPUP ĐANG HIỆN ---
    private BasePopup _currentActivePopup = null;

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

    public popupTutorial GetPopupTutorial()
    {
        if (this._popupTutorial == null)
        {
            if (PrefabManager.Instance != null && PrefabManager.Instance.popupTutorial != null)
            {
                this._popupTutorial = Instantiate(PrefabManager.Instance.popupTutorial, this.transform).GetComponent<popupTutorial>();

                this._popupTutorial.SetCanvas(_mainCanvas);

                this._popupTutorial.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("PrefabManager chưa sẵn sàng hoặc chưa kéo Prefab popupEndGame!");
                return null;
            }
        }
        return this._popupTutorial;
    }

    // --- CORE QUEUE LOGIC ---

    // 1. Thêm lệnh Show vào hàng đợi
    private void AddToQueue(Action showAction)
    {
        this._popupQueue.Enqueue(showAction);
        this.CheckQueue(); // Kiểm tra xem có được hiện luôn không
    }

    // 2. Kiểm tra hàng đợi và hiện popup tiếp theo
    private void CheckQueue()
    {
        Debug.Log("_isShowing: " + _isShowing + "_popupQueue.Count: " + _popupQueue.Count);
        // Nếu đang có thằng diễn HOẶC hàng đợi rỗng -> Thôi
        if (this._isShowing || this._popupQueue.Count == 0) return;

        // Lấy lệnh tiếp theo ra
        Action nextPopupAction = this._popupQueue.Dequeue();

        this._isShowing = true; // Khóa sân khấu
        nextPopupAction?.Invoke();
    }

    // 3. Callback khi một popup tắt hẳn
    private void OnPopupClosed()
    {
        this._isShowing = false; // Mở sân khấu
        this._currentActivePopup = null;
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
                this._currentActivePopup = popup;

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
                this._currentActivePopup = popup;

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

    public void ShowPopupTutorial(string title, string content, float worldY)
    {
        // Gói việc hiện popup lại thành 1 Action và ném vào Queue
        this.AddToQueue(() =>
        {
            var popup = this.GetPopupTutorial();
            if (popup != null)
            {
                this._currentActivePopup = popup;

                // Truyền hàm OnPopupClosed vào để khi nào tắt nó báo lại
                popup.ShowTutorial(this.OnPopupClosed, title, content, worldY);
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

    // --- DÙNG CHO NÚT HOME ---
    public void GoHomeWithGracefulExit()
    {
        // 1. Xóa sạch hàng đợi
        this._popupQueue.Clear();
        this._isShowing = false;

        // 2. Hành động cuối cùng: Chuyển cảnh
        System.Action goHomeAction = () =>
        {
            this._currentActivePopup = null; // Reset cho chắc
            
            // Gọi qua HomeManager để nó set lại biến CurrentMode = Home
            HomeManager.Instance.ReturnToHome();
        };

        // 3. Kiểm tra xem có popup nào đang hiện không
        if (this._currentActivePopup != null && this._currentActivePopup.gameObject.activeSelf)
        {
            // Gọi hàm chung ở BasePopup
            // Dù nó là EndGame hay Setting, nó đều hiểu hàm này
            this._currentActivePopup.GracefulClose(goHomeAction);
        }
        else
        {
            // Không có popup nào -> Về Home luôn
            goHomeAction.Invoke();
        }
    }
}