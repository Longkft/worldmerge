using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class BtnSound : BaseTouch
{
    public enum SettingType { Music, Sound, Vibration }

    [Header("Config")]
    [SerializeField] private SettingType type;
    [SerializeField] private float animDuration = 0.2f;

    [Header("UI References")]
    [SerializeField] private GameObject bgOn;
    [SerializeField] private GameObject bgOff;
    [SerializeField] private TextMeshProUGUI txtStatus;
    [SerializeField] private RectTransform iconRect;
    [SerializeField] private RectTransform textRect;

    // Biến lưu tọa độ CỐ ĐỊNH (Không phụ thuộc vào việc bạn để Icon ở đâu lúc đầu)
    private float _xRight; // Tọa độ bên phải (ON của Icon)
    private float _xLeft;  // Tọa độ bên trái (OFF của Icon)

    private bool _isInitialized = false;

    private void Awake()
    {
        InitializePositions();
    }

    private void InitializePositions()
    {
        if (_isInitialized) return;

        float x1 = iconRect.anchoredPosition.x;
        float x2 = textRect.anchoredPosition.x;

        // Tìm ra đâu là bên Phải (thường là ON), đâu là bên Trái (thường là OFF)
        _xRight = Mathf.Max(x1, x2);
        _xLeft = Mathf.Min(x1, x2);

        _isInitialized = true;
    }

    private void OnEnable()
    {
        // Khi bật Popup lên: Kill tween cũ
        if (iconRect) iconRect.DOKill();
        if (textRect) textRect.DOKill();

        this.Setup();
    }

    public void Setup()
    {
        this.InitializePositions(); // Đảm bảo đã có tọa độ

        bool isOn = false;
        switch (type)
        {
            case SettingType.Music: isOn = HomeManager.Instance.IsMusicOn; break;
            case SettingType.Sound: isOn = HomeManager.Instance.IsSoundOn; break;
            case SettingType.Vibration: isOn = HomeManager.Instance.IsVibrationOn; break;
        }

        // Setup thì không chạy hiệu ứng (false)
        this.UpdateUI(isOn, false);
    }

    protected override void OnTouchStart(PointerEventData data)
    {
        AudioManager.Instance.PlaySFX(SoundType.Click_Button);

        bool newState = false;
        switch (type)
        {
            case SettingType.Music:
                newState = !HomeManager.Instance.IsMusicOn;
                HomeManager.Instance.IsMusicOn = newState;
                break;
            case SettingType.Sound:
                newState = !HomeManager.Instance.IsSoundOn;
                HomeManager.Instance.IsSoundOn = newState;
                break;
            case SettingType.Vibration:
                newState = !HomeManager.Instance.IsVibrationOn;
                HomeManager.Instance.IsVibrationOn = newState;
                break;
        }

        // Click thì có hiệu ứng (true)
        this.UpdateUI(newState, true);
    }

    private void UpdateUI(bool isOn, bool animate)
    {
        if (bgOn) bgOn.SetActive(isOn);
        if (bgOff) bgOff.SetActive(!isOn);
        if (txtStatus) txtStatus.text = isOn ? "On" : "Off";

        // ON: Icon nằm bên TRÁI (_xLeft), Text nằm bên PHẢI (_xRight)
        // OFF: Icon nằm bên PHẢI (_xRight), Text nằm bên TRÁI (_xLeft)

        float targetX_Icon = isOn ? _xLeft : _xRight;
        float targetX_Text = isOn ? _xRight : _xLeft;

        if (animate)
        {
            if (iconRect)
            {
                iconRect.DOKill();
                iconRect.DOAnchorPosX(targetX_Icon, animDuration).SetEase(Ease.OutBack);
            }
            if (textRect)
            {
                textRect.DOKill();
                textRect.DOAnchorPosX(targetX_Text, animDuration).SetEase(Ease.OutBack);
            }
        }
        else
        {
            if (iconRect) iconRect.anchoredPosition = new Vector2(targetX_Icon, iconRect.anchoredPosition.y);
            if (textRect) textRect.anchoredPosition = new Vector2(targetX_Text, textRect.anchoredPosition.y);
        }
    }
}