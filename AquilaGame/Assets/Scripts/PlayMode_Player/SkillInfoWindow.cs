using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillInfoWindow : MonoBehaviour
{
    [SerializeField] Text Preview_NameStr = null;
    [SerializeField] Text Preview_CostStr = null;
    [SerializeField] Text Preview_TypeStr = null;
    [SerializeField] Text Preview_InfoStr = null;
    public void SetUp(Skill skill)
    {
        Preview_NameStr.text = skill.GetRank(out int color) + " " + skill.nameStr;
        Preview_NameStr.color = OnGame.MagicLib.SkillRankColors[color];
        Preview_CostStr.text = "法术消耗:" + skill.MPcost;
        Preview_TypeStr.text = skill.typeStr;
        Preview_InfoStr.text = skill.infoStr;
    }
}
