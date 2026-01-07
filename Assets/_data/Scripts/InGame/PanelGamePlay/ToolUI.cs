using UnityEngine;
using TMPro;

public class ToolUI : MonoBehaviour
{
    [SerializeReference] TextMeshProUGUI numberOfHint;
    [SerializeReference] TextMeshProUGUI numberOfSearch;

    public void setValueHint(int hint)
    {
        if (numberOfHint != null)
        {
            this.numberOfHint.text = hint.ToString();
        }
    }

    public void setValueSearch(int numberSearch)
    {
        if (numberOfSearch != null)
        {
            this.numberOfSearch.text = numberSearch.ToString();
        }
    }
}
