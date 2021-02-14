using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTrap : SummonObjectBase
{
    public float BoomDistance = 1.5f;
    public int LifeTime = 10;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        base.Start();
        Log = "对不幸踩中的倒霉蛋造成" + Values[1].ToString() + "d6点伤害";
    }

    void CheckBOOOOOOOOM()
    {
        if (LifeTime == 10)
            OnScene.Report("<color=#55FFAA>[图腾效果]: </color><color=Orange>[" + Creator.data.Name
                            + "]</color>布置的<color=#FF5555>[" + Name + "]</color>开始生效。");
        else
        {
            foreach (var v in MapSet.nowCreator.characters)
            {
                if (RangeCharacterSelector.CheckDistance(transform.position, v.transform.position, BoomDistance)
                    && RangeCharacterSelector.CheckAlignment(RangeCharacterSelector.SelectorType.WorldCircle, Creator.data, v.data, SkillStartScript.Filter.Enemy))
                {
                    v.DamageRollHP("符文陷阱:伤害效果（目标:" + v.data.Name + "）", 6, (int)Values[1]);
                    OnScene.Report("<color=#55FFAA>[图腾效果]: </color><color=Orange>[" + Creator.data.Name
                            + "]</color>布置的<color=#FF5555>[" + Name + "]</color>爆炸了！");
                    Sound.Play(OnRoundStartSound);
                    Destroy(gameObject);
                }
            }
        }
    }

    override public void OnRoundStart()
    {
        CheckBOOOOOOOOM();
        LifeTime--;
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[法术消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#FF5555>[" + Name + "]</color>消散了。");
            Destroy(gameObject);
        }
    }

    public override void OnRightClick(CharacterScript user)
    {
        CheckBOOOOOOOOM();
    }
}
