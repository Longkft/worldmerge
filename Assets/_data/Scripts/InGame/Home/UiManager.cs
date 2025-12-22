using UnityEngine;

public class UiManager : Singleton<UiManager>
{
    public GameObject panelHome;
    public GameObject panelGamePlay;

    public void SceneHome()
    {
        panelHome.SetActive(true);
        panelGamePlay.SetActive(false);
    }

    public void SceneGamePlay()
    {
        panelGamePlay.SetActive(true);
        panelHome.SetActive(false);
    }
}
