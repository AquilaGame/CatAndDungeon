using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlizzardSnow : SummonObjectBase
{
    public int LifeTime = 2;
    // Start is called before the first frame update
    override public void Start()
    {
        MapSet.nowCreator.BuildMesh();
        if (isSkipSummon) return;
        Sound.Play(SummonSound, this);
        OnScene.Report("<color=#55FFAA>[召唤法术]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#00EEFF>[" + Name + "]</color>正在成形！");
        Log = "在"+Creator.data.Name+"的回合开始时，对范围内的所有目标造成"+Values[1].ToString()+"d6点伤害";
        transform.localScale = new Vector3(Values[0] * OnGame.WorldScale, 1, Values[0] * OnGame.WorldScale);
    }

    override public void OnRoundStart()
    {
        OnScene.Report("<color=#55FFAA>[法术伤害]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#00EEFF>[" + Name + "]</color>开始咆哮着降下冰雨！");
        float distance = OnGame.WorldScale * Values[0] / 2.0f;
        foreach (var v in MapSet.nowCreator.characters)
        {
            if (RangeCharacterSelector.CheckDistance(transform.position, v.transform.position, distance))
            {
                v.DamageRollHP("寒冰风暴:伤害效果（目标:" + v.data.Name + "）",6,(int)Values[1]);
            }
        }
        Sound.Play(OnRoundStartSound, this);
        LifeTime--;
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[法术消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#00EEFF>[" + Name + "]</color>消散了。");
            Destroy(gameObject);
        }
    }


}
