[System.Serializable]
public class ItemData
{
    public string word;      // Ví dụ: "đi"
    public string groupId;   // Ví dụ: "MOVE"
    public string groupName; // Ví dụ: "di chuyển" (Result)

    public ItemData(string w, string id, string name)
    {
        this.word = w;
        this.groupId = id;
        this.groupName = name;
    }
}
