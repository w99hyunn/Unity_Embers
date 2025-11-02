using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public string prefsKey = "";
    public RectTransform resizeHandle;

    private RectTransform targetRect;
    private Vector2 originalSize;
    private Vector2 originalMousePosition;

    private void Awake()
    {
        TryGetComponent<RectTransform>(out targetRect);

        if (PlayerPrefs.HasKey(prefsKey))
        {
            float savedWidth = PlayerPrefs.GetFloat(prefsKey);
            targetRect.sizeDelta = new Vector2(savedWidth, targetRect.sizeDelta.y);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 마우스 클릭 시점 크기 및 위치 저장
        originalSize = targetRect.sizeDelta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(resizeHandle, eventData.position, eventData.pressEventCamera, out originalMousePosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (targetRect == null) return;

        // 현재 마우스 위치 가져오기
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(resizeHandle, eventData.position, eventData.pressEventCamera, out localMousePosition);

        // X축 크기만 조절
        float newWidth = originalSize.x + (localMousePosition.x - originalMousePosition.x);
        newWidth = Mathf.Max(newWidth, 100); // 최소 크기 제한

        targetRect.sizeDelta = new Vector2(newWidth, targetRect.sizeDelta.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 크기 저장
        PlayerPrefs.SetFloat(prefsKey, targetRect.sizeDelta.x);
        PlayerPrefs.Save();
    }
}
