// Đặt tên cho dễ nhớ, sau này gõ code nó tự gợi ý
using UnityEngine;

public enum SoundType
{
    None = 0,

    // --- UI & SYSTEM ---
    Click_Button,
    Popup_Open,
    Popup_Close,
    Win_Level,
    Win,
    Lose_Level,

    // --- GAMEPLAY ---
    Item_Merge,
    Item_Drop,
    Item_Wrong,

    // --- MUSIC (BGM) ---
    BGM_Home,
    BGM_GamePlay
}

[System.Serializable]
public class SoundItem
{
    public SoundType type;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f; // Chỉnh volume riêng cho từng clip nếu cần
}