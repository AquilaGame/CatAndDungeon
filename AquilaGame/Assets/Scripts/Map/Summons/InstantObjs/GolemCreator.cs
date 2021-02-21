using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemCreator : SummonObjectBase
{
    public string CallName = "金属构体";
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        base.Start();
        Log = "如果有足够多的金属，可将其堆放在这个魔法阵中结合成构体，当前的魔法等级能够处理";
        switch ((int)Values[0])
        {
            case 1: Log += "银或秘银等导魔金属";break;
            case 2: Log += "轻金属和铁等普通金属"; break;
            case 3: Log += "钢、铜及导魔性更强的金属"; break;
            case 4: Log += "铅、青铜等重合金"; break;
            default:Log += "任何金属"; break;
        }
        Log += "。 构成的魔像具有一定的抗魔能力，受到的魔法伤害减少" + Values[1].ToString() + "%";
    }

    override public void OnRoundStart()
    {
        CharacterScript cs =  OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, CallName, 0);
        cs.AddBuff("魔法免疫",-1,Values[1]);
        Destroy(gameObject);
    }

}
