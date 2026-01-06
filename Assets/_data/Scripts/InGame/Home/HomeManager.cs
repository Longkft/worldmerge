using System;
using UnityEngine;

public class HomeManager : Singleton<HomeManager>
{
    public SaveData data;

    // 1. Biến private để lưu giá trị thực
    private int _levelCurrent;

    // 2. Sự kiện bắn ra số level mới mỗi khi thay đổi
    public event Action<int> OnLevelChanged;

    // 3. Property public để truy cập và gán
    public int LevelCurrent
    {
        get => _levelCurrent;
        set
        {
            // Chỉ cập nhật nếu giá trị mới khác giá trị cũ (tối ưu)
            if (_levelCurrent != value)
            {
                _levelCurrent = value;

                // BẮN SỰ KIỆN NGAY LẬP TỨC!
                // Bất kỳ ai đăng ký lắng nghe sẽ được gọi
                OnLevelChanged?.Invoke(_levelCurrent);

                // (Tùy chọn) Lưu data luôn nếu muốn
                // data.progress.currentLevelIndex = _levelCurrent;
                // DataManager.Instance.Save();
            }
        }
    }

    private async void Awake()
    {
        base.Awake();

        this.data = await DataManager.Instance.GetDataAsync(); // lấy toàn bộ data
        this.LevelCurrent = data.progress.currentLevelIndex;

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
