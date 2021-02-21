using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTotem : SummonObjectBase
{
    [SerializeField] Transform ObjTrans = null;
    [SerializeField] Transform ObjTrans2 = null;
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
        Log = "图腾中蕴含的强大元素之力在每轮战斗结束时为范围内友方目标恢复" + Values[1].ToString() + "d6生命值";
        ObjTrans.localScale = new Vector3(Values[0] * OnGame.WorldScale, 1, Values[0] * OnGame.WorldScale);
        ObjTrans2.localScale = new Vector3(Values[0] * OnGame.WorldScale /2, 1, Values[0] * OnGame.WorldScale/2);
    }

    override public void OnRoundStart()
    {
        OnScene.Report("<color=#55FFAA>[图腾效果]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#00FFCC>[" + Name + "]</color>散发出生命的气息。");
        float distance = OnGame.WorldScale * Values[0] / 2.0f;
        foreach (var v in MapSet.nowCreator.characters)
        {
            if (RangeCharacterSelector.CheckDistance(transform.position, v.transform.position, distance)
                && RangeCharacterSelector.CheckAlignment(RangeCharacterSelector.SelectorType.WorldCircle, Creator.data, v.data, SkillStartScript.Filter.Friendly))
            {
                v.HealRollHP("治疗图腾:治疗效果（目标:" + v.data.Name + "）", 6, (int)Values[1]);
            }
        }
        LifeTime--;
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[图腾消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#00EEFF>[" + Name + "]</color>耗尽了力量。");
            Destroy(gameObject);
        }
    }

}
