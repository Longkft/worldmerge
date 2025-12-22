using UnityEngine;

public class LevelLoader : Singleton<LevelLoader>
{
    void Start()
    {
        TextAsset json = Resources.Load<TextAsset>("DataLevel");

        if (json == null)
        {
            Debug.LogError("❌ Không load được file levels.json");
            return;
        }

        Debug.Log("📄 JSON RAW:\n" + json.text);

        MergeLevelWrapper data = JsonUtility.FromJson<MergeLevelWrapper>(json.text);

        if (data == null || data.levels == null)
        {
            Debug.LogError("❌ Parse JSON thất bại");
            return;
        }

        Debug.Log("✅ Số level load được: " + data.levels.Count);
    }
}
