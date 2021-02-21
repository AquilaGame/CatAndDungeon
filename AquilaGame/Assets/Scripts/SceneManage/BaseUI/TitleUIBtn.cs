using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TitleUIBtn : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler
{
    [SerializeField] AudioClip OnClickSound = null;
    [SerializeField] AudioClip OnEnterSound = null;
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Sound.Play(OnEnterSound, this);
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Sound.Play(OnClickSound,this);
    }
}
