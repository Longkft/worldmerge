using UnityEngine;

public class PrefabManager : Singleton<PrefabManager>
{
    [Header("--- Gameplay Items ---")]
    public GameObject ItemSlot;
    public GameObject ItemGame;
    public GameObject ItemCompleted;

    [Header("--- Popup ---")]
    public GameObject popupSetting;
    public GameObject popupEndGame;
    public GameObject popupTutorial;
}
