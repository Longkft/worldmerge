using UnityEngine;
using TMPro;
using DG.Tweening;

public class popupEndGame : BasePopup
{
    [Header("fx")]
    [SerializeField] FxView _fx;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI levelUI;

    // --- SAFETY FIRST ---
    private void OnDisable()
    {
        /*DOTween.Kill(this.transform);*/
    }

    public override void  Show(System.Action onCloseCallback)
    {
        DOTween.Kill(this.transform); // Dọn dẹp

        gameObject.SetActive(true);
        base.Show(onCloseCallback);

        UiManager.Instance.gameWord.SetActive(false);
        HomeManager.Instance.OnLevelWin();
        levelUI.text = "LEVEL " + HomeManager.Instance.CurrentPlayingLevel.ToString();

        if (_fx != null)
        {
            _fx.ShowFx(async () => {
                UiManager.Instance.ActiceEffEndGame();

                await Utils.AwaitTime(0.1f, this.destroyCancellationToken);
                // play âm thanh
                AudioManager.Instance.PlaySFX(SoundType.Win);
            });
        }
    }

    public override void Hide()
    {
        if (UiManager.Instance.effEndGame)
            UiManager.Instance.effEndGame.SetActive(false);

        // Chỉ chạy animation nếu object còn sống
        if (_fx != null && this.gameObject.activeInHierarchy)
        {
            _fx.FXBoxHide(() => // Hàm HideFX của FxView đã bao gồm Shadow và Box rồi
            {
                this.gameObject.SetActive(false);

                // Animation xong -> Tự tắt active bên FxView -> Gọi callback
                _onCloseCallback?.Invoke();
            });
        }
        else
        {
            gameObject.SetActive(false);
            _onCloseCallback?.Invoke();
        }
    }

    // =======================================================
    // LOGIC NÚT BẤM (FIXED)
    // =======================================================

    public void btnReplayGame()
    {
        // 1. Lưu callback gốc
        System.Action originalCallback = this._onCloseCallback;

        // 2. Gài logic: Xong animation -> Reset Popup -> Chạy Game
        this._onCloseCallback = () =>
        {
            originalCallback?.Invoke();
            HomeManager.Instance.ReplayCurrentLevel();
        };

        // 3. Chạy Animation
        this.Hide();
    }

    public void btnNextGame()
    {
        // 1. Lưu callback gốc
        System.Action originalCallback = this._onCloseCallback;

        // 2. Gài logic
        this._onCloseCallback = () =>
        {
            originalCallback?.Invoke();
            HomeManager.Instance.PlayNextLevel();
        };

        // 3. Chạy Animation
        this.Hide();
    }
}