using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class CharacterPnlMagicBookChild : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    Skill skill;
    CharacterPanel Pnl;
    [SerializeField] Text Txt = null;
    [SerializeField] AudioClip MouseEnterSound = null;
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Pnl.ShowSkillInfo(skill);
        Pnl.PreviewAPCost(skill);
        Sound.Play(MouseEnterSound, this);
        OnScene.onSelect.character.SetSkillPreview(skill);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Pnl.HideSkillInfo();
        Pnl.PreviewAPCost(Character.ZeroAPCost);
        OnScene.onSelect.character.ResetSkillPreview();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnScene.onSelect.character.UseSkill(skill);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Pnl.ChangeSkill(skill);
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Pnl.PreviewAPCost(Character.ZeroAPCost);
            OnScene.onSelect.character.RemoveSkill(skill);
        }
    }

    public void SetUp(Skill sk, CharacterPanel characterPanel)
    {
        skill = sk;
        Pnl = characterPanel;
        Txt.text = sk.nameStr;
    }



}
