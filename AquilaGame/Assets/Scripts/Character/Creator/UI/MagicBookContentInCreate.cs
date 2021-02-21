using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MagicBookContentInCreate : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public List<Image> eles = new List<Image>(6);
    public Text Name = null;
    [HideInInspector]public Skill skill = null;
    public Sprite TransparentSprite = null;
    GameObject infoWindow;
    SkillInfoWindow infoWindowSpt = null;
    MagicBookInCreate Pnl = null;
    public void SetUp(Skill _skill, GameObject wnd,MagicBookInCreate parentPnl)
    {
        skill = _skill;
        Name.text = skill.GetRank(out int i) + skill.nameStr;
        if (i < 3)
        {
            for (i = 0; i < skill.FldForce; i++)
            {
                eles[i].sprite = OnGame.MagicLib.FldSprites[(int)skill.protype.field];
            }
            for (int j = 0; j < skill.EleForce; j++)
            {
                eles[i++].sprite = OnGame.MagicLib.EleSprites[(int)skill.protype.element];
            }
            for (; i < 6; i++)
            {
                eles[i].sprite = TransparentSprite;
            }
        }
        infoWindow = wnd;
        infoWindowSpt = infoWindow.GetComponent<SkillInfoWindow>();
        Pnl = parentPnl;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        infoWindow.SetActive(false);
        Pnl.Delete(this);
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        infoWindow.SetActive(true);
        infoWindowSpt.SetUp(skill);
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        infoWindow.SetActive(false);
    }
}
