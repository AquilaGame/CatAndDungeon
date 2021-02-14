 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidPortal : SummonObjectBase
{
    static VoidPortal WaitToLink = null;
    [HideInInspector] public VoidPortal LinkTo = null;
    public int LifeTime = 5;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        base.Start();
        Log = "踏足虚空，传送有生命角色时对其造成" + Values[1].ToString() + "d4点伤害";
        if (WaitToLink != null)
        {
            LinkTo = WaitToLink;
            WaitToLink.LinkTo = this;
            WaitToLink = null;
        }
        else
        {
            WaitToLink = this;
        }
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
    public override void OnDestroy()
    {
        if (LinkTo != null && LinkTo.LinkTo != null)
        {
            LinkTo.LinkTo = null;
        }
        base.OnDestroy();
    }

    public override void OnRightClick(CharacterScript user)
    {
        if (LinkTo != null)
            OnScene.manager.SummonObjCall(LinkTo.transform.position, Quaternion.identity, user, "暗夜传送门", 0, 0);
        else
            base.OnRightClick(user);
    }

}
