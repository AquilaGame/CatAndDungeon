using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillUnitOnPanel : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    public bool isField = false;
    public SkillCreatePanel skillCreatePanel = null;
    public FieldType fieldType = FieldType.Sword;
    public ElementType elementType = ElementType.Fire;
    public Image ChildImage = null;
    public bool isActive = false;
    public bool isHighLight = false;
    public GameObject UnitTextField = null;
    public Text PreviewTxt = null;
    Color color = Color.white;
    float r = 0, g = 0, b = 0;
    Image SelfImage;
    readonly static Vector2 ori_child_size = new Vector2(23,23);
    readonly static Vector2 ori_self_size = new Vector2(45,45);
    readonly static Vector2 highlight_child_size = new Vector2(27,27);
    readonly static Vector2 highlight_self_size = new Vector2(55,55);

    public void SetColorAlpha(float alpha)
    {
        SelfImage.color = new Color(r, g, b, alpha);
    }

    void preinit(SkillCreatePanel pnl)
    {
        SelfImage = GetComponent<Image>();
        isActive = true;
        skillCreatePanel = pnl;
    }
    void postinit()
    {
        ChildImage.color = color;
        r = color.r;
        g = color.g;
        b = color.b;
    }
    public void SetActive(FieldType fieldtype, SkillCreatePanel pnl)
    {
        preinit(pnl);
        isField = true;
        fieldType = fieldtype;
        ChildImage.sprite = OnGame.MagicLib.FldSprites[(int)fieldtype];
        color = OnGame.MagicLib.FldColors[(int)fieldtype];
        postinit();
    }
    public void SetActive(ElementType eletype, SkillCreatePanel pnl)
    {
        preinit(pnl);
        isField = false;
        elementType = eletype;
        ChildImage.sprite = OnGame.MagicLib.EleSprites[(int)eletype];
        color = OnGame.MagicLib.EleColors[(int)eletype];
        postinit();
    }

    public void Ring()
    {
        print("Ring");
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (isActive)
        {
            if (skillCreatePanel.isPointerObjActive)
            {
                return;
            }
            UnitTextField.SetActive(true);
            GetComponent<RectTransform>().sizeDelta = highlight_self_size;
            ChildImage.GetComponent<RectTransform>().sizeDelta = highlight_child_size;
            if (isField)
            {
                int index = (int)fieldType;
                PreviewTxt.text = "领域: " + Skill.FldColorStr[index] + Skill.FldDisNames[index] + Skill.EndColotStr;
            }
            else
            {
                int index = (int)elementType;
                PreviewTxt.text = "元素: " + Skill.EleColorStr[index] + Skill.EleDisNames[index] + Skill.EndColotStr;
            }
            isHighLight = true;
        }
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (isActive)
        {
            UnitTextField.SetActive(false);
            GetComponent<RectTransform>().sizeDelta = ori_self_size;
            ChildImage.GetComponent<RectTransform>().sizeDelta = ori_child_size;
            isHighLight = false;
        }
    }
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (isActive)
        {
            if (isField)
                skillCreatePanel.SetMouseDown(fieldType,color, ChildImage.sprite);
            else 
                skillCreatePanel.SetMouseDown(elementType,color, ChildImage.sprite);
        }
    }
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (isActive)
        {
            skillCreatePanel.OnPointerUp(eventData);
        }
    }
}
