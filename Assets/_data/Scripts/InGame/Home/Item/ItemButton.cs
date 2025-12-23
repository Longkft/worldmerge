using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : BaseTouch
{
    private ItemController _controller;

    public void Init(ItemController controller)
    {
        _controller = controller;
    }

    protected override void OnTouchStart(PointerEventData data)
    {
        if (_controller) _controller.OnBeginDrag();
    }

    protected override void OnTouchMove(PointerEventData data)
    {
        if (_controller) _controller.OnDrag();
    }

    protected override void OnTouchEnd(PointerEventData data)
    {
        if (_controller) _controller.OnEndDrag();
    }
}