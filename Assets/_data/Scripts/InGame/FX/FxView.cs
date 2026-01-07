using UnityEngine;
using DG.Tweening;
using System;

public class FxView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup shadowGroup;
    [SerializeField] private RectTransform boxRect;

    [Header("Settings")]
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private Ease boxEase = Ease.OutCubic;

    private void OnEnable()
    {
        ResetToStart();
    }

    private void ResetToStart()
    {
        if (shadowGroup) shadowGroup.alpha = 125f / 255f;
        if (boxRect) boxRect.anchoredPosition = new Vector2(-1000, 0);
    }

    public void ShowFx(Action onComplete = null)
    {
        this.FXShadow(()=> {
            this.FXBox(()=> {
                onComplete?.Invoke();
            });
        });
    }

    // "= null" nghĩa là tham số này không bắt buộc, không truyền cũng không lỗi
    public void FXShadow(Action onComplete = null)
    {
        this.gameObject.SetActive(true);
        if (shadowGroup)
        {
            shadowGroup.DOKill();
            shadowGroup.DOFade(1f, duration)
                .OnComplete(() =>
                {
                    // Chạy xong tween thì kích hoạt callback (nếu có)
                    onComplete?.Invoke();
                });
        }
        else
        {
            // Nếu không có Shadow để chạy, gọi callback luôn để tránh kẹt logic game
            onComplete?.Invoke();
        }
    }

    public void FXBox(Action onComplete = null)
    {
        if (boxRect)
        {
            boxRect.DOKill();
            boxRect.DOAnchorPos(Vector2.zero, duration)
                .SetEase(boxEase)
                .OnComplete(() =>
                {
                    // Chạy xong tween thì kích hoạt callback (nếu có)
                    onComplete?.Invoke();
                });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public void FXShadowHide(Action onComplete = null)
    {
        if (shadowGroup)
        {
            shadowGroup.DOKill();
            shadowGroup.DOFade(0, duration)
                .OnComplete(() =>
                {
                    // Chạy xong tween thì kích hoạt callback (nếu có)
                    onComplete?.Invoke();

                    this.gameObject.SetActive(false);
                });
        }
        else
        {
            // Nếu không có Shadow để chạy, gọi callback luôn để tránh kẹt logic game
            onComplete?.Invoke();
        }
    }

    public void FXBoxHide(Action onComplete = null)
    {
        if (boxRect)
        {
            boxRect.DOKill();
            boxRect.DOAnchorPos(new Vector2(1000,0), duration)
                .SetEase(boxEase)
                .OnComplete(() =>
                {
                    // Chạy xong tween thì kích hoạt callback (nếu có)
                    onComplete?.Invoke();
                });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public void HideFX(Action onComplete = null)
    {
        this.FXBoxHide(() =>
        {
            this.FXShadowHide(()=> {
                onComplete?.Invoke();
            });
        });
    }

    private void OnDisable()
    {
        if (shadowGroup) shadowGroup.DOKill();
        if (boxRect) boxRect.DOKill();
    }
}