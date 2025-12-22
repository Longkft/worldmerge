using TMPro;
using UnityEngine;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bgRendererNone;
    [SerializeField] private SpriteRenderer bgRendererActive;

    [SerializeField] private TextMeshPro textWord;
    [SerializeField] private SpriteRenderer dataIcon;

    private void OnEnable()
    {
        this.InitUi();
    }

    public void InitUi()
    {
        this.bgRendererNone.gameObject.SetActive(true);
        this.bgRendererActive.gameObject.SetActive(false);
        this.textWord.gameObject.SetActive(true);
        this.dataIcon.gameObject.SetActive(false);
    }

    public void SetDataText(string text)
    {
        this.textWord.text = text;

        // Đảm bảo hiển thị đúng chế độ Text
        textWord.gameObject.SetActive(true);
        dataIcon.gameObject.SetActive(false);
    }

    public void SetDataIcon(Sprite icon)
    {
        this.dataIcon.sprite = icon;

        // Đảm bảo hiển thị đúng chế độ Text
        textWord.gameObject.SetActive(false);
        dataIcon.gameObject.SetActive(true);
    }
}
