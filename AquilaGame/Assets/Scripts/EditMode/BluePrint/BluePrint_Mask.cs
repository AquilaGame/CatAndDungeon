using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class BluePrint_Mask : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    bool isPointerActive = false;
    public RectTransform BLPRect;
    RectTransform Rect;
    bool isDrag = false;
    public float MaxZoom = 4.0f;
    public float ZoomSpeed = 0.5f;
    float zoomState = 1.0f;
    Vector2 MouseHolding = Vector2.zero;
    Vector2 LastRectPos = Vector2.zero;

    void Start()
    {
        Rect = GetComponent<RectTransform>();
    }
    void Update()
    {

        if (isDrag)
        {
            if (Input.GetKeyUp(KeyCode.Mouse2))
            {
                LastRectPos = BLPRect.anchoredPosition;
                isDrag = false;
            }
            else if (isPointerActive)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, Input.mousePosition, Camera.main, out Vector2 MousePos);
                Vector2 MouseMoveDelta = MousePos - MouseHolding;
                BLPRect.anchoredPosition = LastRectPos + MouseMoveDelta;
            }
        }
        else if (isPointerActive)
        {
            float f = Input.GetAxis("Mouse ScrollWheel");
            if (f != 0)
            {
                zoomState += f > 0 ? ZoomSpeed : -ZoomSpeed;
                if (zoomState > MaxZoom)
                    zoomState = MaxZoom;
                if (zoomState < 1.0f)
                {
                    zoomState = 1.0f;
                }
                BLPRect.localScale = new Vector3(zoomState, zoomState, 1.0f);
            }
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, Input.mousePosition, Camera.main, out MouseHolding);
                isDrag = true;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerActive = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerActive = false;
    }
}
