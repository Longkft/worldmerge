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
                // Lưu data nếu cần
                // data.progress.currentLevelIndex = _levelCurrent;
            }
        }
    }

    // ================= HINT (GỢI Ý) =================
    private int _hintCount;
    public event Action<int> OnHintChanged; // Event riêng cho Hint

    public int HintCount
    {
        get => _hintCount;
        set
        {
            if (_hintCount != value)
            {
                _hintCount = value;
                // Chỉ bắn event cho ai quan tâm đến Hint
                OnHintChanged?.Invoke(_hintCount);

                // Đồng bộ ngược vào data tổng để save (Ví dụ)
                // data.inventory.hintCount = _hintCount;
            }
        }
    }

    // ================= SEARCH (TÌM KIẾM) =================
    private int _searchCount;
    public event Action<int> OnSearchChanged; // Event riêng cho Search

    public int SearchCount
    {
        get => _searchCount;
        set
        {
            if (_searchCount != value)
            {
                _searchCount = value;
                // Chỉ bắn event cho ai quan tâm đến Search
                OnSearchChanged?.Invoke(_searchCount);

                // data.inventory.searchCount = _searchCount;
            }
        }
    }

    private async void Awake()
    {
        base.Awake();

        this.data = await DataManager.Instance.GetDataAsync(); // lấy toàn bộ data
        this.LevelCurrent = data.progress.currentLevelIndex;
        this.HintCount = data.progress.hints;
        this.SearchCount = data.progress.searchs;

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
        // Khi bạn gán thế này, cái 'set' ở trên sẽ chạy và bắn event -> UI tự đổi
        this.LevelCurrent++;
    }
}
