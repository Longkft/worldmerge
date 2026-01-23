using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks; // Cần thiết cho Async/Await
using System.Text;

public enum GamePlayMode
{
    Word, // Chế độ từ vựng
    Math  // Chế độ toán học
}

//DATA MODELS(Mở rộng thoải mái) ---
[Serializable]
public class UserProgress
{
    public int wordLevelIndex = 1; // Level Word
    public int mathLevelIndex = 1; // Level Math (Mới)

    // Giữ lại biến cũ để migration nếu cần, hoặc bỏ đi
    // public int currentLevelIndex = 1; 

    public int coins = 0;
    public int hints = 0;
    public int searchs = 0;
}

[Serializable]
public class GameSettings
{
    // Nhạc nền (BGM)
    public bool isMusicOn = true;
    public float musicVolume = 1.0f;

    // Âm thanh hiệu ứng (SFX)
    public bool isSfxOn = true;
    public float sfxVolume = 1.0f;

    // Rung (Vibration)
    public bool vibration = true;
}

[Serializable]
public class SaveData
{
    public UserProgress progress = new UserProgress();
    public GameSettings settings = new GameSettings();
}

public class DataManager : Singleton<DataManager>
{
    private string _filePath;

    // Cache dữ liệu trong RAM (để không phải đọc file liên tục)
    private SaveData _cachedData;

    // Cờ đánh dấu đang lưu để tránh lưu chồng chéo
    private bool _isSaving = false;

    // --- KHO CHỨA DATA LEVEL (Trong RAM) ---
    private List<MergeLevel> _wordLevels = new List<MergeLevel>();
    private List<MergeLevel> _mathLevels = new List<MergeLevel>();

    // Tên file trong thư mục Resources
    [Header("Data Config")]
    [SerializeField] private string wordDataFileName = "DataLevel/levels";
    [SerializeField] private string mathDataFileName = "DataLevel/math_data";

    // --- SETUP SINGLETON ---
    protected override void Awake()
    {
        base.Awake(); // Gọi base của Singleton

        _filePath = Path.Combine(Application.persistentDataPath, "gamedata.json");

        Debug.Log("File Save nằm ở đây: " + _filePath);

        // Load dữ liệu tĩnh (Level JSON) ngay khi game bật
        this.LoadAllLevelData();
    }

    protected override bool ShouldDontDestroyOnLoad() => true;

    private void LoadAllLevelData()
    {
        _wordLevels = LoadLevelsFromResource(wordDataFileName);
        _mathLevels = LoadLevelsFromResource(mathDataFileName);
        Debug.Log($"<color=green>Loaded: {_wordLevels.Count} Word Levels, {_mathLevels.Count} Math Levels.</color>");
    }

    private List<MergeLevel> LoadLevelsFromResource(string fileName)
    {
        TextAsset targetFile = Resources.Load<TextAsset>(fileName);
        if (targetFile != null)
        {
            string jsonText = targetFile.text.Trim();

            // Xử lý format JSON array nếu cần (giống logic ReadJson cũ của bạn)
            if (jsonText.StartsWith("["))
            {
                jsonText = "{ \"levels\": " + jsonText + " }";
            }

            try
            {
                MergeLevelWrapper wrapper = JsonUtility.FromJson<MergeLevelWrapper>(jsonText);
                if (wrapper != null && wrapper.levels != null)
                {
                    return wrapper.levels;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Parse JSON {fileName} thất bại: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"Không tìm thấy file Resource: {fileName}");
        }
        return new List<MergeLevel>(); // Trả về list rỗng nếu lỗi
    }

    // --- API LẤY LEVEL DATA THEO MODE ---
    public MergeLevel GetLevelData(int levelIndex, GamePlayMode mode)
    {
        List<MergeLevel> targetList = (mode == GamePlayMode.Math) ? _mathLevels : _wordLevels;

        // Tìm level trong list. Level trong JSON của bạn bắt đầu từ 1.
        return targetList.Find(x => x.level == levelIndex);
    }

    // =========================================================
    // 1. HÀM ĐỌC DỮ LIỆU (ASYNC) - Cốt lõi yêu cầu của bạn
    // =========================================================

    /// <summary>
    /// Lấy dữ liệu game. Nếu chưa có trong RAM, sẽ đọc từ ổ cứng (Async).
    /// Nếu chưa có file, sẽ tạo mới.
    /// </summary>
    public async Task<SaveData> GetDataAsync()
    {
        // 1. Nếu đã có dữ liệu trong RAM rồi thì trả về luôn (siêu nhanh)
        if (_cachedData != null)
        {
            return _cachedData;
        }

        // 2. Nếu chưa có, bắt đầu đọc file bất đồng bộ
        if (!File.Exists(_filePath))
        {
            Debug.Log("File save chưa tồn tại -> Tạo mới.");
            _cachedData = new SaveData();
            // Lưu file mới tạo xuống đĩa (chạy ngầm, không cần await để game chạy tiếp)
            _ = SaveDataAsync();
            return _cachedData;
        }

        try
        {
            string json = "";

            // Đọc file từ ổ cứng (IO Bound - chạy luồng phụ)
            using (StreamReader reader = new StreamReader(_filePath, Encoding.UTF8))
            {
                json = await reader.ReadToEndAsync();
            }

            // Parse JSON (CPU Bound - chạy luồng chính)
            // JsonUtility bắt buộc chạy ở Main Thread nên await xong nó tự về Main Thread
            _cachedData = JsonUtility.FromJson<SaveData>(json);

            // Validate dữ liệu (phòng trường hợp update game thêm trường mới)
            if (_cachedData == null) _cachedData = new SaveData();
            if (_cachedData.progress == null) _cachedData.progress = new UserProgress();
            if (_cachedData.settings == null) _cachedData.settings = new GameSettings();

            return _cachedData;
        }
        catch (Exception e)
        {
            Debug.LogError("Lỗi đọc file save: " + e.Message + " -> Tạo mới data.");
            _cachedData = new SaveData();
            return _cachedData;
        }
    }

    // =========================================================
    // 2. HÀM GHI DỮ LIỆU (ASYNC)
    // =========================================================
    public async Task SaveDataAsync()
    {
        if (_cachedData == null || _isSaving) return;

        _isSaving = true;

        // Serialize JSON (Nhanh)
        string json = JsonUtility.ToJson(_cachedData, true);

        try
        {
            // Ghi xuống ổ cứng (Chậm -> Async)
            using (StreamWriter writer = new StreamWriter(_filePath, false, Encoding.UTF8))
            {
                await writer.WriteAsync(json);
            }
            // Debug.Log("💾 Saved Async!");
        }
        catch (Exception e)
        {
            Debug.LogError("Lỗi ghi file save: " + e.Message);
        }
        finally
        {
            _isSaving = false;
        }
    }

    // --- HELPER: Lưu đồng bộ (Dùng khi Quit Game bắt buộc phải xong ngay) ---
    public void SaveDataSync()
    {
        if (_cachedData == null) return;
        try
        {
            string json = JsonUtility.ToJson(_cachedData);
            File.WriteAllText(_filePath, json);
        }
        catch { }
    }

    private void OnApplicationQuit()
    {
        SaveDataSync();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) _ = SaveDataAsync();
    }
}