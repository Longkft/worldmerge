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
    [SerializeField] private Ease boxEase = Ease.OutBack; // Scale dùng OutBack sẽ nảy đẹp hơn OutCubic

    private void OnEnable()
    {
        ResetToStart();
    }

    private void ResetToStart()
    {
        // Kill hết tween cũ
        if (shadowGroup) shadowGroup.DOKill();
        if (boxRect) boxRect.DOKill();

        // 1. Shadow bắt đầu từ mờ
        if (shadowGroup) shadowGroup.alpha = 0f;

        // 2. [THAY ĐỔI] Box bắt đầu từ bé xíu (Scale = 0)
        if (boxRect)
        {
            boxRect.localScale = Vector3.zero;
            boxRect.anchoredPosition = Vector2.zero; // Đảm bảo nó nằm giữa màn hình
        }
    }

    public void ShowFx(Action onComplete = null)
    {
        this.gameObject.SetActive(true);
        ResetToStart();

        // Hiện Shadow trước -> Rồi Box phình to ra
        this.FXShadow(() => {
            this.FXBox(() => {
                onComplete?.Invoke();
            });
        });
    }

    public void FXShadow(Action onComplete = null)
    {
        if (shadowGroup)
        {
            shadowGroup.DOKill();
            shadowGroup.DOFade(1f, duration)
                .SetUpdate(true) // Chạy kể cả khi Time.timeScale = 0
                .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    // --- [THAY ĐỔI] TWEEN SCALE ---
    public void FXBox(Action onComplete = null)
    {
        if (boxRect)
        {
            boxRect.DOKill();
            // Phóng to từ 0 lên 1
            boxRect.DOScale(Vector3.one, duration)
                .SetEase(boxEase)
                .SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    // --- HIDE ---

    public void HideFX(Action onComplete = null)
    {
        // Thu nhỏ Box -> Ẩn Shadow -> Tắt Object
        this.FXBoxHide(() =>
        {
            this.FXShadowHide(() => {
                onComplete?.Invoke();
            });
        });
    }

    // --- [THAY ĐỔI] TWEEN SCALE HIDE ---
    public void FXBoxHide(Action onComplete = null)
    {
        if (boxRect && this.gameObject.activeInHierarchy)
        {
            boxRect.DOKill();
            // Thu nhỏ từ hiện tại về 0
            boxRect.DOScale(Vector3.zero, duration)
                .SetEase(Ease.InBack) // Lúc tắt dùng InBack sẽ đẹp hơn (hơi thụt vào rồi biến mất)
                .SetUpdate(true)
                .OnComplete(() => onComplete?.Invoke());
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public void FXShadowHide(Action onComplete = null)
    {
        if (shadowGroup && this.gameObject.activeInHierarchy)
        {
            shadowGroup.DOKill();
            shadowGroup.DOFade(0, duration)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                    if (this != null && this.gameObject != null)
                        this.gameObject.SetActive(false);
                });
        }
        else
        {
            onComplete?.Invoke();
            if (this != null && this.gameObject != null)
                this.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        this.transform.DOKill();
        if (shadowGroup) shadowGroup.DOKill();
        if (boxRect) boxRect.DOKill();
    }
}