using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SelectBtnBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public delegate void SelectBtnBhv(int index);
    public SelectBtnBhv onEnter;
    public SelectBtnBhv onExit;
    public SelectBtnBhv click;
    public bool isEnable;
    public int index;
    public void SetBehaviour(SelectBtnBhv OnEnter, SelectBtnBhv OnExit, SelectBtnBhv OnClick)
    {
        onEnter = OnEnter;
        onExit = OnExit;
        click = OnClick;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter(index);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        onExit(index);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        click(index);
    }
    public virtual void SetBtn(object type)
    {

    }
    public virtual void SetDisable()
    {

    }
}
