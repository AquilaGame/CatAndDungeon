using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTotem : SummonObjectBase
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
            Values[1] *= 1.25f;
            Name = "强能" + Name;
        }
        Log = "图腾中蕴含的强大元素之力使友军的火焰法术伤害提高" + Values[1].ToString() + "%";
        ObjTrans.localScale = new Vector3(Values[0] * OnGame.WorldScale, 1, Values[0] * OnGame.WorldScale);
        OnRoundStart();
    }

    void GiveBuff(string name, int time, float val)
    {
        float distance = OnGame.WorldScale * Values[0]/2.0f;

        foreach (var v in MapSet.nowCreator.characters)
        {
            if (RangeCharacterSelector.CheckDistance(transform.position, v.transform.position, distance)
                && RangeCharacterSelector.CheckAlignment(RangeCharacterSelector.SelectorType.WorldCircle,Creator.data,v.data,SkillStartScript.Filter.Friendly))
                v.AddBuff(name, time, val);
        }
    }

    override public void OnRoundStart()
    {
        GiveBuff("火焰缭绕", 1, (int)Values[1]);
        LifeTime--;
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[图腾消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#FFEDCC>[" + Name + "]</color>耗尽了力量。");
            Destroy(gameObject);
        }
    }

}
