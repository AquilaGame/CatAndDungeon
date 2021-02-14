using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UserPanelBtn : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] Image image = null;
    [SerializeField] AudioClip OnMouseEnterSound = null;
    public delegate void UserPanelBtnBhv();
    public UserPanelBtnBhv bhv;
    public UserPanelBtnBhv onPointerEnterBhv;
    public UserPanelBtnBhv onPointerExitBhv;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    public void SetUp(Sprite sprite, Color color,UserPanelBtnBhv _bhv, UserPanelBtnBhv _OnPointEnterBhv = null, UserPanelBtnBhv _OnPointerExitBhv = null)
    {
        image.sprite = sprite;
        image.color = color;
        bhv = _bhv;
        onPointerEnterBhv = _OnPointEnterBhv;
        onPointerExitBhv = _OnPointerExitBhv;
    }
    void OnClick()
    {
        bhv?.Invoke();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnterBhv?.Invoke();
        Sound.Play(OnMouseEnterSound,this);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        onPointerExitBhv?.Invoke();
    }
}
