using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThonsFloor : SummonObjectBase
{
    public int LifeTime = 3;
    [SerializeField] Transform ObjTrans = null;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        base.Start();
        Log = "地面上充满了带有尖刺的荆棘，区域内的目标移动速度减少" + Values[1].ToString() + "%";
        ObjTrans.localScale = new Vector3(Values[0] * OnGame.WorldScale, 1, Values[0] * OnGame.WorldScale);
    }

    void GiveBuff(string name, int time, float val)
    {
        float distance = OnGame.WorldScale * Values[0] / 2.0f;

        foreach (var v in MapSet.nowCreator.characters)
        {
            if (RangeCharacterSelector.CheckDistance(transform.position, v.transform.position, distance))
                v.AddBuff(name, time, val);
        }
    }

    override public void OnRoundStart()
    {
        Sound.Play(OnRoundStartSound, this);
        LifeTime--;
        GiveBuff("荆棘丛生", 1, Values[1]);
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[法术消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#55EE55>[" + Name + "]</color>消散了。");
            Destroy(gameObject);
        }
    }
}
