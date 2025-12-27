using UnityEngine;

public class SlotNode : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }

    // Dữ liệu Logic
    public ItemData CurrentData { get; private set; }

    // Tham chiếu đến Controller thay vì UI
    private ItemController _linkedController;

    public void Init(int row, int col)
    {
        this.Row = row;
        this.Col = col;
        this.name = $"Slot_{row}_{col}";
    }

    public void LinkController(ItemController controller)
    {
        _linkedController = controller;

        // --- SỬA Ở ĐÂY: Phải check null trước khi gọi hàm ---
        if (controller != null)
        {
            controller.SetOwnerSlot(this);
        }
    }

    public ItemController GetController()
    {
        return _linkedController;
    }

    public void SetData(ItemData newData)
    {
        this.CurrentData = newData;
        if (_linkedController != null)
        {
            // Setup lại data và cập nhật Owner
            _linkedController.Setup(newData, this);
        }
    }
}