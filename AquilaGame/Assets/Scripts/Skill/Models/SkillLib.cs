using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static System.Convert;
public class SkillLib : MonoBehaviour
{
    public List<Color> EleColors = null;
    public List<Color> FldColors = null;
    public List<Sprite> EleSprites = null;
    public List<Sprite> FldSprites = null;
    public List<Color> SkillRankColors = null;
    public GameObject SkillInfoWindowObj = null;
    private void Awake()
    {
        OnGame.MagicLib = this;
        OnGame.Log("法术素材库建立连接");
    }
    static string RealStr(string s)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();
        for (int i = 0; i < s.Length; i++)
        {
            if ((s[i] >= '0' && s[i] <= '9') || s[i] == '%' || s[i] == '{' || s[i] == '}' || s[i] == '[' || s[i] == ']' || s[i] == '-' || s[i] == '+' || s[i] == '.')
            {
                builder.Append(s[i]);
            }
            else if (s[i] == '(')
            {
                while (s[i] != ')')
                {
                    builder.Append(s[i]);
                    i++;
                    if (i >= s.Length)
                        throw new System.Exception("找不到匹配的右括号：" + s);
                }
                builder.Append(')');
            }
        }
        s = builder.ToString();
        if (string.IsNullOrEmpty(s))
            throw new System.Exception("找不到有效的参数：" + s);
        return s;
    }

    static SkillStartScript GetStartScript(string s, out string count)
    {
        if (s[0] != '&')
            throw new System.Exception("未找到合适的起始符（'&'）");

        string[] ss = s.Split(' ');
        if (ss[0].Length > 1)
            count = ss[0].Substring(1);
        else
            count = "1";

        switch (ss[1])
        {
            case "自身":
                return new SkillStartScript();
            case "选择目标":
                return new SkillStartScript(RealStr(ss[2]), RealStr(ss[3]), ss[4]);
            case "自身周围":
                return new SkillStartScript(RealStr(ss[2]), ss[3]);
            case "选择区域":
                return new SkillStartScript(RealStr(ss[2]), RealStr(ss[3]), null, ss[4]);
            default:
                throw new System.Exception("不正确的选择器Type");
        }
    }

    static SkillHitScript GetHitScript(string s)
    {
        string[] ss = s.Split(' ');
        switch (ss[0])
        {
            case "-必中":
                return new SkillHitScript();
            case "-对抗":
                switch (ss.Length)
                {
                    //完全模式，一共8个参数：说明符 title baseA exA VS baseB exB add
                    case 8:
                        return new SkillHitScript(ss[1], RealStr(ss[2]), RealStr(ss[3]), ss[4], RealStr(ss[5]), RealStr(ss[6]), RealStr(ss[7]));
                    //简写模式：一共5个参数：说明符 title baseA VS baseB
                    case 5:
                        return new SkillHitScript(ss[1], RealStr(ss[2]), ss[3], RealStr(ss[4]));
                    //极简模式：一共3个参数：说明符 title 属性
                    case 3:
                        return new SkillHitScript(ss[1], ss[2], "VS");
                    default:
                        throw new System.Exception("参数的数目不正确");
                }
            default:
                throw new System.Exception("不正确的检测器Type");
        }
    }

    static SkillValueScript GetValueScript(string s)
    {
        string[] ss = s.Split(' ');
        switch (ss[0])
        {
            case "-数值":
                return new SkillValueScript(ss[1]);
            case "-对抗":
                switch (ss.Length)
                {
                    //完全模式，一共8个参数：说明符 title baseA exA VS baseB exB add
                    case 8:
                        return new SkillValueScript(ss[1], RealStr(ss[2]), RealStr(ss[3]), ss[4], RealStr(ss[5]), RealStr(ss[6]), RealStr(ss[7]));
                    //简写模式：一共5个参数：说明符 title baseA VS baseB
                    case 5:
                        return new SkillValueScript(ss[1], RealStr(ss[2]), ss[3], RealStr(ss[4]));
                    //极简模式：一共3个参数：说明符 title 属性
                    case 3:
                        return new SkillValueScript(ss[1], ss[2], "VS");
                    default:
                        throw new System.Exception("参数的数目不正确");
                }
            case "-随机":
                return new SkillValueScript(ss[1], ss[2]);
            default:
                throw new System.Exception("不正确的计算器Type");
        }
    }

    static SkillResultScript GetResultScript(string s)
    {
        return new SkillResultScript(s);
    }



    private void Start()
    {
        LoadSkills();
    }

    void LoadSkills()
    {
        int readstep = 0;
        ElementType element = ElementType.Air;
        FieldType field = FieldType.Energy;
        int cost = 0;
        string namestr = "";
        string typestr = "";
        string infostr = "";
        bool hasError = true;
        int line = 0;
        SkillStartScript skillStartScript = null;
        SkillHitScript skillHitScript = null;
        SkillValueScript skillValueScript = null;
        SkillResultScript skillResultScript = null;
        SkillScript skillScript = null;
        SkillScript sp = null;
        string recur = "0";
        using (StreamReader sr = new StreamReader("default\\skills.skillset"))
        {
            while (!sr.EndOfStream)
            {
                line++;
                string s = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(s) || s[0] == '#')
                    continue;
                try
                {
                    switch (readstep)
                    {
                        //读取Element和Field，或者非通用技能名字
                        case 0:
                            if (s[0] == '@')
                            {
                                hasError = false;
                                bool isFind = false;
                                for (int i = 0; i < Skill.EleNames.Length; i++)
                                {
                                    if (s.Contains(Skill.EleNames[i]))
                                    {
                                        isFind = true;
                                        element = (ElementType)i;
                                        break;
                                    }
                                }
                                if (!isFind)
                                {
                                    hasError = true;
                                    throw new System.Exception("读取技能文件第"+line.ToString()+"行发生错误：找不到合适的Element");
                                }
                                for (int i = 0; i < Skill.FldNames.Length; i++)
                                {
                                    if (s.Contains(Skill.FldNames[i]))
                                    {
                                        isFind = true;
                                        field = (FieldType)i;
                                        break;
                                    }
                                }
                                if (!isFind)
                                {
                                    hasError = true;
                                    throw new System.Exception("读取技能文件第" + line.ToString() + "行发生错误：找不到合适的Field");
                                }
                                if (!hasError)
                                {
                                    readstep++;
                                }
                            }
                            break;
                        //技能消耗
                        case 1:
                            if (int.TryParse(s, out cost))
                            {
                                readstep++;
                            }
                            else
                            {
                                readstep = 0;
                                throw new System.Exception("读取技能文件第" + line.ToString() + "行发生错误：技能消耗不为可读取的整数");
                            }
                            break;
                        //技能名字
                        case 2:
                            namestr = s;
                            readstep++;
                            break;
                        //技能类别
                        case 3:
                            typestr = s;
                            readstep++;
                            break;
                        //技能脚本第一行 选择目标
                        case 4:
                            try
                            {
                                skillStartScript = GetStartScript(s, out recur);
                            }
                            catch (System.Exception e)
                            {
                                throw new System.Exception("读取技能文件第" + line.ToString() + "行发生错误：" + e.ToString());
                            }
                            readstep++;
                            break;
                        //技能脚本第二行 命中方式
                        case 5:
                            try
                            {
                                skillHitScript = GetHitScript(s);
                            }
                            catch (System.Exception e)
                            {
                                throw new System.Exception("读取技能文件第" + line.ToString() + "行发生错误：" + e.ToString());
                            }
                            readstep++;
                            break;
                        //技能脚本第三行 数值
                        case 6:
                            try
                            {
                                skillValueScript = GetValueScript(s);
                            }
                            catch (System.Exception e)
                            {
                                throw new System.Exception("读取技能文件第" + line.ToString() + "行发生错误：" + e.ToString());
                            }
                            readstep++;
                            break;
                        //技能脚本第四行 命令流
                        case 7:
                            try
                            {
                                skillResultScript = GetResultScript(s);
                            }
                            catch (System.Exception e)
                            {
                                throw new System.Exception("读取技能文件第" + line.ToString() + "行发生错误：" + e.ToString());
                            }
                            if (sp == null)
                            {
                                skillScript = new SkillScript(skillStartScript, skillHitScript, skillValueScript, skillResultScript, recur);
                                sp = skillScript;
                            }
                            else
                            {
                                sp.next = new SkillScript(skillStartScript, skillHitScript, skillValueScript, skillResultScript, recur);
                                sp = sp.next;
                            }
                            readstep++;
                            break;
                        //这一行可能是多组的技能脚本，也可能是info
                        case 8:
                            if (s[0] == '&')
                            {
                                try
                                {
                                    skillStartScript = GetStartScript(s, out recur);
                                }
                                catch (System.Exception e)
                                {
                                    throw new System.Exception("读取技能文件第" + line.ToString() + "行发生错误：" + e.ToString());
                                }
                                readstep = 5;
                                break;
                            }
                            else
                            {
                                infostr = s;
                                readstep++;
                                break;
                            }
                        default://无论里面加了什么东西，找到以@开头的最后一行
                            if (s[0] == '@')
                            {
                                if (!hasError)
                                {
                                    Skill.ProtypeList[(int)element, (int)field] =
                                        new SkillProtype(element, field, skillScript, cost, typestr, namestr, infostr);
                                    skillScript = null;
                                    sp = null;
                                }
                                readstep = 0;
                            }
                            break;
                    }
                }
                catch(System.Exception e)
                {
                    OnGame.Log(e.ToString());
                }
            }
            OnGame.Log("外部法术数据读取结束");
        }
    }

}
