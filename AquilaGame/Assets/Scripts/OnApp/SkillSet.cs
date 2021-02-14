using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType//12
{
    Fire,
    Water,
    Air,
    Earth,
    Order,
    Chaos,
    Light,
    Dark,
    Life,
    Dead,
    Metal,
    Corrupt
}
public enum FieldType//9
{
    Sword,
    Shield,
    Move,
    Trap,
    Sense,
    Tool,
    Energy,
    Wild,
    Plant
}

public class SkillStartScript
{
    public enum Selector {Self,Targets,Neighbor,Range};
    public enum Filter {Friendly,Enemy,All,None};
    public string distanceStr;
    public string rangeStr;
    public string countStr;
    public Selector selector;
    public Filter filter;
    const string noUseStr = null;

    static Filter GetFilter(string _filter)
    {
        if (_filter.Contains("友"))
        {
            return Filter.Friendly;
        }
        else if (_filter.Contains("敌"))
        {
            return Filter.Enemy;
        }
        else if (_filter.Contains("任"))
        {
            return Filter.All;
        }
        else if (_filter.Contains("无"))
        {
            return Filter.None;
        }
        else
            throw new System.Exception("不正确的目标指示器");
    }

    //Self模式
    public SkillStartScript()
    {
        selector = Selector.Self;
        distanceStr = noUseStr;
        rangeStr = noUseStr;
        countStr = noUseStr;
        filter = Filter.Friendly;
    }
    //Targets模式
    public SkillStartScript(string distance, string count, string _filter)
    {
        selector = Selector.Targets;
        distanceStr = distance;
        rangeStr = noUseStr;
        countStr = count;
        filter = GetFilter(_filter);
    }
    //Neighbor模式
    public SkillStartScript(string range, string _filter)
    {
        selector = Selector.Neighbor;
        distanceStr = noUseStr;
        rangeStr = range;
        countStr = noUseStr;
        filter = GetFilter(_filter);
    }
    //Range模式
    public SkillStartScript(string distance, string range, string count, string _filter)
    {
        selector = Selector.Range;
        distanceStr = distance;
        rangeStr = range;
        countStr = noUseStr;
        filter = GetFilter(_filter);
    }
}

public class SkillHitScript
{
    public enum Detector {Forced,Combat}
    public string title;
    public Detector detector;
    public string CombatBase_A;
    public string CombatBase_B;
    public string CombatExtra_A;
    public string CombatExtra_B;
    public string CombatAdd;
    const string noUseStr = null;
    //必中模式
    public SkillHitScript()
    {
        title = noUseStr;
        detector = Detector.Forced;
        CombatBase_A = noUseStr;
        CombatBase_B = noUseStr;
        CombatExtra_A = noUseStr;
        CombatExtra_B = noUseStr;
        CombatAdd = noUseStr;
    }
    //对抗 全写模式
    public SkillHitScript(string _title, string Base_A, string Ex_A, string vsStr, string Base_B, string Ex_B, string Add)
    {
        title = _title;
        detector = Detector.Combat;
        if (vsStr != "VS")
            throw new System.Exception("Hit脚本中，名为" + title + "的项未检测到字符串VS");
        CombatBase_A = Base_A;
        CombatBase_B = Base_B;
        CombatExtra_A = Ex_A;
        CombatExtra_B = Ex_B;
        CombatAdd = Add;
    }
    //对抗 省略加值模式
    public SkillHitScript(string _title, string Base_A, string vsStr, string Base_B)
    {
        title = _title;
        detector = Detector.Combat;
        if (vsStr != "VS")
            throw new System.Exception("Hit脚本中，名为" + title + "的项未检测到字符串VS");
        CombatBase_A = Base_A;
        CombatBase_B = Base_B;
        CombatExtra_A = "0";
        CombatExtra_B = "0";
        CombatAdd = "0";
    }
    //对抗 极简模式
    public SkillHitScript(string _title, string attributeName, string vsStr)
    {
        title = _title;
        detector = Detector.Combat;
        CombatBase_A = "(A." + attributeName + ")";
        CombatBase_B = "(B." + attributeName + ")";
        CombatExtra_A = "0";
        CombatExtra_B = "0";
        CombatAdd = "0";
    }
}
public class SkillValueScript
{
    public enum Calculator { Number, Combat, Random}
    public string title;
    public Calculator calculator;
    //数值模式下存储数据，对抗模式下存储BaseA，随机模式下存储骰子数据
    public string deltaStrA;
    //下面都是对抗用的数据
    public string deltaStrB;
    public string CombatExtra_A;
    public string CombatExtra_B;
    public string CombatAdd;
    const string noUseStr = null;
    //数值模式
    public SkillValueScript(string s)
    {
        title = noUseStr;
        calculator = Calculator.Number;
        deltaStrA = s;
        deltaStrB = noUseStr;
        CombatExtra_A = noUseStr;
        CombatExtra_B = noUseStr;
        CombatAdd = noUseStr;
    }
    //随机模式
    public SkillValueScript(string _title, string s)
    {
        title = _title;
        calculator = Calculator.Random;
        deltaStrA = s;
        deltaStrB = noUseStr;
        CombatExtra_A = noUseStr;
        CombatExtra_B = noUseStr;
        CombatAdd = noUseStr;
    }
    //对抗 全写模式
    public SkillValueScript(string _title, string Base_A, string Ex_A, string vsStr, string Base_B, string Ex_B, string Add)
    {
        title = _title;
        calculator = Calculator.Combat;
        if (vsStr != "VS")
            throw new System.Exception("Value脚本中，名为" + title + "的项未检测到字符串VS");
        deltaStrA = Base_A;
        deltaStrB = Base_B;
        CombatExtra_A = Ex_A;
        CombatExtra_B = Ex_B;
        CombatAdd = Add;
    }
    //对抗 省略加值模式
    public SkillValueScript(string _title, string Base_A, string vsStr, string Base_B)
    {
        title = _title;
        calculator = Calculator.Combat;
        if (vsStr != "VS")
            throw new System.Exception("Value脚本中，名为" + title + "的项未检测到字符串VS");
        deltaStrA = Base_A;
        deltaStrB = Base_B;
        CombatExtra_A = "0";
        CombatExtra_B = "0";
        CombatAdd = "0";
    }
    //对抗 极简模式
    public SkillValueScript(string _title, string attributeName, string vsStr)
    {
        title = _title;
        calculator = Calculator.Combat;
        deltaStrA = "(A." + attributeName + ")";
        deltaStrB = "(B." + attributeName + ")";
        CombatExtra_A = "0";
        CombatExtra_B = "0";
        CombatAdd = "0";
    }
}

public class SkillResultScript
{
    public enum Executor {Damage,Heal,Call,CallObj,AddState,Report}
    public Executor executor;
    public List<Executor> todoList;
    public List<string> args;
    public SkillResultScript(string s)
    {
        todoList = new List<Executor>();
        args = new List<string>();
        string[] ss = s.Split(' ');
        if (ss.Length < 2)
        {
            throw new System.Exception("Result脚本中，名为" + s + "的项未检测到完整的命令列表");
        }
        try
        {
            for (int i = 0; i < ss.Length; i++)
            {
                switch (ss[i])
                {
                    case "-伤害"://1个参数
                        todoList.Add(Executor.Damage);
                        i++;
                        args.Add(ss[i]);
                        break;
                    case "-治疗"://1个参数
                        todoList.Add(Executor.Heal);
                        i++;
                        args.Add(ss[i]);
                        break;
                    case "-提示"://1个参数
                        todoList.Add(Executor.Report);
                        i++;
                        args.Add(ss[i]);
                        break;
                    case "-召唤"://2个参数
                        todoList.Add(Executor.Call);
                        i++;
                        args.Add(ss[i]);
                        i++;
                        args.Add(ss[i]);
                        break;
                    case "-召唤物体"://3个参数
                        todoList.Add(Executor.CallObj);
                        i++;
                        args.Add(ss[i]);
                        i++;
                        args.Add(ss[i]);
                        i++;
                        args.Add(ss[i]);
                        break;
                    case "-状态"://3个参数
                        todoList.Add(Executor.AddState);
                        i++;
                        args.Add(ss[i]);
                        i++;
                        args.Add(ss[i]);
                        i++;
                        args.Add(ss[i]);
                        break;
                    default:
                        throw new System.Exception();
                }
            }
        }
        catch (System.Exception e)
        {
            throw new System.Exception("Result脚本中，名为" + s + "的项未检测到完整的命令列表(" + e.ToString() + ")");
        }
    }
}

public class SkillScript
{
    public SkillStartScript start;
    public SkillHitScript hit;
    public SkillValueScript value;
    public SkillResultScript result;
    public SkillScript next;
    public string recur;
    public SkillScript(SkillStartScript st, SkillHitScript ht, SkillValueScript val, SkillResultScript res, string cur)
    {
        start = st;
        hit = ht;
        value = val;
        result = res;
        recur = cur;
        next = null;
    }
}

public class SkillExecuteScript
{
    public SkillStartScript.Selector selector;
    public SkillStartScript.Filter filter;
    public SkillHitScript.Detector detector;
    public SkillValueScript.Calculator calculator;
    public List<SkillResultScript.Executor> executors = new List<SkillResultScript.Executor>();
    public float distance;
    public float range;
    public int selectCount;
    public string HitTitle;
    public string HitBaseA;
    public string HitBaseB;
    public string HitExA;
    public string HitExB;
    public string HitAdd;
    public string ValueTitle;
    public string ValueDeltaA;
    public string ValueDeltaB;
    public string ValueExA;
    public string ValueExB;
    public string ValueAdd;
    public List<string> args = new List<string>();

    public SkillExecuteScript()
    {

    }
}

public class SkillProtype
{
    public ElementType element;
    public FieldType field;
    public SkillScript script;
    public int MPcost = 0;
    public string typestr;
    public string name;
    public string info;
    public SkillProtype(ElementType ele, FieldType fld,SkillScript skillScript, int cost, string _type, string _name, string _info)
    {
        element = ele;
        field = fld;
        script = skillScript;
        MPcost = cost;
        typestr = _type;
        name = _name;
        info = _info;
    }
    public Color GetColor()
    {
        return Color.Lerp(OnGame.MagicLib.EleColors[(int)element],
                OnGame.MagicLib.FldColors[(int)field], 0.5f);
    }
}

public class Skill
{
    //三个标杆法术：
    //初阶：近战类-愤怒打击-2级消耗2，法术类-寒冰箭-2级消耗4，强力类-神圣领域-2级消耗10
    //中阶：近战类-愤怒打击-4级消耗4，法术类-寒冰箭-4级消耗8，强力类-神圣领域-4级消耗20
    //高阶：近战类-愤怒打击-6级消耗6，法术类-寒冰箭-6级消耗12，强力类-神圣领域-6级消耗30
    //低阶小怪应该能被中阶输出法术秒（一轮下来），单体输出法术的典例是雷霆震击，高阶12费打6-34，中阶8费打5-25，低阶3-10
    //爆发高的技能，灵魂之焰这种，纯输出不考虑距离高阶能打5-50，对自己打5-20
    //AOE技能，比如火球一发低阶打1-4，中阶是2-8，高阶是3-12
    //治愈术高阶回复16-40，中阶回12-30，低阶回4-10，因为一般只有一个奶，但对面可能会打很多下
    //同级的近战普通输出应该比雷霆震击略低
    //按这样算的话一个小怪血量差不多是20，中型怪是40，大型怪是60
    //角色差不多算是小型和中型之间，考虑到凑不齐人啥的，就先算每个体质生命点数是3
    //2021年1月更新：重置了数值模型，上面写的那些废了-_-#
    public static string[] EleNames = System.Enum.GetNames(ElementType.Air.GetType());
    public static string[] FldNames = System.Enum.GetNames(FieldType.Energy.GetType());
    public static string[] EleDisNames = { "火焰","水域","大气","大地","秩序","混乱","光明","黑暗","生命","死灵","金属","腐化"};
    public static string[] FldDisNames = { "武力", "防护", "移动", "束缚", "感知", "工具", "能量", "野兽", "植物"};
    public static string[] EleColorStr = 
        {"<color=#FF5A00>","<color=#3CD4FD>","<color=#FFFF7E>","<color=#56C100>","<color=#D3D3EC>","<color=#424142>",
         "<color=#FFF763>","<color=#232423>","<color=#FF2E34>","<color=#394100>","<color=#B5B5CE>","<color=#810081>" };
    public static string[] FldColorStr = 
        {"<color=#C0514F>","<color=#00C0C0>","<color=#A5A3C3>","<color=#977D7E>","<color=#89F000>","<color=#EFAC84>",
         "<color=#C0FFFF>","<color=#9F6508>","<color=#008A00>",};
    public static string DisableColorStr = "<color=#AAAAAA>";
    public static string EndColotStr = "</color>";
    public static SkillProtype[,] ProtypeList = new SkillProtype[EleNames.Length, FldNames.Length];

    public SkillProtype protype;
    public int EleForce;
    public int FldForce;
    public Skill(SkillProtype skillProtype, int elecount, int fldcount)
    {
        protype = skillProtype;
        EleForce = elecount;
        FldForce = fldcount;
    }
    public override string ToString()
    {
        return ((int)protype.element).ToString() + ' ' + ((int)protype.field).ToString() + ' ' + EleForce.ToString() + ' ' + FldForce.ToString();
    }
    public static bool operator == (Skill sk1, Skill sk2)
    {
        if (sk1 as object == null || sk2 as object == null)
            return Equals(sk1,sk2);
        return sk1.protype == sk2.protype && sk1.FldForce == sk2.FldForce && sk1.EleForce == sk2.EleForce;
    }
    public static bool operator !=(Skill sk1, Skill sk2)
    {
        if (sk1 as object == null || sk2 as object == null)
            return !Equals(sk1, sk2);
        return sk1.protype != sk2.protype || sk1.FldForce != sk2.FldForce || sk1.EleForce != sk2.EleForce;
    }
    public string GetRank(out int i)
    {
        i = (EleForce + FldForce - 1) / 2;
        switch (i)
        {
            case 0: return "初阶";
            case 1: return "中阶";
            case 2: return "高阶";
            default: return "终极";
        }
    }
    public int MPcost
    //减的那个1是因为组合所需的两个必需元素算一个
    //重置之后费用太低，不减了
    //MP耗得太快了，又减了
    {
        get { return protype.MPcost * (EleForce + FldForce - 1); }
    }
    public string typeStr
    {
        get { return protype.typestr;}
    }
    public string nameStr
    {
        get { return protype.name; }
    }
    public string TranslateStr(string ori)
    {
        if (ori == null)
            return null;
        System.Text.StringBuilder sb = new System.Text.StringBuilder(16);
        for (int i = 0, j = 0; i < ori.Length; i++)
        {
            if (ori[i] == '{')
            {
                for (j = 0; ori[i + j] != '}'; j++)
                {
                    ;
                }
                sb.Append(((int)(System.Convert.ToSingle(ori.Substring(i + 1, j - 1)) * FldForce)).ToString());
                i += j;
            }
            else if (ori[i] == '[')
            {
                for (j = 0; ori[i + j] != ']'; j++)
                {
                    ;
                }
                sb.Append(((int)(System.Convert.ToSingle(ori.Substring(i + 1, j - 1)) * EleForce)).ToString());
                i += j;
            }
            else
                sb.Append(ori[i]);
        }
        return sb.ToString();
    }
    public string infoStr
    {
        get
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(128);
            for (int i = 0, j = 0; i < protype.info.Length; i++)
            {
                if (protype.info[i] == '{')
                {
                    for (j = 0; protype.info[i+j] != '}'; j++)
                        ;
                    sb.Append("<b>");
                    sb.Append(FldColorStr[(int)protype.field]);
                    sb.Append(
                        ((int)(System.Convert.ToSingle(
                            protype.info.Substring(i + 1, j - 1)) * FldForce)).ToString());
                    sb.Append(EndColotStr);
                    sb.Append("</b>");
                    i += j;
                }
                else if (protype.info[i] == '[')
                {
                    for (j = 0; protype.info[i + j] != ']'; j++)
                        ;
                    sb.Append("<b>");
                    sb.Append(EleColorStr[(int)protype.element]);
                    sb.Append(
                        ((int)(System.Convert.ToSingle(
                            protype.info.Substring(i + 1, j - 1)) * EleForce)).ToString());
                    sb.Append(EndColotStr);
                    sb.Append("</b>");
                    i += j;
                }
                else
                    sb.Append(protype.info[i]);
            }
            return sb.ToString();
        }
    }
    public List<SkillExecuteScript> execStripts
    {
        get
        {
            List<SkillExecuteScript> ret = new List<SkillExecuteScript>();
            SkillScript sp = protype.script;
            try
            {
                while (sp != null)
                {
                    
                    //----
                    SkillExecuteScript ses = new SkillExecuteScript();
                    ses.selector = sp.start.selector;
                    ses.filter = sp.start.filter;
                    ses.detector = sp.hit.detector;
                    ses.calculator = sp.value.calculator;
                    ses.executors = sp.result.todoList;
                    ses.distance = OnGame.WorldScale * System.Convert.ToSingle(TranslateStr(sp.start.distanceStr));
                    ses.range = OnGame.WorldScale * System.Convert.ToSingle(TranslateStr(sp.start.rangeStr));
                    ses.selectCount = (int)System.Convert.ToSingle(TranslateStr(sp.start.countStr));
                    ses.HitTitle = sp.hit.title;
                    ses.HitBaseA = TranslateStr(sp.hit.CombatBase_A);
                    ses.HitBaseB = TranslateStr(sp.hit.CombatBase_B);
                    ses.HitExA = TranslateStr(sp.hit.CombatExtra_A);
                    ses.HitExB = TranslateStr(sp.hit.CombatExtra_B);
                    ses.HitAdd = TranslateStr(sp.hit.CombatAdd);
                    ses.ValueTitle = sp.value.title;
                    ses.ValueDeltaA = TranslateStr(sp.value.deltaStrA);
                    ses.ValueDeltaB = TranslateStr(sp.value.deltaStrB);
                    ses.ValueExA = TranslateStr(sp.value.CombatExtra_A);
                    ses.ValueExB = TranslateStr(sp.value.CombatExtra_B);
                    ses.ValueAdd = TranslateStr(sp.value.CombatAdd);
                    foreach (string arg in sp.result.args)
                    {
                        ses.args.Add(TranslateStr(arg));
                    }
                    ret.Add(ses);
                    for (int recur = System.Convert.ToInt32(TranslateStr(sp.recur)); recur > 1; recur--)
                    {
                        //我觉得因为技能是固定的，所以这里没必要new一个新的，直接用这个引用就行了
                        ret.Add(ses);
                    }
                    //----
                    sp = sp.next;
                }
            }
            catch (System.Exception e)
            {
                OnGame.Log("技能“" + nameStr + "”在执行时出现问题:" + e.ToString());
                //OnScene.Report("技能“" + nameStr + "”在执行时出现问题:" + e.ToString());
            }
            return ret;
        }
    }
}

public class MagicBook
{
    public List<Skill> skills = new List<Skill>();
    public void Add(Skill skill)
    {
        skills.Add(skill);
    }
    public void Remove(Skill skill)
    {
        skills.Remove(skill);
    }
    public void Change(Skill old, Skill skill)
    {
        int index = skills.IndexOf(old);
        skills[index] = skill;
    }
    public int Count { get { return skills.Count; } }
}


