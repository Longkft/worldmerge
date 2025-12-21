using System.Collections.Generic;

[System.Serializable]
public class LevelGroup
{
    public string id;
    public List<string> words;
    public string result;
}

[System.Serializable]
public class MergeLevel
{
    public int level;
    public int mergePerGroup;
    public List<LevelGroup> groups;
}

[System.Serializable]
public class MergeLevelWrapper
{
    public List<MergeLevel> levels;
}
