using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Buff
{
    public string Name;
    public int index;
    public int continueTime;
    public float value;
    public CharacterScript character;
    protected GameObject obj;
    public Buff(int idx,CharacterScript cs, int time, float val)
    {
        Name = OnGame.buffLib.names[idx];
        index = idx;
        character = cs;
        continueTime = time;
        value = val;
    }

    public virtual string Info()
    {
        return "未知的状态描述";
    }
    //加载和添加都调用，刷新显示
    public virtual void OnBuffEnable()
    {
        if (OnGame.buffLib.bufObjs[index] != null)
            obj = Object.Instantiate(OnGame.buffLib.bufObjs[index], character.transform);
    }
    //只有脚本添加时候调用
    public virtual void OnBuffAdd()
    {
        OnScene.Report("<color=#7CC0D2>[" + character.data.Name + "]</color>获得了状态:<color=#F16B08>【" + Name + "】</color>");
        OnBuffEnable();
    }
    //消除显示，清除和修改、删除都调用
    public virtual void OnBuffDisable()
    {
        if (obj != null)
            Object.Destroy(obj);
    }
    //只有脚本清除时候调用
    public virtual void OnBuffRemove()
    {
        OnBuffDisable();
    }

    public virtual void OnRoundEnd()
    {
        if (continueTime > 0)
        {
            continueTime--;
            if (continueTime <= 0)
            {
                character.BuffEnd(this);
            }
        }
    }

    public virtual void OnRoundStart()
    {
        
    }
    public virtual Sprite Icon
    {
        get { return OnGame.buffLib.icons[index]; }
    }
    public virtual GameObject buffGameObject
    {
        get { return OnGame.buffLib.bufObjs[index]; }
    }
}

//用于给角色显示天赋的Buff
public class OccupationBranchBuff : Buff
{
    string info;
    public OccupationBranchBuff(CharacterScript cs) : base(0,cs,-1,-1)
    {
        index = -1;
        string[] temp = Occupation.GetOcccupationBranchData(cs.data.OccupationData.type,cs.data.OccupationData.branch).Split('\n');
        Name = temp[0].Substring(1, temp[0].Length - 2);
        info = temp[1];
    }
    public OccupationBranchBuff(Occupation occupation) : base(0, null, -1, -1)
    {
        index = -1;
        string[] temp = Occupation.GetOcccupationBranchData(occupation.type, occupation.branch).Split('\n');
        Name = temp[0].Substring(1, temp[0].Length - 2);
        info = temp[1];
    }
    public override GameObject buffGameObject => null;
    public override Sprite Icon => OnGame.buffLib.OccupationBranchIcon;
    public override string Info() {return info;}
    public override void OnBuffAdd() { }
    public override void OnBuffEnable() { }
    public override void OnBuffDisable() { }
    public override void OnBuffRemove() { }
    public override void OnRoundStart() { }
    public override void OnRoundEnd() { }
}

//BUFF_0: 燃烧
public class BUFF_0 : Buff
{
    public BUFF_0(CharacterScript cs, int time, float val) : base(0, cs, time, val) { }
    public override string Info()
    {
        return "该目标着火了，每回合开始时受到"+value.ToString()+"%最大生命值的伤害";
    }
    public override void OnRoundStart()
    {
        int damage = (int)(value * character.data.BaseData.MaxHP / 100);
        OnScene.Report("<color=#CC44EE>[负面效果]: </color><color=Orange>[" + character.data.Name + "]</color>受到了<color=#FF1111><b>"
                                + damage.ToString() + "</b></color>点火焰伤害。");
        character.HPGetDamage(damage);
        base.OnRoundStart();
    }
}

//BUFF_1: 冰冻
public class BUFF_1 : Buff
{
    public BUFF_1(CharacterScript cs, int time, float val) : base(1, cs, time, val) { }
    public override string Info()
    {
        return "该目标被冻结，每回合增加的行动点数减少一半";
    }
    public override void OnRoundStart()
    {
        OnScene.Report("<color=#CC44EE>[负面效果]: </color><color=Orange>[" + character.data.Name + "]</color>正受到寒冰的严重阻碍。");
        base.OnRoundStart();
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        obj.GetComponent<IceBuff>().Hold();
    }
}

//BUFF_2: 禁锢
public class BUFF_2 : Buff
{
    public BUFF_2(CharacterScript cs, int time, float val) : base(2, cs, time, val) { }
    public override string Info()
    {
        return "该目标暂时无法移动自身，受到伤害可解除此效果";
    }
    public override void OnRoundStart()
    {
        OnScene.Report("<color=#CC44EE>[负面效果]: </color><color=Orange>[" + character.data.Name + "]</color>正处于被禁锢状态。");
        base.OnRoundStart();
    }
}

//BUFF_3: 晕眩
public class BUFF_3 : Buff
{
    public BUFF_3(CharacterScript cs, int time, float val) : base(3, cs, time, val) { }
    public override string Info()
    {
        return "该目标晕头转向，因此在检定时受到%50敏捷的惩罚";
    }
}

//BUFF_4: 死灵
public class BUFF_4 : Buff
{
    public BUFF_4(CharacterScript cs, int time, float val) : base(4, cs, time, val) { }
    public override string Info()
    {
        return "对该目标的治疗效果转而造成等量的伤害,在战斗结束时，若死灵的力量强于主人，则会攻击主人所在的阵营。";
    }
    public override void OnBuffAdd()
    {
        //死灵是默认属性，不写获得提示文字
    }
}

//BUFF_5: 潜行
public class BUFF_5 : Buff
{
    public BUFF_5(CharacterScript cs, int time, float val) : base(5, cs, time, val) { }
    public override string Info()
    {
        return "该目标处于潜行状态，可由发动攻击、使用物品和受到伤害动作解除";
    }
}

//BUFF_6: 保护
public class BUFF_6 : Buff
{
    public BUFF_6(CharacterScript cs, int time, float val) : base(6, cs, time, val) { }
    public override string Info()
    {
        return "该目标受到保护，生命值无法降至1以下";
    }
}

//BUFF_7: 沉默
public class BUFF_7 : Buff
{
    public BUFF_7(CharacterScript cs, int time, float val) : base(7, cs, time, val) { }
    public override string Info()
    {
        return "该目标被封闭了施法能力，在回合开始前进行一次感知检定，若失败则不可使用技能";
    }
}

//BUFF_8: 麻木
public class BUFF_8 : Buff
{
    public BUFF_8(CharacterScript cs, int time, float val) : base(8, cs, time, val) { }
    public override string Info()
    {
        return "该目标的身体麻木，暂时无法使用手中的武器攻击";
    }
}

//BUFF_9: 混乱
public class BUFF_9 : Buff
{
    public BUFF_9(CharacterScript cs, int time, float val) : base(9, cs, time, val) { }
    public override string Info()
    {
        return "该目标的感知被扭曲了，每回合会选择最近的目标攻击";
    }
}

//BUFF_10: 神佑
public class BUFF_10 : Buff
{
    public BUFF_10(CharacterScript cs, int time, float val) : base(10, cs, time, val) { }
    public override string Info()
    {
        return "该目标被神注视着，因此在进行检定和对抗时获得额外"+value.ToString()+"点加值";
    }
}

//BUFF_11: 目盲
public class BUFF_11 : Buff
{
    public BUFF_11(CharacterScript cs, int time, float val) : base(11, cs, time, val) { }
    public override string Info()
    {
        return "该目标只能看清自己身边的东西，无法使用远程法术和远程攻击";
    }
}

//BUFF_12: 狂暴
public class BUFF_12 : Buff
{
    int value2;
    public BUFF_12(CharacterScript cs, int time, float val) : base(12, cs, time, val) { value2 = time * 2; }
    public override string Info()
    {
        return "该目标进入了狂暴状态，其ATK获得"+value.ToString()+"点临时加值，DEF获得"+value2.ToString()+"点临时加值，但只能使用近战普通攻击";
    }
}

//BUFF_13: 凋零
public class BUFF_13 : Buff
{
    public BUFF_13(CharacterScript cs, int time, float val) : base(13, cs, time, val) { }
    public override string Info()
    {
        return "该目标的生命力正在散失，在回合开始时损失1d4的生命值";
    }
    public override void OnRoundStart()
    {
        RollPanel rollPanel = OnScene.manager.OpenRollPanel();
        rollPanel.SetUp("凋零:伤害效果", (i) =>
        {
            OnScene.Report("<color=#CC44EE>[负面效果]: </color><color=Orange>[" + character.data.Name + "]</color>的生命流失了<color=#336633><b>"
                                + i.ToString() + "</b></color>点。");
            character.HPGetDamage(i);
        });
        base.OnRoundStart();
    }
}

//BUFF_14: 禁足
public class BUFF_14 : Buff
{
    public BUFF_14(CharacterScript cs, int time, float val) : base(14, cs, time, val) { }
    public override string Info()
    {
        return "该目标无法移动";
    }
    public override void OnRoundStart()
    {
        OnScene.Report("<color=#CC44EE>[负面效果]: </color><color=Orange>[" + character.data.Name + "]</color>正受到藤蔓的严重阻碍。");
        base.OnRoundStart();
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        obj.GetComponent<VineBuff>().Hold();
    }
}

//BUFF_15: 魅惑
public class BUFF_15 : Buff
{
    public BUFF_15(CharacterScript cs, int time, float val) : base(15, cs, time, val) { }
    public override string Info()
    {
        return "该目标受到了魅惑，因此改变了自己的立场";
    }
}

//BUFF_16: 火焰结界
public class BUFF_16 : Buff
{
    public BUFF_16(CharacterScript cs, int time, float val) : base(16, cs, time, val) { }
    public override string Info()
    {
        return "该目标被火焰保护，受到近战攻击时，火焰会对接触到的目标造成"+value.ToString()+"d4点伤害";
    }
}

//BUFF_17: 火焰缭绕
public class BUFF_17 : Buff
{
    public BUFF_17(CharacterScript cs, int time, float val) : base(17, cs, time, val) { }
    public override string Info()
    {
        return "该目标被火焰缭绕，火属性法术伤害加深"+value.ToString()+"%";
    }
}

//BUFF_18: 清洗伤口
public class BUFF_18 : Buff
{
    public BUFF_18(CharacterScript cs, int time, float val) : base(18, cs, time, val) { }
    public override string Info()
    {
        return "该目标的伤口正在恢复（每回合开始时恢复1d8+"+value.ToString()+"点）";
    }
    public override void OnRoundStart()
    {
        RollPanel rollPanel = OnScene.manager.OpenRollPanel();
        rollPanel.SetUp("清洗伤口:治疗效果", (i) =>
        {
            OnScene.Report("<color=#CC44EE>[状态效果]: </color><color=Orange>[" + character.data.Name + "]</color>的生命恢复了<color=#66CCCC><b>"
                                + i.ToString() + "</b></color>点。");
            character.HPGetHeal(i);
        });
        base.OnRoundStart();
    }
}

//BUFF_19: 魔法屏障
public class BUFF_19 : Buff
{
    public BUFF_19(CharacterScript cs, int time, float val) : base(19, cs, time, val) { }
    public override string Info()
    {
        return "该目标正在受到魔法的保护,在受到来自魔法的伤害时，该屏障可抵御"+value.ToString()+"d4伤害";   
    }
}

//BUFF_20: 风之轻灵
public class BUFF_20 : Buff
{
    public BUFF_20(CharacterScript cs, int time, float val) : base(20, cs, time, val) { }
    public override string Info()
    {
        return "该目标受到了风的祝福，敏捷增加"+((int)value).ToString()+"点";
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        character.DEX += (int)value;
    }
    public override void OnBuffDisable()
    {
        base.OnBuffDisable();
        character.DEX -= (int)value;
    }
}

//BUFF_21: 羽落术
public class BUFF_21 : Buff
{
    public BUFF_21(CharacterScript cs, int time, float val) : base(21, cs, time, val) { }
    public override string Info()
    {
        return "该目标变得十分轻盈，能够跳下"+(value+1).ToString()+"米内的高台，落地后失效。";
    }
}

//BUFF_22: 多重攻击
public class BUFF_22 : Buff
{
    public BUFF_22(CharacterScript cs, int time, float val) : base(22, cs, time, val) { }
    public override string Info()
    {
        return "该目标每次攻击可以选择"+value.ToString()+"个目标";
    }
}

//BUFF_23: 牛之蛮力
public class BUFF_23 : Buff
{
    public BUFF_23(CharacterScript cs, int time, float val) : base(23, cs, time, val) { }
    public override string Info()
    {
        return "该目标变得像牛一般强壮，STR获得" + ((int)value).ToString() + "点加值";
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        character.STR += (int)value;
        if (character.data.OccupationData.type == OccupationType.Druid && character.data.OccupationData.branch == 1)
        {
            character.STR += 2;
            character.CON += 2;
        }
    }
    public override void OnBuffDisable()
    {
        base.OnBuffDisable();
        character.STR -= (int)value;
        if (character.data.OccupationData.type == OccupationType.Druid && character.data.OccupationData.branch == 1)
        {
            character.STR -= 2;
            character.CON -= 2;
        }
    }
}

//BUFF_24: 熊之坚韧
public class BUFF_24 : Buff
{
    public BUFF_24(CharacterScript cs, int time, float val) : base(24, cs, time, val) { }
    public override string Info()
    {
        return "该目标变得像熊一般坚韧，CON获得" + ((int)value).ToString() + "点加值";
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        character.CON += (int)value;
        if (character.data.OccupationData.type == OccupationType.Druid && character.data.OccupationData.branch == 1)
        {
            character.STR += 2;
            character.CON += 2;
        }
    }
    public override void OnBuffDisable()
    {
        base.OnBuffDisable();
        character.CON -= (int)value;
        if (character.data.OccupationData.type == OccupationType.Druid && character.data.OccupationData.branch == 1)
        {
            character.STR -= 2;
            character.CON -= 2;
        }
    }
}

//BUFF_25: 追猎印记
public class BUFF_25 : Buff
{
    public BUFF_25(CharacterScript cs, int time, float val) : base(25, cs, time, val) { }
    public override string Info()
    {
        return "该目标被标记了，标记的主人对其造成的伤害增加" + value.ToString() + "%";
    }
}

//BUFF_26: 防护邪恶
public class BUFF_26 : Buff
{
    public BUFF_26(CharacterScript cs, int time, float val) : base(26, cs, time, val) { }
    public override string Info()
    {
        return "该目标受到秩序善良阵营的保护，受到邪恶阵营的伤害减少" + value.ToString() + "%";
    }
}

//BUFF_27: 防护善良
public class BUFF_27 : Buff
{
    public BUFF_27(CharacterScript cs, int time, float val) : base(27, cs, time, val) { }
    public override string Info()
    {
        return "该目标受到邪恶阵营的保护，受到善良阵营的伤害减少" + value.ToString() + "%";
    }
}

//BUFF_28: 遭受审判
public class BUFF_28 : Buff
{
    public BUFF_28(CharacterScript cs, int time, float val) : base(28, cs, time, val) { }
    public override string Info()
    {
        return "该目标被指控为有罪，受到守序阵营的伤害增加" + value.ToString() + "%";
    }
}

//BUFF_29: 律令随行
public class BUFF_29 : Buff
{
    public BUFF_29(CharacterScript cs, int time, float val) : base(29, cs, time, val) { }
    public override string Info()
    {
        return "该目标被附加了一项禁令，当目标触犯律令时，对目标造成"+value.ToString()+"d4点伤害";
    }
}

//BUFF_30: 神圣领域
public class BUFF_30 : Buff
{
    public BUFF_30(CharacterScript cs, int time, float val) : base(30, cs, time, val) { }
    public override string Info()
    {
        return "光明与秩序的信念，铸就此间的地上神国";
    }
    public override void OnBuffAdd()
    {
        OnBuffEnable();
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        obj.GetComponent<SaintBuff>().SetUp(continueTime * 4 *OnGame.WorldScale);
        OnRoundStart();
    }
    public override void OnRoundStart()
    {
        base.OnRoundStart();
        obj.GetComponent<SaintBuff>().PlaySound();
        foreach (var cs in MapSet.nowCreator.characters)
        {
            if (RangeCharacterSelector.CheckDistance(character.transform.position, cs.transform.position, OnGame.WorldScale * continueTime * 3 / 2))
            {
                switch (cs.data.alignmentType)
                {
                    case AlignmentType.LawfulGood:
                        cs.AddBuff("神圣之灵", 1, (int)value * 2);
                        break;
                    case AlignmentType.NeutralGood:
                    case AlignmentType.LawfulNeutral:
                        cs.AddBuff("神圣之灵", 1, (int)value);
                        break;
                    case AlignmentType.NeutralEvil:
                    case AlignmentType.ChaoticNeutral:
                        cs.AddBuff("天堂之火", 1, (int)value);
                        break;
                    case AlignmentType.ChaoticEvil:
                        cs.AddBuff("天堂之火", 1, (int)value * 2);
                        break;
                    default:break;
                }
            }
        }
    }
}

//BUFF_31: 灵魂链接
public class BUFF_31 : Buff
{
    static List<CharacterScript> characters = new List<CharacterScript>();
    public BUFF_31(CharacterScript cs, int time, float val) : base(31, cs, time, val) { }
    public override string Info()
    {
        return "该目标的灵魂被链接了，当被链接的一个目标受到伤害时，对所有其余目标造成相同的伤害";
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        characters.Add(character);
    }
    public override void OnBuffDisable()
    {
        base.OnBuffDisable();
        if(characters.Contains(character))
            characters.Remove(character);
    }
    public void OnDamage(CharacterScript cs, int damage)
    {
        foreach (var i in characters)
        {
            if (i != null && i != cs)
            {
                OnScene.Report("<color=#CC44EE>[负面效果]: </color><color=Orange>[" + i.data.Name + "]</color>受到了来自<color=Yellow>["
                                + character.data.Name + "]</color>的<color=#33DD33>[灵魂链接]</color>效果的<color=#FF1111><b>"
                                + damage.ToString() + "</b></color>点伤害。");
                //灵魂链接触发的伤害不能触发灵魂链接
                i.HPGetDamage(damage,false);
            }
        }
    }

}

//BUFF_32: 力量祝福
public class BUFF_32 : Buff
{
    public BUFF_32(CharacterScript cs, int time, float val) : base(32, cs, time, val) { }
    public override string Info()
    {
        return "该角色的武器闪耀着光辉，在角色造成伤害后，额外对目标造成" + value.ToString() + "%的伤害";
    }
    public void CauseDamage(CharacterScript enemy, float damage)
    {
        int HPdamage = (int)(damage * value / 100);
        OnScene.Report("<color=#CC44EE>[状态效果]: </color><color=#FF3333>[" + enemy.data.Name + "]</color>受到了来自力量祝福的<color=#FF1111><b>"
            + HPdamage.ToString() + "</b></color>点伤害。");
        enemy.HPGetDamage(HPdamage);
    }
}

//BUFF_33: 神圣护甲
public class BUFF_33 : Buff
{
    public BUFF_33(CharacterScript cs, int time, float val) : base(33, cs, time, val) { }
    public override string Info()
    {
        return "该目标被圣光环绕着，受到的伤害减少" + value.ToString() + "%";
    }
}

//BUFF_34: 暗夜薄纱
public class BUFF_34 : Buff
{
    public BUFF_34(CharacterScript cs, int time, float val) : base(34, cs, time, val) { }
    public override string Info()
    {
        return "该目标被阴影环绕，受到的近战伤害减少" + value.ToString() + "点";
    }
}

//BUFF_35: 暗影化身
public class BUFF_35 : Buff
{
    public BUFF_35(CharacterScript cs, int time, float val) : base(35, cs, time, val) { }
    public override string Info()
    {
        return "该目标被一团阴影包裹，免疫物理伤害，将受到的"+value.ToString()+"%伤害转化为MP";
    }
    public void DamageToMP(float damage)
    {
        int MPheal = (int)(damage * value / 100);
        OnScene.Report("<color=#CC44EE>[状态效果]: </color><color=#33FF33>[" + character.data.Name + "]</color>通过暗影形态受到伤害恢复了<color=#9999FF><b>" 
            + MPheal.ToString() + "</b></color>点MP。");
        character.MPGetHeal(MPheal);
    }
}

//BUFF_36: 黑暗根须
public class BUFF_36 : Buff
{
    public BUFF_36(CharacterScript cs, int time, float val) : base(36, cs, time, val) { }
    bool isHeal = false;
    public override string Info()
    {
        if(isHeal)
            return "该目标被根须缠绕,无法移动和攻击,每回合开始时恢复" + value.ToString() + "%的HP和MP,受到超过10点攻击或体质检定可取消此效果。";
        else
            return "该目标被根须缠绕,无法移动和攻击,受到超过10点攻击或体质检定可取消此效果。";

    }
    public override void OnRoundStart()
    {
        base.OnRoundStart();
        if (isHeal)
        {
            int HPheal = (int)(character.data.BaseData.MaxHP * value / 100);
            int MPheal = (int)(character.data.BaseData.MaxMP * value / 100);
            character.HPGetHeal(HPheal);
            character.MPGetHeal(MPheal);
            OnScene.Report("<color=#CC44EE>[状态效果]: </color><color=Orange>[" + character.data.Name + "]</color>恢复了<color=#99FF99><b>"
            + HPheal.ToString() + "</b></color>点HP和<color=#9999FF><b>" + MPheal.ToString() + "</b></color>点MP。");
        }
        OnScene.Report("<color=#CC44EE>[状态效果]: </color><color=Orange>[" + character.data.Name + "]</color>正处于被缠绕状态。");
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        switch (character.data.alignmentType)
        {
            case AlignmentType.ChaoticEvil:
            case AlignmentType.LawfulEvil:
            case AlignmentType.NeutralEvil:
                isHeal = true;break;
            default:break;
        }
        obj.GetComponent<VineBuff>().Hold();
    }
}

//BUFF_37: 生命祝福
public class BUFF_37 : Buff
{
    public BUFF_37(CharacterScript cs, int time, float val) : base(37, cs, time, val) { }
    public override string Info()
    {
        return "该目标的武器被祝福过，在攻击时产生"+value.ToString()+"%的生命回复效果";
    }
    public void CauseDamage(CharacterScript enemy, float damage)
    {
        int HPheal = (int)(damage * value / 100);
        OnScene.Report("<color=#CC44EE>[状态效果]: </color><color=#33FF33>[" + character.data.Name + "]</color>通过生命祝福恢复了<color=#99FF99><b>"
            + HPheal.ToString() + "</b></color>点HP。");
        character.HPGetHeal(HPheal);
    }
}

//BUFF_38: 骨甲
public class BUFF_38 : Buff
{
    public BUFF_38(CharacterScript cs, int time, float val) : base(38, cs, time, val) { }
    public override string Info()
    {
        return "该目标装备骨甲，能在受到攻击时抵消"+value.ToString()+"点伤害，同时DEX和CHA减少2点";
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        character.DEX -= 2;
        character.CHA -= 2;
    }
    public override void OnBuffDisable()
    {
        base.OnBuffDisable();
        character.DEX += 2;
        character.CHA += 2;
    }
    public int GetDefence(int damage)
    {
        int val = (int)value;
        val = damage > val ? val : damage;
        OnScene.Report("<color=#CC44EE>[状态效果]: </color><color=#33FF33>[" + character.data.Name + "]</color>身上的骨甲抵消了<color=#33FF33><b>"
            + val.ToString() + "</b></color>点伤害。");
        return val;
    }

}

//BUFF_39: 法力护甲
public class BUFF_39 : Buff
{
    public BUFF_39(CharacterScript cs, int time, float val) : base(39, cs, time, val) { }
    public override string Info()
    {
        return "该目标装备法力护甲，能在受到攻击时抵消" + value.ToString() + "点伤害";
    }
    public int GetDefence(int damage)
    {
        int val = (int)value;
        val = damage > val ? val : damage;
        OnScene.Report("<color=#CC44EE>[状态效果]: </color><color=#33FF33>[" + character.data.Name + "]</color>身上的法力护甲抵消了<color=#9999FF><b>"
            + val.ToString() + "</b></color>点伤害。");
        return val;
    }
}

//BUFF_40: 好梦
public class BUFF_40 : Buff
{
    public BUFF_40(CharacterScript cs, int time, float val) : base(40, cs, time, val) { }
    public override string Info()
    {
        return "该目标正处于甜蜜梦乡";
    }
    public override void OnRoundStart()
    {
        OnScene.Report("<color=#CC44EE>[负面效果]: </color><color=Orange>[" + character.data.Name + "]</color>由于深陷睡梦而无法参与战斗。");
        base.OnRoundStart();
    }
}

//BUFF_41: 噩梦
public class BUFF_41 : Buff
{
    public BUFF_41(CharacterScript cs, int time, float val) : base(41, cs, time, val) { }
    public override string Info()
    {
        return "该目标经历了一次噩梦，全属性获得1点临时减值";
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        character.STR -= 1;
        character.DEX -= 1;
        character.CON -= 1;
        character.INT -= 1;
        character.WIS -= 1;
        character.CHA -= 1;
    }
    public override void OnBuffDisable()
    {
        base.OnBuffDisable();
        character.STR += 1;
        character.DEX += 1;
        character.CON += 1;
        character.INT += 1;
        character.WIS += 1;
        character.CHA += 1;
    }
}

//BUFF_42: 灵吸之梦
public class BUFF_42 : Buff
{
    public BUFF_42(CharacterScript cs, int time, float val) : base(42, cs, time, val) { }
    public override string Info()
    {
        return "该目标在梦境中消耗了法力，每回合开始时MP受到1d10的减损";
    }
    public override void OnRoundStart()
    {
        base.OnRoundStart();
        RollPanel rollPanel = OnScene.manager.OpenRollPanel();
        rollPanel.SetUp("灵吸之梦:MP流失", (i) =>
        {
            character.MPGetDamage(i);
            OnScene.Report("<color=#CC44EE>[负面效果]: </color><color=#33FF33>[" 
                + character.data.Name + "]</color>由于灵吸之梦损失了<color=#EE99FF><b>" 
                + i.ToString() + "</b></color>点MP。");
        }, 10, 1);
    }
}

//BUFF_43: 巨龙之梦
public class BUFF_43 : Buff
{
    public BUFF_43(CharacterScript cs, int time, float val) : base(43, cs, time, val) { }
    public override string Info()
    {
        return "在梦境里，该目标获得了巨龙的传承，全属性增加3点";
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        character.STR += 3;
        character.DEX += 3;
        character.CON += 3;
        character.INT += 3;
        character.WIS += 3;
        character.CHA += 3;
    }
    public override void OnBuffDisable()
    {
        base.OnBuffDisable();
        character.STR -= 3;
        character.DEX -= 3;
        character.CON -= 3;
        character.INT -= 3;
        character.WIS -= 3;
        character.CHA -= 3;
    }
}

//BUFF_44: 异变之种
public class BUFF_44 : Buff
{
    public BUFF_44(CharacterScript cs, int time, float val) : base(44, cs, time, val) { }
    public override string Info()
    {
        return "当目标受到治疗时，转而对其造成等量的伤害。";
    }
}

//BUFF_45: 碎甲
public class BUFF_45 : Buff
{
    public BUFF_45(CharacterScript cs, int time, float val) : base(45, cs, time, val) { }
    public override string Info()
    {
        return "该目标的护甲损坏了，受到的伤害额外增加"+value.ToString()+"%";
    }
}

//BUFF_46: 恐惧邪能
public class BUFF_46 : Buff
{
    public BUFF_46(CharacterScript cs, int time, float val) : base(46, cs, time, val) { }
    public override string Info()
    {
        return "该目标被恐惧邪能附体，近战造成的伤害获得"+ ((int)value).ToString()+"点额外加值，在每回合开始时受到"+ ((int)(value/2)).ToString() + "点伤害";
    }
    public override void OnRoundStart()
    {
        int damage = (int)(value/2);
        OnScene.Report("<color=#CC44EE>[负面效果]: </color><color=Orange>[" + character.data.Name + "]</color>受到了<color=#FF1111><b>"
                                + damage.ToString() + "</b></color>点来自邪能之火的伤害。");
        character.HPGetDamage(damage);
        base.OnRoundStart();
    }
}


//BUFF_47: 荆棘丛生
public class BUFF_47 : Buff
{
    public BUFF_47(CharacterScript cs, int time, float val) : base(47, cs, time, val) { }
    public override string Info()
    {
        return "该目标身处荆棘之中，移动速度减少了" + value.ToString() + "%";
    }
}

//BUFF_48: 魔法免疫
public class BUFF_48 : Buff
{
    public BUFF_48(CharacterScript cs, int time, float val) : base(48, cs, time, val) { }
    public override string Info()
    {
        return "该目标来自魔法造成的伤害拥有" + value.ToString() + "%的抗性。";
    }
}

//BUFF_49: 神圣之灵
public class BUFF_49 : Buff
{
    public BUFF_49(CharacterScript cs, int time, float val) : base(49, cs, time, val) { }
    public override string Info()
    {
        return "该目标受到神圣领域的影响而获得全属性" + ((int)value).ToString() + "点的临时强化";
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        character.STR += (int)value;
        character.DEX += (int)value;
        character.CON += (int)value;
        character.INT += (int)value;
        character.WIS += (int)value;
        character.CHA += (int)value;
    }
    public override void OnBuffDisable()
    {
        base.OnBuffDisable();
        character.STR -= (int)value;
        character.DEX -= (int)value;
        character.CON -= (int)value;
        character.INT -= (int)value;
        character.WIS -= (int)value;
        character.CHA -= (int)value;
    }
}

//BUFF_50: 天堂之火
public class BUFF_50 : Buff
{
    public BUFF_50(CharacterScript cs, int time, float val) : base(50, cs, time, val) { }
    public override string Info()
    {
        return "该目标受到神圣领域的影响而获得全属性" + ((int)value).ToString() + "点的临时减值";
    }
    public override void OnBuffEnable()
    {
        base.OnBuffEnable();
        character.STR -= (int)value;
        character.DEX -= (int)value;
        character.CON -= (int)value;
        character.INT -= (int)value;
        character.WIS -= (int)value;
        character.CHA -= (int)value;
    }
    public override void OnBuffDisable()
    {
        base.OnBuffDisable();
        character.STR += (int)value;
        character.DEX += (int)value;
        character.CON += (int)value;
        character.INT += (int)value;
        character.WIS += (int)value;
        character.CHA += (int)value;
    }
}
//BUFF_51: 梦境
public class BUFF_51 : Buff
{
    public BUFF_51(CharacterScript cs, int time, float val) : base(51, cs, time, val) { }
    public override string Info()
    {
        return "该目标正在受到梦境的影响";
    }
    public override void OnBuffAdd()
    {
        base.OnBuffAdd();
        string bufName;
        switch ((int)value)
        {
            case 1:bufName = "好梦";break;
            case 2:bufName = "噩梦"; break;
            case 3:bufName = "灵吸之梦"; break;
            case 4:bufName = "巨龙之梦";break;
            default: bufName = "好梦";break;
        }
        character.AddBuff(bufName, continueTime, 1);
    }
}


