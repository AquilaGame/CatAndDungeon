using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MousePanel : MonoBehaviour
{
    [SerializeField] Text Txt1 = null;
    [SerializeField] Text Txt2 = null;
    [SerializeField] RectTransform rect = null;
    [SerializeField] RectTransform OnPlayCanvas = null;
    [SerializeField] Camera cam = null;
    public  Vector2 offsetDelta;
    public Vector2 TranslateScreenPosToCanvas(Vector3 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(OnPlayCanvas, pos, cam, out Vector2 ret);
        return ret;
    }
    public void FollowMouse(Vector3 pos)
    {
        rect.anchoredPosition = TranslateScreenPosToCanvas(pos) + offsetDelta;
    }

    public void SetStr1(string s)
    {
        Txt1.text = s;
    }
    public void SetStr1(string s, Color color)
    {
        Txt1.text = s;
        Txt1.color = color;
    }
    public void SetStr2(string s)
    {
        Txt2.text = s;
    }
    public void SetStr2(string s, Color color)
    {
        Txt2.text = s;
        Txt2.color = color;
    }
}
