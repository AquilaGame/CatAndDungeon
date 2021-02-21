using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkFog : SummonObjectBase
{
    public int LifeTime = 2;
    // Start is called before the first frame update
    override public void Start()
    {
        MapSet.nowCreator.BuildMesh();
        if (isSkipSummon) return;
        Sound.Play(SummonSound, this);
        OnScene.Report("<color=#55FFAA>[召唤法术]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#CC00FF>[" + Name + "]</color>正在成形！");
        Log = "进入迷雾的目标在其每回合开始时进行一次感知检定，若失败则陷入目盲状态";
        transform.localScale = new Vector3(Values[0] * OnGame.WorldScale, 1, Values[0] * OnGame.WorldScale);
    }

    //不是static的话当它消失了，回调就会出错误,可以UI打开顺序总结成一个栈
    static Stack<CharacterScript> characters = new Stack<CharacterScript>();
    public static void FogCallback(int val)
    {
        CharacterScript character = characters.Pop();
        if (val <= 0)
        {
            character.AddBuff("目盲", 1, 1);
        }
    }

    override public void OnRoundStart()
    {
        OnScene.Report("<color=#55FFAA>[法术效果]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#CC00FF>[" + Name + "]</color>遮蔽了光芒。");
        characters.Clear();
        float distance = OnGame.WorldScale * Values[0] / 2.0f;
        Sound.Play(OnRoundStartSound, this);
        foreach (var v in MapSet.nowCreator.characters)
        {
            if (RangeCharacterSelector.CheckDistance(transform.position, v.transform.position, distance))
            {
                characters.Push(v);
                CombatPanel combatPanel = OnScene.manager.OpenCombatPanel();
                combatPanel.SetUp("黑暗迷雾:豁免检定", v.data.Name, "迷雾", v.data.BaseData.WIS, 0, FogCallback, "0", "0", "-10");
            }
        }
        LifeTime--;
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[法术消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#00EEFF>[" + Name + "]</color>消散了。");
            Destroy(gameObject);
        }
    }


}
