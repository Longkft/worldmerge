using UnityEngine;
using TMPro;
using DG.Tweening;

public class RowCompletedView : MonoBehaviour
{
    [SerializeField] private TextMeshPro txtTitle;   // Tên nhóm (COLOR)
    [SerializeField] private TextMeshPro txtContent; // List item (RED, BLUE...)

    private float numberScale = 0.72f;

    public void Setup(string title, string content)
    {
        if (txtTitle) txtTitle.text = title;
        if (txtContent) txtContent.text = content;
    }

    public void ActiveEff()
    {
        // eff
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(new Vector3(numberScale, numberScale, numberScale), 0.3f).SetEase(Ease.OutBack);
    }
}