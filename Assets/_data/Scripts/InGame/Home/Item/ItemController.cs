using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private ItemUI uiItem;
    [SerializeField] private ItemButton btnItem;

    // Dữ liệu nội tại
    // Thay đổi ở đây: Lưu cả cục data
    public ItemData Data { get; private set; }

    public bool IsSelected { get; private set; }

    public void Setup(ItemData data)
    {
        this.Data = data;
        IsSelected = false;

        // Setup UI chỉ cần lấy text ra hiển thị
        if (uiItem != null)
        {
            uiItem.InitUi();
            uiItem.SetDataText(data.word);
        }
    }
}
