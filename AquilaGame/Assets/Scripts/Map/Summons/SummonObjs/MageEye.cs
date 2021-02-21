using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEye : SummonObjectBase
{
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        base.Start();
        Log = "永久隐身，奥术之眼的拥有者可随时通过此眼睛观察" + Values[0].ToString()+ "米内的情况，每次消耗"+Values[1].ToString()+"点MP";
        OnRoundStart();
    }

    override public void OnRoundStart()
    {
    }

}
