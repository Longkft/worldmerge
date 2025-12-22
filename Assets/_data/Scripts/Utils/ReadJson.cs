using System.Collections.Generic;
using UnityEngine;

// 1. Kế thừa từ Singleton<ReadJson> thay vì MonoBehaviour
public class ReadJson : Singleton<ReadJson>
{
    [Header("Settings")]
    [SerializeField] private string jsonFileName = "DataLevel/levels";

    private List<MergeLevel> _allLevels;

    // 2. Override lại Awake để khởi tạo dữ liệu
    protected override void Awake()
    {
        base.Awake();

        LoadLevels();
    }

    // 3. Override hàm này trả về true để giữ object khi chuyển scene
    protected override bool ShouldDontDestroyOnLoad()
    {
        return true;
    }

    private void LoadLevels()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile == null)
        {
            Debug.LogError($"Không tìm thấy file '{jsonFileName}' trong thư mục Resources!");
            return;
        }

        string jsonText = jsonFile.text.Trim();

        if (jsonText.StartsWith("["))
        {
            jsonText = "{ \"levels\": " + jsonText + " }";
        }

        try
        {
            MergeLevelWrapper wrapper = JsonUtility.FromJson<MergeLevelWrapper>(jsonText);

            if (wrapper != null && wrapper.levels != null)
            {
                _allLevels = wrapper.levels;
                Debug.Log($"<color=green> Đã load động thành công {_allLevels.Count} levels từ file {jsonFileName}.</color>");
                /*Debug.Log(this.GetLevelData(0));*/
            }
            else
            {
                Debug.LogError("Parse JSON thất bại: Dữ liệu trống.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Lỗi cú pháp JSON: {e.Message}");
        }
    }

    public MergeLevel GetLevelData(int index)
    {
        if (_allLevels == null || index < 0 || index >= _allLevels.Count) return null;
        return _allLevels[index];
    }

    public int TotalLevels => _allLevels != null ? _allLevels.Count : 0;
}