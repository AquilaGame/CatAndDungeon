using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkTeleport : SummonObjectBase
{
    // Start is called before the first frame update
    override public void Start()
    {
        Sound.Play(SummonSound, this);
        Creator.NavAgent.Warp(transform.position);
        Destroy(gameObject);
    }

}
