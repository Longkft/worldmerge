using TMPro;
using UnityEngine;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bgRendererNone;
    [SerializeField] private SpriteRenderer bgRendererActive;
    [SerializeField] private TextMeshPro textWord;
    [SerializeField] private SpriteRenderer dataIcon;

    private int _defaultSortingOrder;

    private void Awake()
    {
        // Lưu lại layer gốc để khi thả ra thì trả về như cũ
        if (bgRendererNone) _defaultSortingOrder = bgRendererNone.sortingOrder;
    }

    public void InitUi()
    {
        bgRendererNone.gameObject.SetActive(true);
        bgRendererActive.gameObject.SetActive(false);
        textWord.gameObject.SetActive(true);
        dataIcon.gameObject.SetActive(false);

        // Reset layer
        SetSortingOrder(_defaultSortingOrder);
    }

    public void SetDataText(string text)
    {
        textWord.text = text;
        textWord.gameObject.SetActive(true);
        dataIcon.gameObject.SetActive(false);
    }

    public void SetDataIcon(Sprite icon)
    {
        dataIcon.sprite = icon;
        textWord.gameObject.SetActive(false);
        dataIcon.gameObject.SetActive(true);
    }

    // --- HÀM MỚI: Xử lý hiển thị khi kéo thả ---

    public void SetSortingOrder(int order)
    {
        // Đưa tất cả lên lớp trên cùng để không bị che bởi Slot khác
        if (bgRendererNone) bgRendererNone.sortingOrder = order;
        if (bgRendererActive) bgRendererActive.sortingOrder = order;
        if (textWord) textWord.sortingOrder = order + 1;
        if (dataIcon) dataIcon.sortingOrder = order + 1;
    }

    public int GetDefaultSortingOrder() => _defaultSortingOrder;
}