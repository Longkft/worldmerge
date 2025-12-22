using UnityEngine;

public class HomeManager : Singleton<HomeManager>
{
    public SaveData data;
    public int levelCurrent;
    private async void Awake()
    {
        base.Awake();

        this.data = await DataManager.Instance.GetDataAsync(); // lấy toàn bộ data
        this.levelCurrent = data.progress.currentLevelIndex;

        this.LoadDataLevelIndex();
    }

    protected void LoadDataLevelIndex()
    {
        var levelData = ReadJson.Instance.GetLevelData(this.levelCurrent - 1);

        string logContent = JsonUtility.ToJson(levelData, true);

        Debug.Log($"Data Level {this.levelCurrent}:\n" + logContent);
    }
}
