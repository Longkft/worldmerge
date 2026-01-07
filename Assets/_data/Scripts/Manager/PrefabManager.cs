using UnityEngine;

public class PrefabManager : Singleton<PrefabManager>
{
    [Header("--- Gameplay Items ---")]
    public GameObject ItemSlot;
    public GameObject ItemGame;

    [Header("--- Popup ---")]
    public GameObject popupSetting;
}
