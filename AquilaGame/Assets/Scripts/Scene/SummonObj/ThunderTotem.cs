using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderTotem : SummonObjectBase
{
    [SerializeField] Transform ObjTrans = null;
    public int LifeTime = 5;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        base.Start();
        if (Creator.data.OccupationData.type == OccupationType.Shaman && Creator.data.OccupationData.branch == 1)
        {
            Values[1] += 1;
            Name = "强能" + Name;
        }
        Log = "图腾中蕴含的强大元素之力在每轮战斗开始时对范围内敌方目标造成" + Values[1].ToString() + "d6点伤害";
        ObjTrans.localScale = new Vector3(Values[0] * OnGame.WorldScale, 1, Values[0] * OnGame.WorldScale);
    }

    override public void OnRoundStart()
    {
        Sound.Play(OnRoundStartSound, this);
        OnScene.Report("<color=#55FFAA>[图腾效果]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#EE99EE>[" + Name + "]</color>降下了毁灭的雷电。");
        float distance = OnGame.WorldScale * Values[0] / 2.0f;
        foreach (var v in MapSet.nowCreator.characters)
        {
            if (RangeCharacterSelector.CheckDistance(transform.position, v.transform.position, distance)
                && RangeCharacterSelector.CheckAlignment(RangeCharacterSelector.SelectorType.WorldCircle, Creator.data, v.data, SkillStartScript.Filter.Enemy))
            {
                v.DamageRollHP("风暴图腾:伤害效果（目标:" + v.data.Name + "）", 6, (int)Values[1]);
            }
        }
        LifeTime--;
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[图腾消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#EE99EE>[" + Name + "]</color>耗尽了力量。");
            Destroy(gameObject);
        }
    }

}
