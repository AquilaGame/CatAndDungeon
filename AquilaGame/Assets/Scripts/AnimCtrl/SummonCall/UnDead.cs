using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnDead : SummonScriptBase
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Sound.Play(SummonSound, Self);
    }

}
