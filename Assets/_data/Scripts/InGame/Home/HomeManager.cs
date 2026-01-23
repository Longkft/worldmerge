using System;
using UnityEngine;

public enum GameMode
{
    Home,
    GamePlay
}

public class HomeManager : Singleton<HomeManager>
{
    public SaveData data;

    // Trạng thái màn hình hiện tại (Home hoặc đang chơi)
    public GameMode CurrentMode { get; private set; } = GameMode.Home; // lưu game mode

    // Chế độ chơi được chọn (Word / Math)
    public GamePlayMode SelectedGamePlayMode { get; private set; } = GamePlayMode.Word;

    // ================= LEVEL =================
    // 1. Biến lưu Level cao nhất đã mở khóa (Dùng để lưu xuống đĩa)
    // ================= EVENTS =================
    // Sự kiện riêng lẻ để cập nhật text trên 2 nút
    public event Action<int> OnWordLevelChanged;
    public event Action<int> OnMathLevelChanged;

    // ================= PROPERTIES =================
    // Helper lấy level hiện tại để hiển thị lên UI
    public int WordLevelIndex => data != null ? data.progress.wordLevelIndex : 1;
    public int MathLevelIndex => data != null ? data.progress.mathLevelIndex : 1;

    // Level hiện tại của chế độ ĐANG CHỌN (Dùng cho logic thắng thua)
    public int MaxLevelUnlocked
    {
        get
        {
            if (data == null) return 1;
            return (SelectedGamePlayMode == GamePlayMode.Math) ? data.progress.mathLevelIndex : data.progress.wordLevelIndex;
        }
        private set
        {
            if (data != null)
            {
                // 1. Lưu vào RAM biến tương ứng
                if (SelectedGamePlayMode == GamePlayMode.Math)
                {
                    data.progress.mathLevelIndex = value;
                    OnMathLevelChanged?.Invoke(value); // Báo cho nút Math cập nhật text
                }
                else
                {
                    data.progress.wordLevelIndex = value;
                    OnWordLevelChanged?.Invoke(value); // Báo cho nút Word cập nhật text
                }
            }
        }
    }

    // 2. Biến lưu Level đang chơi hiện tại (Session - không lưu xuống đĩa)

    // [THÊM] Sự kiện này dành riêng cho Gameplay Panel (Cập nhật tiêu đề màn chơi)
    public event Action<int> OnPlayingLevelChanged;

    // Biến này giúp phân biệt việc đang "Cày ải" hay "Chơi lại"
    // Public cho bên ngoài đọc, Private cho bên trong sửa
    public int CurrentPlayingLevel { get; private set; }

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
    /*private async void Awake()
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

        this.CurrentMode = GameMode.Home; // Mặc định khi mở game lên là ở Home
        UiManager.Instance.SceneHome();
    }*/

    private async void Awake()
    {
        base.Awake();

        // 1. Load Data
        this.data = await DataManager.Instance.GetDataAsync();

        this.HintCount = data.progress.hints;
        this.SearchCount = data.progress.searchs;

        _isMusicOn = data.settings.isMusicOn;
        _isSoundOn = data.settings.isSfxOn;
        _isVibrationOn = data.settings.vibration;

        // 2. Init Audio
        AudioManager.Instance.SetMusicState(data.settings.isMusicOn);
        AudioManager.Instance.SetSFXState(data.settings.isSfxOn);
        AudioManager.Instance.PlayMusic(SoundType.BGM_Home);

        // 3. Init UI
        this.CurrentMode = GameMode.Home;
        UiManager.Instance.SceneHome();

        // 4. Update UI Text lần đầu tiên cho cả 2 nút
        OnWordLevelChanged?.Invoke(this.WordLevelIndex);
        OnMathLevelChanged?.Invoke(this.MathLevelIndex);
    }

    // Hàm gọi khi bấm nút "Word Mode"
    public void PlayWordMode()
    {
        this.SelectedGamePlayMode = GamePlayMode.Word;
        Debug.Log("Selected: WORD MODE");

        // Chơi luôn level cao nhất của Word
        StartGameAtLevel(this.WordLevelIndex);
    }

    // Hàm gọi khi bấm nút "Math Mode"
    public void PlayMathMode()
    {
        this.SelectedGamePlayMode = GamePlayMode.Math;
        Debug.Log("Selected: MATH MODE");

        // Chơi luôn level cao nhất của Math
        StartGameAtLevel(this.MathLevelIndex);
    }

    protected void LoadDataLevelIndex()
    {
        var levelData = ReadJson.Instance.GetLevelData(this.MaxLevelUnlocked - 1);

        string logContent = JsonUtility.ToJson(levelData, true);

        Debug.Log($"Data Level {this.MaxLevelUnlocked}:\n" + logContent);
    }

    public void StartGameAtLevel(int levelIndex)
    {
        // Đang màn game
        this.CurrentMode = GameMode.GamePlay;

        // Lưu lại level đang chơi vào biến tạm
        this.CurrentPlayingLevel = levelIndex;

        // Chuyển cảnh UI
        UiManager.Instance.SceneGamePlay();

        // [THÊM] Bắn sự kiện báo cho PanelGameplay biết là đang chơi level mấy
        // Để dù là Replay hay Next thì UI cũng tự update theo số này
        OnPlayingLevelChanged?.Invoke(this.CurrentPlayingLevel);

        // Bảo GridManager tạo map
        // Lấy data từ DataManager dựa trên chế độ đã chọn
        var levelData = DataManager.Instance.GetLevelData(levelIndex, this.SelectedGamePlayMode);

        if (levelData != null)
        {
            GridManager.Instance.GenerateGrid(levelData);
        }
        else
        {
            Debug.LogError($"Lỗi: Không tìm thấy data cho {SelectedGamePlayMode} - Level {levelIndex}");
        }
    }

    /// <summary>
    /// Hàm gọi khi người chơi THẮNG game
    /// </summary>
    public void OnLevelWin()
    {
        // Xóa save game chơi dở đi(vì đã thắng rồi)
        // Chỉ xóa save của chế độ hiện tại
        DataManager.Instance.ClearMatchProgress(this.SelectedGamePlayMode);

        // Logic kiểm tra xem có được cộng Save hay không

        // Chỉ cộng Save khi: Level vừa thắng == Level cao nhất hiện có
        // (Tức là đang phá đảo, chứ không phải đang chơi lại bài cũ)
        if (this.CurrentPlayingLevel == this.MaxLevelUnlocked)
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
        StartGameAtLevel(CurrentPlayingLevel + 1);
    }

    /// Hàm cho nút REPLAY
    public void ReplayCurrentLevel()
    {
        // Chơi lại đúng cái level VỪA THẮNG
        StartGameAtLevel(this.CurrentPlayingLevel);
    }

    /// Hàm cho nút PLAY ở màn hình HOME
    public void PlayMaxLevel()
    {
        // Ở Home thì luôn chơi level cao nhất
        StartGameAtLevel(this.MaxLevelUnlocked);
    }

    public void ReturnToHome()
    {
        if (this.CurrentMode == GameMode.GamePlay && GridManager.Instance != null)
        {
            // Gọi hàm lưu của GridManager
            GridManager.Instance.SaveGameState();
        }

        // Đánh dấu là về Home
        this.CurrentMode = GameMode.Home;

        // Gọi UI về Home
        UiManager.Instance.SceneHome();
    }
}