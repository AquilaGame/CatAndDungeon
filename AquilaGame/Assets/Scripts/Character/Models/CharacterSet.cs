using System.Collections;
using System.Collections.Generic;
using static System.Convert;
using UnityEngine;
public class Attribute6
{
    public int _str;//力量，影响力量检定和近战职业伤害
    public int _dex;//敏捷，影响敏捷检定和敏捷职业伤害，同时影响移动速度和每回合移动距离，法术和攻击命中率，以及出手顺序
    public int _con;//体质，影响体质检定，血上限和受到普通攻击时的防御力
    public int _int;//智力，影响智力检定，法术普通攻击伤害
    public int _wis;//感知，影响感知检定和法术普通攻击距离
    public int _cha;//魅力，影响魅力检定和初始金钱
    public Attribute6(int STR, int DEX, int CON, int INT, int WIS, int CHA)
    {
        _str = STR;
        _dex = DEX;
        _con = CON;
        _int = INT;
        _wis = WIS;
        _cha = CHA;
    }
    public Attribute6(Attribute6 b)
    {
        _str = b._str;
        _dex = b._dex;
        _con = b._con;
        _int = b._int;
        _wis = b._wis;
        _cha = b._cha;
    }
    static public Attribute6 operator + (Attribute6 attributeA, Attribute6 attributeB)
    {
        return new Attribute6(
            attributeA._str + attributeB._str,
            attributeA._dex + attributeB._dex,
            attributeA._con + attributeB._con,
            attributeA._int + attributeB._int,
            attributeA._wis + attributeB._wis,
            attributeA._cha + attributeB._cha);
    }
    public override string ToString()
    {
        return _str.ToString() + ' '
                + _dex.ToString() + ' '
                + _con.ToString() + ' '
                + _int.ToString() + ' '
                + _wis.ToString() + ' '
                + _cha.ToString();
    }
}

public class AttributeData
{
    public Attribute6 baseAttribute;
    public static int[] EXPTable =
        {/*1->2*/1300,/*2->3*/2000,/*3->4*/2700,/*4->5*/4000,/*5->6*/5000,
         /*6->7*/7600,/*7->8*/9500,/*8->9*/12000,/*9->10*/19200,/*10->11*/23560,
         /*11->12*/34200,/*12->13*/52000,/*13->14*/63420,/*14->15*/78000,/*15->16*/85000,
         /*16->17*/97000,/*17->18*/103500,/*18->19*/120800,/*19->20*/150000,/*不可能达到*/196850};
    public int EXP = 0;
    public int NextEXP
    {
        get { return EXPTable[level-1]; }
    }
    public int MaxHP = 0;
    public int MaxMP = 0;
    public int HP = 0;
    public int MP = 0;
    public int level = 1;
    List<Attribute6> bufflist = new List<Attribute6>();
    public int STR
    {
        get
        {
            int ret = baseAttribute._str;
            foreach (var v in bufflist)
            {
                ret += v._str;
            }
            return ret;
        }
        set { baseAttribute._str = value; }
    }
    public int DEX
    {
        get
        {
            int ret = baseAttribute._dex;
            foreach (var v in bufflist)
            {
                ret += v._dex;
            }
            return ret;
        }
        set { baseAttribute._dex = value; }
    }
    public int CON
    {
        get
        {
            int ret = baseAttribute._con;
            foreach (var v in bufflist)
            {
                ret += v._con;
            }
            return ret;
        }
        set { baseAttribute._con = value; }
    }
    public int INT
    {
        get
        {
            int ret = baseAttribute._int;
            foreach (var v in bufflist)
            {
                ret += v._int;
            }
            return ret;
        }
        set { baseAttribute._int = value; }
    }
    public int WIS
    {
        get
        {
            int ret = baseAttribute._wis;
            foreach (var v in bufflist)
            {
                ret += v._wis;
            }
            return ret;
        }
        set { baseAttribute._wis = value; }
    }
    public int CHA
    {
        get
        {
            int ret = baseAttribute._cha;
            foreach (var v in bufflist)
            {
                ret += v._cha;
            }
            return ret;
        }
        set { baseAttribute._cha = value; }
    }
    public AttributeData(Attribute6 attribute6,Occupation occupation)
    {
        baseAttribute = attribute6;
        if (occupation.type == OccupationType.Warrior && occupation.branch == 0)
        {
            baseAttribute._con += 2;
        }
        //按CON值获得HP
        MaxHP = GetMaxHP();
        HP = MaxHP;
        //按职业种类获得MP
        MaxMP = GetMaxMP(occupation);
        MP = MaxMP;
    }
    public int AfterAttributeModified(Occupation occupation)
    {
        //返回0代表不需要刷新，1代表刷新HP，2代表刷新MP，3代表都刷新。
        int ret = 0;
        int maxhp = GetMaxHP();
        int maxmp = GetMaxMP(occupation);
        if (maxhp != MaxHP)
        {
            ret |= 1;
            HP = (int)((float)HP / MaxHP * maxhp);
            MaxHP = maxhp;
        }
        if (maxmp != MaxMP)
        {
            ret |= 2;
            MP = (int)((float)MP / MaxMP * maxmp);
            MaxMP = maxmp;
        }
        return ret;
    }

    public int AddEXP(int exp)
    {
        if (level >= 20)
            return 0;
        int level_old = level;
        EXP += exp;
        while (EXP > NextEXP)
        {
            EXP -= NextEXP;
            level++;
        }
        return (level- level_old) *2;
    }

    public int GetMaxHP()
    {
        return baseAttribute._con > 0 ? (baseAttribute._con * 2) : 1;
    }
    public int GetMaxMP(Occupation occupation)
    {
        int ret = 1;
        switch (occupation.type)
        {
            //一个法师在初始时候能在不补蓝情况下放10个初阶技能
            //神职职业的法力是法师60%到80%左右，因为他们的技能更加牛逼
            //非法术职业的法力差不多是法师的%30-%50，因为他们的法术消耗就低
            //主施法职业，智力影响法术等级
            //法师只受智力影响，初始值期望是20，对应60点，之后每投入1点增加6点，25级时达到90点，30级时达到120点
            case OccupationType.Mage:
                if (baseAttribute._int > 20)
                { ret = baseAttribute._int * 6 - 60; break; }
                else
                { ret = baseAttribute._int * 3; break; }
            //萨满受智力和魅力影响，初始值期望是智力20魅力15，对应60点,相当于二者相乘除以5,只加智力的话25级是75,30级是90
            case OccupationType.Shaman:
                { ret = baseAttribute._int * baseAttribute._cha / 5; break; }
            //术士受智力和体力影响，初始期望是智力18体力18，考虑到加体力是正效果，把二者相乘除以6，相当于初始54,全加智力的话30级是90
            case OccupationType.Warlock:
                {                    ret = baseAttribute._int * baseAttribute._con / 6; break;}
            //神职和半神职职业，感知影响法术等级
            //牧师主属性是感知和魅力，和萨满一样，不过少一点，除以6，大概相当于萨满的83%，太低了奶不动大家。
            case OccupationType.Priest:
                { ret = baseAttribute._cha * baseAttribute._wis / 6; break; }
            //侍僧主属性是感知和体质，再加上本身自己技能就强，还能献祭队友，除以7。
            case OccupationType.Acolyte:
                { ret = baseAttribute._con * baseAttribute._wis / 7; break; }
            //圣骑士只受感知影响，初始期望是16，12之前一个感知是2，从13起，一个感知是3，这样一个主感知圣骑士15级是33，20级是48，25级是63,30级是78
            case OccupationType.Paladin:
                if (baseAttribute._wis > 12)
                { ret = baseAttribute._wis * 3 - 12; break; }
                else
                { ret = baseAttribute._wis * 2; break; }
            //黑武士只受力量影响，初始期望是18,20之前一个力量是2.从21起，一个力量是3，初始力量18的黑武士是36,25级是55,30级是70
            case OccupationType.Darknight:
                if (baseAttribute._str > 20)
                { ret = baseAttribute._str * 3 - 10; break; }
                else
                { ret = baseAttribute._str * 2; break; }
            //德鲁伊和牧师一样
            case OccupationType.Druid:
                { ret = baseAttribute._cha * baseAttribute._wis / 6; break; }
            //战士受体质、敏捷和力量三者影响，期望值是18,16,18，以三者合计值每点提供1算,初始50-60，每升一级增加2
            case OccupationType.Warrior:
                { ret = baseAttribute._str + baseAttribute._dex + baseAttribute._con; break; }
            //猎人受敏捷和感知影响，和侍僧一样公式就行
            case OccupationType.Hunter:
                { ret = baseAttribute._dex * baseAttribute._wis / 7; break; }
            //游荡者受敏捷、感知和魅力影响，其中魅力的影响是额外的
            case OccupationType.Rogue:
                { ret = baseAttribute._dex * baseAttribute._wis / 7 + baseAttribute._cha; break; }
                    default:
                { ret = 1; break; }
        }
        return ret > 0 ? ret : 1;
    }
    
    public override string ToString()
    {
        return baseAttribute.ToString() + ' ' 
            + MaxHP.ToString() + ' ' + MaxMP.ToString() + ' '
            + HP.ToString() + ' ' + MP.ToString() + ' '
            + level.ToString() + ' ' + EXP.ToString();
    }
}

public enum BirthGroupType
{
    KingdomMale,
    KingDomFemale,
    EmpireMale,
    EmpireFemale,
    ChurchMale,
    ChurchFemale,
    CityMale,
    CityFemale,
    Monsters,
    NPCs
}

public enum CharacterType {Enemy,Neutral,Friend}
public class Character
{
    public const float ONE_TURN_AP = 4.0f;
    public string Name;
    public string Log;
    public string Nickname;
    public string BranchName;
    public AttributeData BaseData = null;
    public Occupation OccupationData = null;
    public AlignmentType alignmentType = AlignmentType.LawfulGood;
    public MagicBook magicBook = null;
    public ItemList package = null;
    public BirthGroupType birth = BirthGroupType.KingdomMale;
    public CharacterType characterType = CharacterType.Neutral;
    public List<Buff> buffs = null;
    public float AP = 0.0f;
    public int SkillAssignableCount { get { return Occupation.GetOcccupationMaxSkillCount(this) - magicBook.Count; } }
    public Character(string name,string log, string nickname, string branchname,
        Occupation occupation,BirthGroupType birthGroupType, AttributeData attribute, AlignmentType alignment,
        MagicBook book, ItemList itemList
        )
    {
        Name = name;
        Nickname = nickname;
        BranchName = branchname;
        Log = log;
        OccupationData = occupation;
        birth = birthGroupType;
        BaseData = attribute;
        alignmentType = alignment;
        magicBook = book;
        package = itemList;
        buffs = new List<Buff>();
        //依照CHA获得初始金钱
        Dice D10 = new Dice(10);
        package.Add(new Item(D10.roll(BaseData.CHA),"金币", "闪闪发光的金币"));
        AP = 0.0f;
    }

    public Character(System.IO.StreamReader sr)
    {
        Name = "读取错误";
        try
        {
            AP = 0.0f;
            Name = sr.ReadLine();
            Nickname = sr.ReadLine();
            BranchName = sr.ReadLine();
            string alignAndBirthStr = sr.ReadLine();
            string occupationStr = sr.ReadLine();
            string attributeStr = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Nickname)
                || string.IsNullOrWhiteSpace(BranchName) || string.IsNullOrEmpty(alignAndBirthStr)
                || string.IsNullOrEmpty(occupationStr) || string.IsNullOrEmpty(attributeStr))
                throw new System.Exception("读取基础属性错误");
            string[] ss = alignAndBirthStr.Split(' ');
            alignmentType = (AlignmentType)ToInt32(ss[0]);
            birth = (BirthGroupType)ToInt32(ss[1]);
            if (ss.Length >= 3)
                characterType = (CharacterType)ToInt32(ss[2]);
            ss = occupationStr.Split(' ');
            OccupationData = new Occupation((OccupationType)ToInt32(ss[0]), ToInt32(ss[1]));
            ss = attributeStr.Split(' ');
            BaseData = new AttributeData(
                new Attribute6(ToInt32(ss[0]),ToInt32(ss[1]),ToInt32(ss[2]),ToInt32(ss[3]),ToInt32(ss[4]),ToInt32(ss[5]))
                , OccupationData);

            BaseData.MaxHP = ToInt32(ss[6]);
            BaseData.MaxMP = ToInt32(ss[7]);
            BaseData.   HP = ToInt32(ss[8]);
            BaseData.   MP = ToInt32(ss[9]);
            BaseData.level = ToInt32(ss[10]);
            BaseData.  EXP = ToInt32(ss[11]);

            int MagicCount = ToInt32(sr.ReadLine());
            magicBook = new MagicBook();
            for (int i = 0; i < MagicCount; i++)
            {
                ss = sr.ReadLine().Split(' ');
                Skill skill = new Skill(Skill.ProtypeList[ToInt32(ss[0]), ToInt32(ss[1])], ToInt32(ss[2]), ToInt32(ss[3]));
                magicBook.Add(skill);
            }
            int ItemCount = ToInt32(sr.ReadLine());
            package = new ItemList();
            for (int i = 0; i < ItemCount; i++)
            {
                int count = ToInt32(sr.ReadLine());
                string itemName = sr.ReadLine();
                string itemLog = sr.ReadLine();
                package.Add(new Item(count, itemName, itemLog));
            }
            buffs = new List<Buff>();
            int BuffCount = ToInt32(sr.ReadLine());
            {
                for (int i = 0; i < BuffCount; i++)
                {
                    ss = sr.ReadLine().Split(' ');
                    if (ss[1] == "-1" && ss[2] == "-1")
                    {
                        ;
                    }
                    else
                    {
                        System.Type buffType = OnGame.buffLib.buffs[ss[0]];
                        buffs.Add((Buff)System.Activator.CreateInstance(buffType, null, ToInt32(ss[1]), ToSingle(ss[2])));
                    }
                    //刷新buff的事交给外面的CharacterScript处理
                }
            }
            //-------
            Log = "";
            string logLine;
            while ((logLine = sr.ReadLine()) != null)
            {
                Log += logLine + '\n';
            }

        }
        catch (System.Exception e)
        {
            OnScene.Report(e.ToString());
            Log = e.ToString();
            BranchName = "读取错误";
            Nickname = "读取错误";
            birth = BirthGroupType.KingdomMale;
            OccupationData = Occupation.identity;
            BaseData = new AttributeData(new Attribute6(20, 20, 20, 20, 20, 20), Occupation.identity);
            alignmentType = AlignmentType.ChaoticEvil;
            magicBook = new MagicBook();
            package = new ItemList();
            buffs = new List<Buff>();
            AP = 0.0f;
        }

    }
    public void Save(System.IO.StreamWriter sw)
    {
        //基本数据
        sw.WriteLine(Name);
        sw.WriteLine(Nickname);
        sw.WriteLine(BranchName);
        sw.WriteLine(((int)alignmentType).ToString() + ' ' + ((int)birth).ToString() + ' ' + ((int)characterType).ToString());
        //职业数据
        sw.WriteLine(((int)OccupationData.type).ToString() + ' ' + OccupationData.branch.ToString());
        //属性点
        sw.WriteLine(BaseData.ToString());
        //法术书
        sw.WriteLine(magicBook.Count);
        foreach (var v in magicBook.skills)
        {
            sw.WriteLine(v.ToString());
        }
        //背包
        sw.WriteLine(package.Count);
        foreach (var v in package.items)
        {
            sw.WriteLine(v.ToString());
        }
        int BranchBuffIndex = buffs.FindIndex((b) => { return b.continueTime == -1 && b.value == -1.0f; });
        sw.WriteLine(BranchBuffIndex == -1 ? buffs.Count : buffs.Count - 1);
        for (int i = 0; i < buffs.Count; i++)
        {
            if (i == BranchBuffIndex)
                continue;
            else 
                sw.WriteLine(buffs[i].Name + ' ' + buffs[i].continueTime.ToString() + ' ' + buffs[i].value.ToString());
        }
        //附加数据
        sw.Write(Log);
    }

    public float AttackRange
    {
        get
        {
            switch (OccupationData.type)
            {
                //近战职业
                //使用巨型武器职业
                case OccupationType.Paladin:
                case OccupationType.Darknight:
                    return 3.0f * OnGame.WorldScale;
                //剑盾职业
                case OccupationType.Acolyte:
                case OccupationType.Priest:
                case OccupationType.Warrior:
                    return 2.0f * OnGame.WorldScale;
                //长枪职业
                case OccupationType.Druid:
                    return 2.5f * OnGame.WorldScale;
                //匕首职业
                case OccupationType.Rogue:
                    return 1.7f * OnGame.WorldScale;
                //物理远程
                case OccupationType.Hunter:
                    return BaseData.DEX / 3.0f * OnGame.WorldScale;
                //魔法远程
                case OccupationType.Mage:
                case OccupationType.Shaman:
                case OccupationType.Warlock:
                    return (BaseData.WIS + BaseData.INT) / 6.0f * OnGame.WorldScale;
                default:
                    return 0;
            }
        }
    }
    public int NormalATK
    {
        get
        {
            switch (OccupationData.type)
            {
                //近战职业
                case OccupationType.Paladin:
                case OccupationType.Darknight:
                case OccupationType.Acolyte:
                case OccupationType.Priest:
                case OccupationType.Warrior:
                case OccupationType.Druid:
                case OccupationType.Rogue:
                    return BaseData.STR * 2;
                //物理远程
                case OccupationType.Hunter:
                    return BaseData.DEX * 2;
                //魔法远程
                case OccupationType.Mage:
                case OccupationType.Shaman:
                case OccupationType.Warlock:
                    return BaseData.INT * 2;
                default:
                    return 0;
            }
        }
    }
    public int NormalDEF
    {
        get
        {
            return BaseData.CON;
        }
    }
    public const float ZeroAPCost = 0.0f;
    public static float GetAPCost(object obj)
    {
        if (OnScene.isTimerModeOn == false)
            return 0.0f;
        System.Type type = obj.GetType();
        if (type == typeof(float))
        {
            return (float)obj;
        }
        else if (type == typeof(Skill))
        {
            return 2.0f;
        }
        else if (type == typeof(Item))
        {
            return 1.0f;
        }
        else if (type == typeof(int))
        {
            return (int)obj;
        }
        else return 0.0f;
    }

    public bool APCanDo(object obj)
    {
        return AP >= GetAPCost(obj);
    }
    public float WalkAPCost(float Distance)
    {
        if (OnScene.isTimerModeOn == false)
            return 0.0f;
        return Distance * 8 / BaseData.DEX;
    }
    public bool APCanWalk(float Distance)
    {
        return AP > WalkAPCost(Distance);
    }
}

public class CharacterAttributeTranslator
{
    Character A = null;
    Character B = null;
    public void Set(Character A, Character B)
    {
        this.A = A;
        this.B = B;
    }
    public string Convert(string s)
    {
        if (s[0] != '(')
            return s;
        Character character = null;
        string[] ss = s.Substring(1, s.Length - 2).Split('.');
        switch (ss[0])
        {
            case "发起方":
            case "使用方":
            case "A":
                character = A;
                break;
            case "对抗方":
            case "目标方":
            case "B":
                character = B;
                break;
        }
        //↑如果用户这个地方的参数写错了，就会抛出一个空引用异常。
        //下面也是↓
        switch (ss[1])
        {
            case "STR":
            case "力量":
                return character.BaseData.STR.ToString();
            case "DEX":
            case "敏捷":
                return character.BaseData.DEX.ToString();
            case "CON":
            case "体质":
                return character.BaseData.CON.ToString();
            case "INT":
            case "智力":
                return character.BaseData.INT.ToString();
            case "WIS":
            case "感知":
                return character.BaseData.WIS.ToString();
            case "CHA":
            case "魅力":
                return character.BaseData.CHA.ToString();
            case "DEF":
            case "防御":
                return character.NormalDEF.ToString();
            case "ATK":
            case "攻击":
                return character.NormalATK.ToString();
            case "NAME":
            case "名字":
                return character.Name;
            default:
                throw new System.Exception("括号中的内容无法被转换");
        }

    }
}
