using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptWater : SummonObjectBase
{
    public int LifeTime = 5;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        base.Start();
        Log = "(Shift+右键点击使用)纯天然，富有营养，饮用可恢复"+((int)Values[1]).ToString()+"d6点生命值和"+(2* (int)Values[1]).ToString() + "d4点MP，古神出品，质量保证（如果你能无视泉水里的残肢和触手的话）";
    }

    override public void OnRoundStart()
    {
        LifeTime--;
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[法术消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#FFEDCC>[" + Name + "]</color>自行消散了。");
            Destroy(gameObject);
        }
    }

    public override void OnRightClick(CharacterScript user)
    {
        user.HealRollHP("腐化之泉:HP回复", 6, (int)Values[1]);
        user.HealRollMP("腐化之泉:MP回复", 4, 2*(int)Values[1]);
    }

}
