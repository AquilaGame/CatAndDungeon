using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EPOOutline;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterScript : MonoBehaviour
{
    [SerializeField] List<AudioClip> BirthSound = new List<AudioClip>();
    [SerializeField] List<AudioClip> WalkSound = new List<AudioClip>();
    [SerializeField] List<AudioClip> AttackSound = new List<AudioClip>();
    [SerializeField] List<AudioClip> DeathSound = new List<AudioClip>();
    [HideInInspector]public NavMeshAgent NavAgent;
    [HideInInspector]public Outlinable outline = null;
    public Character data = null;
    [HideInInspector]public CharacterCanvas canvas = null;
    public float HeroSeatOffset = 0.0015f;
    GameObject attackRangeGameObj = null;
    Quaternion defaultQuaternion;
    [HideInInspector] public CharacterScript Enemy = null;
    [HideInInspector] public CharacterAnimCtrl animCtrl = null;
    [HideInInspector] public CharacterPanel dataPnl = null;
    [HideInInspector] public bool isOperating = false;
    [SerializeField] Transform HeroSeat = null;
    private void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        outline = GetComponent<Outlinable>();
        if (HeroSeat != null)
            HeroSeat.Translate(new Vector3(0, HeroSeatOffset, 0));
        else
            transform.Translate(new Vector3(0, HeroSeatOffset, 0));
    }
    // Update is called once per frame
    void Update()
    {
        if(canvas!=null)
            canvas.transform.rotation = defaultQuaternion;
    }
    public void RemoveThis()
    {
        OnScene.manager.RemoveCharacter(this);
    }
    public void SetMove(Vector3 pos, float distance)
    {
        NavAgent.destination = pos;
        //NavAgent.speed = 20 * MoveSpeedMulty;
        data.AP -= data.WalkAPCost(distance);
        if (OnScene.onSelect.character == this)
            dataPnl.FlushAP();
    }
    public void SetCanvas(CharacterCanvas _canvas)
    {
        canvas = _canvas;
        canvas.GetComponent<Canvas>().worldCamera = OnScene.mainCam.cam;
        defaultQuaternion = Quaternion.Euler(new Vector3(45.0f,0,0));
    }
    public void SetUp(Character character)
    {
        data = character;
        gameObject.name = data.Name;
        animCtrl = GetComponent<CharacterAnimCtrl>();
        data.buffs.Add(new OccupationBranchBuff(this));
        foreach (Buff buff in data.buffs)
        {
            buff.character = this;
            buff.OnBuffEnable();
        }
    }
    public void ResetSelect()
    {
        outline.enabled = false;
    }
    public void SetSelect()
    {
        outline.enabled = true;
        if (data.BaseData.HP > 0)
            PlaySound(CharacterSoundType.Walk);
    }

    public void PlaySound(CharacterSoundType type)
    {
        List<AudioClip> nowPlay;
        switch (type)
        {
            case CharacterSoundType.Attack:
                nowPlay = AttackSound;
                break;
            case CharacterSoundType.Birth:
                nowPlay = BirthSound;
                break;
            case CharacterSoundType.Death:
                nowPlay = DeathSound;
                break;
            case CharacterSoundType.Walk:
                nowPlay = WalkSound;
                break;
            default: return;
        }
        if (nowPlay.Count == 0) return;
        Sound.Play(nowPlay[Random.Range(0, nowPlay.Count)],this);
    }




    public void OnClickAttackBtn()
    {
        if (data.AP < Character.GetAPCost(2.0f))
        {
            OnScene.Report("<color=#55FFAA>[行动]: 由于行动力不足，</color><color=Orange>["
                + data.Name + "]</color>发起的攻击动作失败。");
            return;
        }
        OnScene.manager.CharacterSelector.StartCatch(this, SkillStartScript.Filter.Enemy, ProcessAttack, data.AttackRange, 1);
    }

    public void ProcessAttack(List <CharacterScript> css, Vector3 noUse)
    {
        if (css.Count != 0)
        {
            StartAttack(css[0]);
        }
    }
    public void StartAttack(CharacterScript cs)
    {
        if (isHaveBuff("潜行", out Buff buff))
        {
            BuffEnd(buff);
        }
        Enemy = cs;
        if (Enemy.data.BaseData.HP > 0)
        {
            CombatPanel combatPanel = OnScene.manager.OpenCombatPanel();
            combatPanel.SetUp("普通攻击：命中", data.Name, Enemy.data.Name, data.BaseData.DEX, Enemy.data.BaseData.DEX, AfterCheckHit);
        }
        //如果敌人都躺地上了那就不用判定了，取10就行了
        else
        {
            AfterCheckHit(10);
        }
        PlaySound(CharacterSoundType.Attack);
    }
    public void AfterCheckHit(int value)
    {
        
        if (value > 0)
        {
            CombatPanel combatPanel = OnScene.manager.OpenCombatPanel();
            combatPanel.SetUp("普通攻击：伤害", data.Name, Enemy.data.Name, data.NormalATK, Enemy.data.NormalDEF, AfterCalcNormalAttack);
        }
        else
        {
            OnScene.Report("<color=#CC44EE>[攻击结算]: </color><color=Orange>[" + data.Name + "]</color>攻击了<color=Red>["
                                + Enemy.data.Name + "]</color>，但攻击没有命中");
            animCtrl.Attack(Enemy,0);
            Enemy = null;
        }
        data.AP -= Character.GetAPCost(2.0f);
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushAP();
        }
    }

    public void AfterCalcNormalAttack(int value)
    {
        int damage;
        if (value > 0)
        {
            OnScene.Report("<color=#CC44EE>[攻击结算]: </color><color=Orange>[" + data.Name + "]</color>攻击了<color=Red>["
                                + Enemy.data.Name + "]</color>，造成了<color=#FF1111><b>"
                                + value.ToString() + "</b></color>点伤害。");
            damage = value;
        }
        else
        {
            OnScene.Report("<color=#CC44EE>[攻击结算]: </color><color=Orange>[" + data.Name + "]</color>攻击了<color=Red>["
                                + Enemy.data.Name + "]</color>，造成了轻微的伤害（<color=#FF1111><b>1</b></color>点）。");
            damage = 1;
        }
        
        animCtrl.Attack(Enemy,damage);
    }
    public virtual void AfterCauseDamage(CharacterScript enemy, int damage)
    {
        Buff buff = null;
        if (isHaveBuff("力量祝福", out buff))
        {
            ((BUFF_32)buff).CauseDamage(enemy, damage);
        }
        if (isHaveBuff("生命祝福", out buff))
        {
            ((BUFF_37)buff).CauseDamage(enemy, damage);
        }
        if (data.OccupationData.type == OccupationType.Darknight)
        {
            if (data.OccupationData.branch == 0)
            {
                int HPdamage = (int)(damage * 0.15);
                OnScene.Report("<color=#CC44EE>[触发天赋]: </color><color=#FF3333>[" + enemy.data.Name + "]</color>受到了来自攻击者天赋的<color=#FF1111><b>"
                    + HPdamage.ToString() + "</b></color>点伤害。");
                enemy.HPGetDamage(HPdamage);
            }
            else
            {
                int HPheal = (int)(damage * 0.15);
                if (HPheal <= 0)
                    HPheal = 1;
                OnScene.Report("<color=#CC44EE>[触发天赋]: </color><color=#FF3333>[" + data.Name + "]</color>受到了来自天赋的<color=#99FF99><b>"
                    + HPheal.ToString() + "</b></color>点治疗。");
                HPGetHeal(HPheal);
            }
        }
        if (data.OccupationData.type == OccupationType.Rogue && data.OccupationData.branch == 0)
        {
            int MPheal = (int)(data.BaseData.MaxMP * 0.05);
            if (MPheal <= 0)
                MPheal = 1;
            OnScene.Report("<color=#CC44EE>[触发天赋]: </color><color=#FF3333>[" + data.Name + "]</color>受到了来自天赋的<color=#9999FF><b>"
                + MPheal.ToString() + "</b></color>点MP回复。");
            MPGetHeal(MPheal);
        }
    }
    public virtual int GetRealDamage(int damage)
    {
        Buff buff = null;
        if(isHaveBuff("骨甲",out buff))
        {
            damage -= ((BUFF_38)buff).GetDefence(damage);
        }
        if (isHaveBuff("法力护甲", out buff))
        {
            damage -= ((BUFF_39)buff).GetDefence(damage);
        }
        return damage;
    }
    public virtual void AfterGetDamage(int damage)
    {
        Buff buff = null;
        if (isHaveBuff("潜行", out buff))
        {
            BuffEnd(buff);
        }
        if (isHaveBuff("禁锢", out buff))
        {
            BuffEnd(buff);
        }
        if (damage > 10 && isHaveBuff("黑暗根须", out buff))
        {
            BuffEnd(buff);
        }
        if (isHaveBuff("灵魂链接", out buff))
        {
            ((BUFF_31)buff).OnDamage(this, damage);
        }
        if(isHaveBuff("暗影化身",out buff))
        {
            ((BUFF_35)buff).DamageToMP(damage);
        }
    }
    public void StartAttackRangePreview()
    {

        OnScene.manager.CharacterSelector.SetRangePreview(this, data.AttackRange * 2);
    }
    public void EndAttackRangePreview()
    {
        OnScene.manager.CharacterSelector.ResetRangePreview();
    }

    public void AddEXP(int val)
    {
        OnScene.Report("<color=#CC44EE>[获得经验]: </color><color=Orange>[" + data.Name + "]</color>获得了<color=#6644FF><b>"+val.ToString()+ "</b></color>点经验。");
        int add = data.BaseData.AddEXP(val);
        if (add > 0)
        {
            OnScene.Report("<color=#CC44EE>[获得经验]: </color><color=Orange>[" + data.Name + "]</color>提升到了<color=#11FF55><b>" 
                + data.BaseData.level.ToString() + "</b></color>级，获得了<color=Yellow><b>" +add.ToString()+ "</b></color>点属性值。");

        }
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushEXP();
            dataPnl.FlushName();
        }
    }

    public void AddSkill(Skill skill)
    {
        OnScene.Report("<color=#CC44EE>[学习法术]: </color><color=Orange>[" + data.Name + "]</color>向法术书里记录了一项名为<color=#77CCCC>["
                + skill.nameStr + "]</color>的法术。");
        data.magicBook.Add(skill);
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushMagicBook();
        }
    }

    public void RemoveSkill(Skill skill)
    {
        OnScene.Report("<color=#CC44EE>[移除法术]: </color><color=Orange>[" + data.Name + "]</color>移除了一项名为<color=#77CCCC>["
        + skill.nameStr + "]</color>的法术。");
        data.magicBook.Remove(skill);
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushMagicBook();
        }
    }

    public void ChangeSkill(Skill old, Skill skill)
    {
        OnScene.Report("<color=#CC44EE>[整备法术]: </color><color=Orange>[" + data.Name + "]</color>将法术书中一项名为<color=#44AAAA>[" +
            old.nameStr + "]</color>的法术替换为名为<color=#44AAAA>[" + skill.nameStr + "]</color>的法术。");
        data.magicBook.Change(old, skill);
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushMagicBook();
        }
    }

    public void AddItem(Item item)
    {
        OnScene.Report("<color=#CC44EE>[获得物品]: </color><color=Orange>[" + data.Name + "]</color>获得了<color=#00CCFF><b>" +
            item.count.ToString() + "</b></color>个<color=#44AAAA>[" + item.Name + "]</color>。");
        data.package.Add(item);
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushItemList();
        }
    }

    public void RemoveItem(Item item)
    {
        OnScene.Report("<color=#CC44EE>[丢弃物品]: </color><color=Orange>[" + data.Name + "]</color>丢弃了<color=#00CCFF><b>" +
            item.count.ToString() + "</b></color>个<color=#44AAAA>[" + item.Name + "]</color>。");
        data.package.Remove(item);
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushItemList();
        }
    }


    public void HPGetDamage(int damage, bool checkEffect = true)
    {
        damage = GetRealDamage(damage);
        Sound.PlayDamage(damage);
        if (damage <= 0) return;
        //普通状态受到伤害
        if (data.BaseData.HP > 0)
        {
            data.BaseData.HP -= damage;
            if (data.BaseData.HP <= 0)
            {
                if (isHaveBuff("保护"))
                {
                    data.BaseData.HP = 1;
                }
                else
                {
                    PlaySound(CharacterSoundType.Death);
                    animCtrl.SetDeath(true);
                }
            }
            animCtrl.SetInjured();
        }
        //濒死状态受到伤害
        else
        {
            data.BaseData.HP -= damage;
        }
        if (checkEffect)
        {
            AfterGetDamage(damage);
        }
        if (OnScene.onSelect.character == this)
            dataPnl.FlushHP();
    }

    public void HPGetHeal(int heal)
    {
        if (isHaveBuff("死灵")||isHaveBuff("异变之种"))
        {
            HPGetDamage(heal);
            return;
        }
        Sound.PlayHeal();
        if (data.BaseData.HP <= 0)
        {
            data.BaseData.HP += heal;
            if (data.BaseData.HP > 0)
                animCtrl.SetDeath(false);
        }
        else
            data.BaseData.HP += heal;

        if (data.BaseData.HP > data.BaseData.MaxHP)
            data.BaseData.HP = data.BaseData.MaxHP;
        if (OnScene.onSelect.character == this)
            dataPnl.FlushHP();
    }

    public void MPGetDamage(int damage)
    {
        data.BaseData.MP -= damage;
        if (data.BaseData.MP < 0)
            data.BaseData.MP = 0;
        if (OnScene.onSelect.character == this)
            dataPnl.FlushMP();
    }
    public void MPGetHeal(int heal)
    {
        data.BaseData.MP += heal;
        if (data.BaseData.MP > data.BaseData.MaxMP)
            data.BaseData.MP = data.BaseData.MaxMP;
        if (OnScene.onSelect.character == this)
            dataPnl.FlushMP();
    }

    public void UseItem(Item item)
    {
        float APcost = Character.GetAPCost(item);
        if (data.AP < APcost)
        {
            OnScene.Report("<color=#55FFAA>[行动]: </color>由于行动力不足，<color=Orange>["
                + data.Name + "]</color>使用物品<color=Yellow>[" + item.Name + "]</color>失败。");
            return;
        }
        OnScene.Report("<color=#55FFAA>[行动]: </color><color=Orange>[" + OnScene.onSelect.character.data.Name
            + "]</color>使用了物品<color=Yellow>[" + item.Name + "]</color>");
        if (isHaveBuff("潜行", out Buff buff))
        {
            BuffEnd(buff);
        }
        data.package.Use(item);
        Sound.PlayHeal();
        data.AP -= APcost;
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushItemList();
            dataPnl.FlushAP();
        }
    }


    public void SkillUseConfirm(Skill skill)
    {
        data.BaseData.MP -= skill.MPcost;
        if (data.OccupationData.type == OccupationType.Rogue && data.OccupationData.branch == 1 && skill.protype.element == ElementType.Dark)
            data.AP -= Character.GetAPCost(skill) * 0.6f;
        else
            data.AP -= Character.GetAPCost(skill);
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushMP();
            dataPnl.FlushAP();
        }
        if (isHaveBuff("潜行", out Buff buff))
        {
            BuffEnd(buff);
        }
        OnScene.Report("<color=#55FFAA>[行动]: </color><color=Orange>["+ data.Name 
            + "]</color>使用了技能<color=Yellow>["+ skill.nameStr + "]</color>");
        if (data.OccupationData.type == OccupationType.Mage && data.OccupationData.branch == 1)
        {
            int MPheal = (int)(skill.MPcost * 0.25f);
            if (MPheal <= 0)
                MPheal = 1;
            OnScene.Report("<color=#CC44EE>[触发天赋]: </color><color=#FF3333>[" + data.Name + "]</color>受到了来自天赋的<color=#9999FF><b>"
                + MPheal.ToString() + "</b></color>点MP回复。");
            MPGetHeal(MPheal);
        }
        if (data.OccupationData.type == OccupationType.Warlock && data.OccupationData.branch == 1)
        {
            if (skill.protype.element == ElementType.Chaos || skill.protype.element == ElementType.Corrupt)
            {
                HPGetDamage(2);
                OnScene.Report("<color=#CC44EE>[触发天赋]: </color><color=#FF3333>[" + data.Name + "]</color>受到了来自天赋的<color=#FF1111><b>2</b></color>点伤害。");
            }
        }
    }

    public void UseSkill(Skill skill)
    {
        if (data.AP < Character.GetAPCost(skill))
        {
            OnScene.Report("<color=#55FFAA>[行动]: </color>由于行动力不足，<color=Orange>[" 
                + data.Name + "]</color>使用技能<color=Yellow>[" + skill.nameStr + "]</color>失败。");
            return;
        }
        if (data.BaseData.MP < skill.MPcost)
        {
            OnScene.Report("<color=#55FFAA>[行动]: </color>由于法力值不足，<color=Orange>["
                + data.Name + "]</color>使用技能<color=Yellow>[" + skill.nameStr + "]</color>失败。");
            return;
        }
        try
        {
            OnScene.manager.skillExecutor.StartExecute(this, skill.execStripts, skill);
        }
        catch (System.Exception e)
        {
            OnScene.Report("<color=#FF0000>[系统错误]:</color> 由于以下原因，<color=Orange>["
                + data.Name + "]</color>使用技能<color=Yellow>[" + skill.nameStr + "]</color>失败：" + e.ToString());
        }
    }

    public void SetSkillPreview(Skill skill)
    {
        try
        {
            OnScene.manager.skillExecutor.SetSkillPreview(this, skill.execStripts);
        }
        catch (System.Exception e)
        {
            OnScene.Report("<color=#FF0000>[系统错误]: </color>技能<color=Yellow>[" + skill.nameStr + "]</color>的执行脚本存在如下问题：" + e.ToString());
        }
    }
    public void ResetSkillPreview()
    {
        OnScene.manager.skillExecutor.ResetSkillPreview();
    }

    public void AddBuff(string buffName, int time, float value)
    {
        //如果已经有这个buf，更新它的强度和时间，后来的会冲掉前面的
        foreach (var v in data.buffs)
        {
            if (v.Name == buffName)
            {
                v.continueTime = time;
                v.value = value;
                return;
            }
        }
        System.Type buffType = OnGame.buffLib.buffs[buffName];
        Buff buff = (Buff)System.Activator.CreateInstance(buffType, this, time, value);
        buff.OnBuffAdd();
        StartCoroutine(AddBuffLater(buff));
    }
    public void ChangeBuff(int index, string buffName, int time, float value)
    {
        //同名的，改时间和强度
        if (buffName == data.buffs[index].Name)
        {
            data.buffs[index].continueTime = time;
            data.buffs[index].value = value;
            return;
        }
        //不同名的，直接删掉重加但不触发start和end事件
        System.Type buffType = OnGame.buffLib.buffs[buffName];
        Buff buff = (Buff)System.Activator.CreateInstance(buffType, this, time, value);
        data.buffs[index].OnBuffDisable();
        data.buffs[index] = buff;
        data.buffs[index].OnBuffEnable();
        if (OnScene.onSelect.character == this)
            dataPnl.FlushBuff();
    }
    public void RemoveBuff(int index)
    {
        data.buffs[index].OnBuffDisable();
        StartCoroutine(RemoveBuffLater(data.buffs[index]));
    }
    public void BuffEnd(Buff buff)
    {
        buff.OnBuffRemove();
        StartCoroutine(RemoveBuffLater(buff));
    }
    IEnumerator AddBuffLater(Buff buff)
    {
        yield return 0;
        data.buffs.Add(buff);
        if (OnScene.onSelect.character == this)
            dataPnl.FlushBuff();
    }

    IEnumerator RemoveBuffLater(Buff buff)
    {
        yield return 0;
        data.buffs.Remove(buff);
        if (OnScene.onSelect.character == this)
            dataPnl.FlushBuff();
    }

    public void ChangeBuffCallback(int index, string name, int time, float val)
    {
        ChangeBuff(index, name, time, val);
    }
    public void AddBuffCallback(int index, string name, int time, float val)
    {
        AddBuff(name, time, val);
    }

    public void HealRollHP(string title = "治疗效果",int faces = 4, int round = 1, string adjustPerRoll = "0", string adjust = "0")
    {
        RollPanel pnl = OnScene.manager.OpenRollPanel();
        pnl.SetUp(title, AfterHealRollHP, faces, round, adjustPerRoll, adjust);
    }

    public void DamageRollHP(string title = "直接伤害效果", int faces = 4, int round = 1, string adjustPerRoll = "0", string adjust = "0")
    {
        RollPanel pnl = OnScene.manager.OpenRollPanel();
        pnl.SetUp(title, AfterDamageRollHP, faces, round, adjustPerRoll, adjust);
    }

    public void HealRollMP(string title = "MP恢复效果", int faces = 4, int round = 1, string adjustPerRoll = "0", string adjust = "0")
    {
        RollPanel pnl = OnScene.manager.OpenRollPanel();
        pnl.SetUp(title, AfterHealRollMP, faces, round, adjustPerRoll, adjust);
    }

    public void DamageRollMP(string title = "直接MP伤害效果", int faces = 4, int round = 1, string adjustPerRoll = "0", string adjust = "0")
    {
        RollPanel pnl = OnScene.manager.OpenRollPanel();
        pnl.SetUp(title, AfterDamageRollMP, faces, round, adjustPerRoll, adjust);
    }

    public void AfterHealRollHP(int value)
    {
        OnScene.Report("<color=#CC44EE>[受到治疗]: </color><color=#33FF33>[" + data.Name + "]</color>受到了<color=#99FF99><b>" +value.ToString()+ "</b></color>点治疗。");
        HPGetHeal(value);
    }
    public void AfterDamageRollHP(int value)
    {
        OnScene.Report("<color=#CC44EE>[受到伤害]: </color><color=#33FF33>[" + data.Name + "]</color>受到了<color=#FF1111><b>" + value.ToString() + "</b></color>点伤害。");
        HPGetDamage(value);
    }
    public void AfterHealRollMP(int value)
    {
        OnScene.Report("<color=#CC44EE>[回复法力]: </color><color=#33FF33>[" + data.Name + "]</color>的MP恢复了<color=#9999FF><b>" + value.ToString() + "</b></color>点。");
        MPGetHeal(value);
    }
    public void AfterDamageRollMP(int value)
    {
        OnScene.Report("<color=#CC44EE>[减损法力]: </color><color=#33FF33>[" + data.Name + "]</color>的MP损失了<color=#EE99FF><b>" + value.ToString() + "</b></color>点。");
        MPGetDamage(value);
    }

    public int STR { get { return data.BaseData.STR; } set { data.BaseData.STR = value >= 0 ? value : 0; AfterChangeAttribute(); } }
    public int DEX { get { return data.BaseData.DEX; } set { data.BaseData.DEX = value >= 0 ? value : 0; AfterChangeAttribute(); } }
    public int CON { get { return data.BaseData.CON; } set { data.BaseData.CON = value >= 0 ? value : 0; AfterChangeAttribute(); } }
    public int INT { get { return data.BaseData.INT; } set { data.BaseData.INT = value >= 0 ? value : 0; AfterChangeAttribute(); } }
    public int WIS { get { return data.BaseData.WIS; } set { data.BaseData.WIS = value >= 0 ? value : 0; AfterChangeAttribute(); } }
    public int CHA { get { return data.BaseData.CHA; } set { data.BaseData.CHA = value >= 0 ? value : 0; AfterChangeAttribute(); } }
    public int HP { get { return data.BaseData.HP; } set { data.BaseData.HP = value; } }
    public int MP { get { return data.BaseData.MP; } set { data.BaseData.MP = value; } }
    void AfterChangeAttribute()
    {
        int val = data.BaseData.AfterAttributeModified(data.OccupationData);
        if (CON == 0) { HP = 0;animCtrl.SetInjured();animCtrl.SetDeath(true); PlaySound(CharacterSoundType.Death);}
        if (OnScene.onSelect.character == this)
        {
            dataPnl.FlushAttributeData(-1);
            switch (val)
            {
                case 0:
                    break;
                case 1:
                    dataPnl.FlushHP();
                    break;
                case 2:
                    dataPnl.FlushMP();
                    break;
                default:
                    dataPnl.FlushHP();
                    dataPnl.FlushMP();
                    break;
            }
            dataPnl.FlushMagicBook();
        }
    }


    public void InputSTR(string s)
    {
        if (int.TryParse(s, out int val))
        {
            STR = val;
        }
    }
    public void InputDEX(string s)
    {
        if (int.TryParse(s, out int val))
        {
            DEX = val;
        }
    }
    public void InputCON(string s)
    {
        if (int.TryParse(s, out int val))
        {
            CON = val;
        }
    }
    public void InputINT(string s)
    {
        if (int.TryParse(s, out int val))
        {
            INT = val;
        }
    }
    public void InputWIS(string s)
    {
        if (int.TryParse(s, out int val))
        {
            WIS = val;
        }
    }
    public void InputCHA(string s)
    {
        if (int.TryParse(s, out int val))
        {
            CHA = val;
        }
    }
    public void Sneak()
    {
        CombatPanel cPanel = OnScene.manager.OpenCombatPanel();
        cPanel.SetUp("潜行:敏捷检定", data.Name, "环境", data.BaseData.DEX, 0, (int i) => { if (i > 0) AddBuff("潜行", -1, 1); }, "0", "0", "-10");
    }
    //计时模式开启时
    public void OnRoundStart()
    {
        if (OnScene.isTimerModeOn)
        {
            return;
        }
        
        foreach (Buff buff in data.buffs)
        {
            buff.OnRoundStart();
        }
        if (isHaveBuff("禁锢") || isHaveBuff("好梦") || isHaveBuff("黑暗根须"))
        {
            data.AP += 0;
        }
        else if (isHaveBuff("冰冻"))
        {
            data.AP += Character.ONE_TURN_AP / 2;
        }
        else
        {
            data.AP += Character.ONE_TURN_AP;
        }
        if (data.AP > Character.ONE_TURN_AP * 2)
            data.AP = Character.ONE_TURN_AP * 2;
        if (OnScene.onSelect.character == this)
            dataPnl.FlushAP();
        OnScene.isTimerModeOn = true;
        isOperating = true;
        canvas.SetDisplay();
    }
    //计时模式关闭时
    public void OnRoundEnd()
    {
        if (!OnScene.isTimerModeOn)
        {
            return;
        }
        foreach (Buff buff in data.buffs)
        {
            buff.OnRoundEnd();
        }
        if (OnScene.onSelect.character == this)
            dataPnl.PreviewAPCost(0.0f);
        OnScene.isTimerModeOn = false;
        isOperating = false;
        canvas.SetDisplay();
    }
    public bool isHaveBuff(string buffName)
    {
        foreach (Buff buff in data.buffs)
        {
            if (buff.Name == buffName)
                return true;
        }
        return false;
    }
    public bool isHaveBuff(string buffName,out Buff buffinstance)
    {
        foreach (Buff buff in data.buffs)
        {
            if (buff.Name == buffName)
            {
                buffinstance = buff;
                return true;
            }
        }
        buffinstance = null;
        return false;
    }
    public void StartWarp()
    {
        MapSet.nowCreator.UnregistCharacter(this);
        OnScene.manager.CharacterSelector.StartLocationCatch(WarpCallBack);
    }
    public void WarpCallBack(List<CharacterScript> NoUsed, Vector3 WarpTo)
    {
        NavAgent.Warp(WarpTo);
        MapSet.nowCreator.RegistCharacter(this,true);
    }
}
