using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenDear : SummonScriptBase
{
    public override void Start()
    {
        if (isSkipSummon)
        {
            return;
        }
        base.Start();
        Sound.Play(SummonSound, Self);
    }
    public void OnDestroy()
    {
        if (Creator != null)
        {
            Creator.HealRollMP("活化植物:法力回收",4,(int)Value);
        }
    }

}
