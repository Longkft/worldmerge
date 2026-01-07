using UnityEngine;
using TMPro;

public class PanelGameplayUI : MonoBehaviour
{
    [SerializeReference] TextMeshProUGUI groundEnd;
    [SerializeReference] TextMeshProUGUI levelUI;

    public void setGroundEnd(int numberGround) // hoàn thành được bao nhiêu dãy chữ, set ui
    {
        if (groundEnd != null)
        {
            this.groundEnd.text = numberGround.ToString() + "/6";
        }
    }

    public void setLevelUI(int levelGame) // hoàn thành được bao nhiêu dãy chữ, set ui
    {
        if (levelUI != null)
        {
            this.levelUI.text = "LEVEL " + levelGame.ToString();
        }
    }
}
