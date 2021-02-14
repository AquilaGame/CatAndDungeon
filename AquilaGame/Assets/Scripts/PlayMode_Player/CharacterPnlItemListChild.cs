using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CharacterPnlItemListChild : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    Item item;
    CharacterPanel Pnl;
    [SerializeField] Text Txt = null;
    [SerializeField] AudioClip MouseEnterSound = null;
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Pnl.ShowItemInfo(item);
        Sound.Play(MouseEnterSound, this);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Pnl.HideItemInfo();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnScene.onSelect.character.UseItem(item);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Pnl.EditItem(item);
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            OnScene.onSelect.character.RemoveItem(item);
        }
    }

    public void SetUp(Item it, CharacterPanel characterPanel)
    {
        item = it;
        Pnl = characterPanel;
        Txt.text = it.Name;
    }

}
