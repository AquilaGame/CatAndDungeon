using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaintTotem : SummonObjectBase
{
    [SerializeField] Transform ObjTrans = null;
    public int LifeTime = 5;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        base.Start();
        Log = "圣像寄托着神的意志，在"+Creator.data.Name+"的回合开始时对范围内最近的邪恶阵营目标造成" + Values[1].ToString() + "d6点伤害";
        ObjTrans.localScale = new Vector3(Values[0] * OnGame.WorldScale, 1, Values[0] * OnGame.WorldScale);
    }

    override public void OnRoundStart()
    {
        Sound.Play(OnRoundStartSound, this);
        OnScene.Report("<color=#55FFAA>[神像效果]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#EEEE99>[" + Name + "]</color>降下了惩罚之雷。");
        float distance = OnGame.WorldScale * Values[0] / 2.0f;
        int index = -1;
        float minDis = 99;
        for (int i = 0; i < MapSet.nowCreator.characters.Count;i++)
        {            if (MapSet.nowCreator.characters[i].data.alignmentType == AlignmentType.LawfulEvil ||
                MapSet.nowCreator.characters[i].data.alignmentType == AlignmentType.ChaoticEvil ||
                MapSet.nowCreator.characters[i].data.alignmentType == AlignmentType.NeutralEvil)
            {
                float dis = new Vector3(transform.position.x - MapSet.nowCreator.characters[i].transform.position.x
                , 0, transform.position.z - MapSet.nowCreator.characters[i].transform.position.z).magnitude;
                if (dis <= distance)
                {
                    if (dis < minDis)
                    {
                        minDis = dis;
                        index = i;
                        //写到这发现犯傻了，以为i有用来着，事实上没啥用，写个foreach多好（懒得改了）
                    }
                }
            }
        }
        if (index >= 0)
            MapSet.nowCreator.characters[index].DamageRollHP("威仪之像:伤害效果（目标:" + MapSet.nowCreator.characters[index].data.Name + "）", 6, (int)Values[1]);
        LifeTime--;
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[法术消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#EEEE99>[" + Name + "]</color>消散了。");
            Destroy(gameObject);
        }
    }

}
