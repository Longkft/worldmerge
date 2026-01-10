using UnityEngine;

public class UiManager : Singleton<UiManager>
{
    public GameObject panelHome;
    public GameObject panelGamePlay;
    public GameObject panelShop;
    public GameObject effEndGame;
    public GameObject gameWord;

    public void SceneHome()
    {
        panelHome.SetActive(true);
        panelGamePlay.SetActive(false);
        panelShop.SetActive(false);
        effEndGame.SetActive(false);
        gameWord.SetActive(false);
    }

    public void SceneGamePlay()
    {
        panelGamePlay.SetActive(true);
        panelHome.SetActive(false);
        panelShop.SetActive(false);
        effEndGame.SetActive(false);
        gameWord.SetActive(true);
    }

    public void SceneShop()
    {
        panelShop.SetActive(true);
        effEndGame.SetActive(false);
    }

    public void ActiceEffEndGame()
    {
        effEndGame.SetActive(true);
    }
}
