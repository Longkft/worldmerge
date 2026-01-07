using System;
using UnityEngine;

public class HomeManager : Singleton<HomeManager>
{
    public SaveData data;

    // ================= LEVEL =================
    private int _levelCurrent;
    public event Action<int> OnLevelChanged;
    public int LevelCurrent
    {
        get => _levelCurrent;
        set
        {
            if (_levelCurrent != value)
            {
                _levelCurrent = value;
                OnLevelChanged?.Invoke(_levelCurrent);

                // Cập nhật RAM (nếu cần thiết để save progress)
                if (data != null) data.progress.currentLevelIndex = _levelCurrent;
            }
        }
    }

    // ================= HINT =================
    private int _hintCount;
    public event Action<int> OnHintChanged;
    public int HintCount
    {
        get => _hintCount;
        set
        {
            if (_hintCount != value)
            {
                _hintCount = value;
                OnHintChanged?.Invoke(_hintCount);
                // Cập nhật RAM
                if (data != null) data.progress.hints = _hintCount;
            }
        }
    }

    // ================= SEARCH =================
    private int _searchCount;
    public event Action<int> OnSearchChanged;
    public int SearchCount
    {
        get => _searchCount;
        set
        {
            if (_searchCount != value)
            {
                _searchCount = value;
                OnSearchChanged?.Invoke(_searchCount);
                // Cập nhật RAM
                if (data != null) data.progress.searchs = _searchCount;
            }
        }
    }

    // ================= SETTING: MUSIC =================
    private bool _isMusicOn;
    public event Action<bool> OnMusicChanged;

    public bool IsMusicOn
    {
        get => _isMusicOn;
        set
        {
            if (_isMusicOn != value)
            {
                _isMusicOn = value;

                // 1. CHỈ CẬP NHẬT BIẾN TRONG RAM (Không gọi Save Disk)
                if (data != null) data.settings.isMusicOn = _isMusicOn;

                // 2. Logic game
                AudioManager.Instance.SetMusicState(_isMusicOn);
                OnMusicChanged?.Invoke(_isMusicOn);
            }
        }
    }

    // ================= SETTING: SOUND =================
    private bool _isSoundOn;
    public event Action<bool> OnSoundChanged;

    public bool IsSoundOn
    {
        get => _isSoundOn;
        set
        {
            if (_isSoundOn != value)
            {
                _isSoundOn = value;

                // 1. Cập nhật RAM
                if (data != null) data.settings.isSfxOn = _isSoundOn;

                // 2. Logic game
                AudioManager.Instance.SetSFXState(_isSoundOn);
                OnSoundChanged?.Invoke(_isSoundOn);
            }
        }
    }

    // ================= SETTING: VIBRATION =================
    private bool _isVibrationOn;
    public event Action<bool> OnVibrationChanged;

    public bool IsVibrationOn
    {
        get => _isVibrationOn;
        set
        {
            if (_isVibrationOn != value)
            {
                _isVibrationOn = value;

                // 1. Cập nhật RAM
                if (data != null) data.settings.vibration = _isVibrationOn;

                // 2. Logic game
                OnVibrationChanged?.Invoke(_isVibrationOn);
            }
        }
    }

    // ================= AWAKE & INIT =================
    private async void Awake()
    {
        base.Awake();

        // Load data lên RAM 1 lần duy nhất ở đây
        this.data = await DataManager.Instance.GetDataAsync();

        this.LevelCurrent = data.progress.currentLevelIndex;
        this.HintCount = data.progress.hints;
        this.SearchCount = data.progress.searchs;

        _isMusicOn = data.settings.isMusicOn;
        _isSoundOn = data.settings.isSfxOn;
        _isVibrationOn = data.settings.vibration;

        AudioManager.Instance.SetMusicState(_isMusicOn);
        AudioManager.Instance.SetSFXState(_isSoundOn);
        AudioManager.Instance.PlayMusic(SoundType.BGM_Home);

        this.LoadDataLevelIndex();
        UiManager.Instance.SceneHome();
    }

    protected void LoadDataLevelIndex()
    {
        var levelData = ReadJson.Instance.GetLevelData(this.LevelCurrent - 1);

        string logContent = JsonUtility.ToJson(levelData, true);

        Debug.Log($"Data Level {this.LevelCurrent}:\n" + logContent);
    }

    public void NextLevel()
    {
        this.LevelCurrent++;
    }
}