using System;
using UnityEngine;

public class HomeManager : Singleton<HomeManager>
{
    public SaveData data;

    // ================= LEVEL =================
    // 1. Biến lưu Level cao nhất đã mở khóa (Dùng để lưu xuống đĩa)
    private int _maxLevelUnlocked;
    public event Action<int> OnLevelChanged; // Sự kiện update Map UI ở Home

    public int MaxLevelUnlocked
    {
        get => _maxLevelUnlocked;
        private set // Chỉ cho phép đổi trong script này
        {
            if (_maxLevelUnlocked != value)
            {
                _maxLevelUnlocked = value;
                OnLevelChanged?.Invoke(_maxLevelUnlocked);

                // [QUAN TRỌNG] Cập nhật Data Save ngay khi biến này thay đổi
                if (data != null) data.progress.currentLevelIndex = _maxLevelUnlocked;
            }
        }
    }

    // 2. [MỚI] Biến lưu Level đang chơi hiện tại (Session - không lưu xuống đĩa)
    // Biến này giúp phân biệt việc đang "Cày ải" hay "Chơi lại"
    private int _currentPlayingLevel;
    public int CurrentPlayingLevel => _currentPlayingLevel;

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

        this.MaxLevelUnlocked = data.progress.currentLevelIndex;
        this._currentPlayingLevel = this.MaxLevelUnlocked;

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
        var levelData = ReadJson.Instance.GetLevelData(this.MaxLevelUnlocked - 1);

        string logContent = JsonUtility.ToJson(levelData, true);

        Debug.Log($"Data Level {this.MaxLevelUnlocked}:\n" + logContent);
    }

    public void StartGameAtLevel(int levelIndex)
    {
        // 1. Lưu lại level đang chơi vào biến tạm
        this._currentPlayingLevel = levelIndex;

        // 2. Chuyển cảnh UI
        UiManager.Instance.SceneGamePlay();

        // 3. Bảo GridManager tạo map
        // (Lấy data dựa trên levelIndex truyền vào)
        var levelData = ReadJson.Instance.GetLevelData(this._currentPlayingLevel - 1);
        GridManager.Instance.GenerateGrid(levelData);
    }

    /// <summary>
    /// Hàm gọi khi người chơi THẮNG game
    /// </summary>
    public void OnLevelWin()
    {
        // [QUAN TRỌNG] Logic kiểm tra xem có được cộng Save hay không

        // Chỉ cộng Save khi: Level vừa thắng == Level cao nhất hiện có
        // (Tức là đang phá đảo, chứ không phải đang chơi lại bài cũ)
        if (this._currentPlayingLevel == this._maxLevelUnlocked)
        {
            // Setter sẽ tự động cập nhật Data.progress và bắn Event
            this.MaxLevelUnlocked++;
        }
        else
        {
            Debug.Log("Đang Replay level cũ, không cộng Save Data.");
        }
    }

    /// Hàm cho nút NEXT
    public void PlayNextLevel()
    {
        // Chơi level tiếp theo của level VỪA THẮNG
        StartGameAtLevel(_currentPlayingLevel + 1);
    }

    /// Hàm cho nút REPLAY
    public void ReplayCurrentLevel()
    {
        // Chơi lại đúng cái level VỪA THẮNG
        StartGameAtLevel(this._currentPlayingLevel);
    }

    /// Hàm cho nút PLAY ở màn hình HOME
    public void PlayMaxLevel()
    {
        // Ở Home thì luôn chơi level cao nhất
        StartGameAtLevel(this._maxLevelUnlocked);
    }
}