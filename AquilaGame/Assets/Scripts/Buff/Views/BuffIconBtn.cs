using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BuffIconBtn : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]public BuffPanel Pnl;
    Image image = null;

    public void SetUp(BuffPanel panel, Buff buff)
    {
        image = GetComponent<Image>();
        image.sprite = buff.Icon;
        Pnl = panel;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Pnl.OnPointerLeftClick(this);
        else
            Pnl.OnPointerRightClick(this);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Pnl.OnPointerEnter(this);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Pnl.OnPointerExit();
    }
}
