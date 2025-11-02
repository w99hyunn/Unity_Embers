using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DisableDragScrollRect : ScrollRect
{
    // 드래그 입력을 무시하도록 이벤트를 오버라이드
    public override void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 이벤트 무시
    }

    public override void OnDrag(PointerEventData eventData)
    {
        // 드래그 이벤트 무시
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 이벤트 무시
    }
}