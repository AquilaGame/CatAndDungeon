using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Convert;

public class SkillExecutor
{
    List<SkillExecuteScript> nowScripts = null;
    List<CharacterScript> nowTargets = new List<CharacterScript>();
    List<int> nowValues = new List<int>();
    int nowProgress = 0;
    int nowProcessing = 0;
    Skill nowSkill = null;
    Vector3 nowSelectPosition = Vector3.zero;
    CharacterScript nowCharacter = null;
    CharacterAttributeTranslator translator = new CharacterAttributeTranslator();
    bool isReported = false;
    SkillExecuteScript nowScript
    {
        get { return nowScripts[nowProgress]; }
    }

    public void SetSkillPreview(CharacterScript cs, List<SkillExecuteScript> scripts)
    {
        if (scripts.Count > 0)
        {
            SkillExecuteScript script = scripts[0];
            switch (script.selector)
            {
                case SkillStartScript.Selector.Self:
                    OnScene.manager.CharacterSelector.SetRangePreview(cs, OnGame.WorldScale);
                    break;
                case SkillStartScript.Selector.Neighbor:
                    OnScene.manager.CharacterSelector.SetRangePreview(cs, script.range * 2);
                    break;
                case SkillStartScript.Selector.Targets:
                    OnScene.manager.CharacterSelector.SetRangePreview(cs, script.distance * 2);
                    break;
                case SkillStartScript.Selector.Range:
                    OnScene.manager.CharacterSelector.SetRangePreview(cs, script.distance * 2);
                    break;
            }
        }
    }
    public void ResetSkillPreview()
    {
        OnScene.manager.CharacterSelector.ResetRangePreview();
    }

    public void StartExecute(CharacterScript cs, List<SkillExecuteScript> scripts,Skill skill)
    {
        isReported = false;
        nowCharacter = cs;
        nowScripts = scripts;
        nowProgress = 0;
        nowProcessing = 0;
        nowSkill = skill;
        nowTargets.Clear();
        nowValues.Clear();
        if (nowScript.filter == SkillStartScript.Filter.Friendly || nowScript.executors.Contains(SkillResultScript.Executor.Heal))
        {
            Sound.PlayBuff();
        }
        else if (nowScript.filter == SkillStartScript.Filter.Enemy || nowScript.filter == SkillStartScript.Filter.All)
        {
            nowCharacter.PlaySound(CharacterSoundType.Attack);
        }
        switch (nowScript.selector)
        {
            case SkillStartScript.Selector.Self:
                AfterSelect(new List<CharacterScript> {nowCharacter}, Vector3.zero);
                break;
            case SkillStartScript.Selector.Neighbor:
                OnScene.manager.CharacterSelector.StartCatch(nowCharacter, nowScript.filter, AfterSelect, nowScript.range);
                break;
            case SkillStartScript.Selector.Targets:
                OnScene.manager.CharacterSelector.StartCatch(nowCharacter, nowScript.filter, AfterSelect,nowScript.distance,nowScript.selectCount);
                break;
            case SkillStartScript.Selector.Range:
                OnScene.manager.CharacterSelector.StartCatch(nowCharacter, nowScript.filter, AfterSelect,nowScript.distance,nowScript.range);
                //这里还要注意处理选择区域时的事件，用于召唤和召唤物体类的技能。
                break;
        }


        //委托方式执行
    }

    public void ContinueExecute()
    {
        nowProgress++;
        if (nowProgress >= nowScripts.Count)
            return;
        nowProcessing = 0;
        nowTargets.Clear();
        nowValues.Clear();
        nowSelectPosition = Vector3.zero;
        switch (nowScript.selector)
        {
            case SkillStartScript.Selector.Self:
                AfterSelect(new List<CharacterScript> { nowCharacter }, Vector3.zero);
                break;
            case SkillStartScript.Selector.Neighbor:
                OnScene.manager.CharacterSelector.StartCatch(nowCharacter, nowScript.filter, AfterSelect, nowScript.range);
                break;
            case SkillStartScript.Selector.Targets:
                OnScene.manager.CharacterSelector.StartCatch(nowCharacter, nowScript.filter, AfterSelect, nowScript.distance, nowScript.selectCount);
                break;
            case SkillStartScript.Selector.Range:
                OnScene.manager.CharacterSelector.StartCatch(nowCharacter, nowScript.filter, AfterSelect, nowScript.distance, nowScript.range);
                //这里还要注意处理选择区域时的事件，用于召唤和召唤物体类的技能。
                break;
        }
        //委托方式执行
    }

    //选择器返回一个指定Character的列表
    void AfterSelect(List<CharacterScript> targets, Vector3 pos)
    {
        //如果一个技能能进行到这里，那说明选择没有问题，就可以在这里进行MP的结算了
        if (!isReported)
        {
            nowCharacter.SkillUseConfirm(nowSkill);
            isReported = true;
        }
        nowSelectPosition = pos;
        //如果filter是无的话，会被认为是一个纯召唤技能，纯召唤技能的目标为自身
        if (nowScript.filter == SkillStartScript.Filter.None)
        {
            targets.Add(nowCharacter);
        }
        nowTargets = targets;
        if (nowTargets.Count == 0)
            return;
        switch (nowScript.detector)
        {
            case SkillHitScript.Detector.Forced:
                foreach (var cs in nowTargets)
                {
                    nowValues.Add(0);
                }
                AfterDetectHit();
                break;
            case SkillHitScript.Detector.Combat:
                //此时的nowProcessing应该是0
                ConstructHitCombatPanel(nowProcessing);
                break;
        }
    }

    void ConstructHitCombatPanel(int i)
    {
        translator.Set(nowCharacter.data, nowTargets[i].data);
        CombatPanel combatPanel = OnScene.manager.OpenCombatPanel();
        combatPanel.SetUp(nowScript.HitTitle, nowCharacter.data.Name, nowTargets[i].data.Name,
            ToInt32(translator.Convert(nowScript.HitBaseA)), ToInt32(translator.Convert(nowScript.HitBaseB)),
            ProcessHitCombat, nowScript.HitExA, nowScript.HitExB, nowScript.HitAdd);
    }

    //处理每一个对抗命中检测
    void ProcessHitCombat(int result)
    {
        OnScene.Report("<color=#CC44EE>[命中骰结果]: </color><color=Orange>[" 
            + nowCharacter.data.Name + "]</color>对<color=Yellow>[" + nowTargets[nowProcessing].data.Name 
            + "]</color>使用的技能的命中骰结果为:<color=#FF88FF><b>" + result.ToString() + "</b></color>");
        if (result > 0)
            nowValues.Add(0);
        else
            nowValues.Add(-1);
        nowProcessing++;
        if (nowProcessing >= nowTargets.Count)
        {
            nowProcessing = 0;
            AfterDetectHit();
        }
        else
        {
            ConstructHitCombatPanel(nowProcessing);
        }
    }

    void AfterDetectHit()
    {
        switch (nowScript.calculator)
        {
            case SkillValueScript.Calculator.Number:
                for (nowProcessing = 0; nowProcessing < nowValues.Count; nowProcessing++)
                {
                    if (nowValues[nowProcessing] != -1)
                    {
                        translator.Set(nowCharacter.data, nowTargets[nowProcessing].data);
                        nowValues[nowProcessing] = ToInt32(translator.Convert(nowScript.ValueDeltaA));
                    }
                }
                nowProcessing = 0;
                AfterCalculate();
                break;
            case SkillValueScript.Calculator.Combat:
                //找到第一个命中的目标打开窗口
                for (nowProcessing = 0; nowProcessing < nowValues.Count; nowProcessing++)
                {
                    if (nowValues[nowProcessing] != -1)
                    {
                        ConstructValueCombatPanel(nowProcessing);
                        break;
                    }
                }
                break;
            case SkillValueScript.Calculator.Random:
                //找到第一个命中的目标打开窗口
                for (nowProcessing = 0; nowProcessing < nowValues.Count; nowProcessing++)
                {
                    if (nowValues[nowProcessing] != -1)
                    {
                        ConstructValueRandomPanel(nowProcessing);
                        break;
                    }
                }
                break;
        }

    }

    void ConstructValueCombatPanel(int i)
    {
        translator.Set(nowCharacter.data, nowTargets[i].data);
        CombatPanel combatPanel = OnScene.manager.OpenCombatPanel();
        combatPanel.SetUp(nowScript.ValueTitle, nowCharacter.data.Name, nowTargets[i].data.Name,
            ToInt32(translator.Convert(nowScript.ValueDeltaA)), ToInt32(translator.Convert(nowScript.ValueDeltaB)),
            ProcessValueCombat, nowScript.ValueExA, nowScript.ValueExB, nowScript.ValueAdd);
    }

    //处理每一个对抗数值计算
    void ProcessValueCombat(int result)
    {
        OnScene.Report("<color=#CC44EE>[数值骰结果]: </color><color=Orange>[" + nowCharacter.data.Name
            + "]</color>对<color=Yellow>[" + nowTargets[nowProcessing].data.Name +
            "]</color>使用的技能的数值骰结果为:<color=#FF88FF><b>" + result.ToString() + "</b></color>");
        if (result > 0)
            nowValues[nowProcessing] = result;
        else
            //这里采用命中至少造成一点效果的机制
            nowValues[nowProcessing] = 1;
        //找到下一个命中的目标打开窗口
        for (nowProcessing++; nowProcessing < nowValues.Count; nowProcessing++)
        {
            if (nowValues[nowProcessing] != -1)
            {
                ConstructValueCombatPanel(nowProcessing);
                break;
            }
        }
        //如果找不到新的了
        if (nowProcessing >= nowTargets.Count)
        {
            nowProcessing = 0;
            AfterCalculate();
        }
    }

    void ConstructValueRandomPanel(int i)
    {
        int faces;
        int rounds;
        string add1;
        string add2;
        //格式是2+1d4+2这种，先按照d分开
        string[] ss = nowScript.ValueDeltaA.Split('d');
        string[] fs = ss[0].Split('+');
        string[] es = ss[1].Split('+');
        if (fs.Length == 2)
        {
            add1 = fs[0];
            rounds = ToInt32(fs[1]);
        }
        else
        {
            add1 = "0";
            rounds = ToInt32(fs[0]);
        }
        if (es.Length == 2)
        {
            faces = ToInt32(es[0]);
            add2 = es[1];
        }
        else
        {
            faces = ToInt32(es[0]);
            add2 = "0";
        }

        RollPanel rollPanel = OnScene.manager.OpenRollPanel();
        rollPanel.SetUp(nowScript.ValueTitle + "  目标：" + nowTargets[i].data.Name, ProcessValueRoll,
            faces, rounds, add1, add2);
    }

    //处理每一个随机数值计算
    void ProcessValueRoll(int result)
    {
        OnScene.Report("<color=#CC44EE>[数值骰结果]: </color><color=Orange>[" + nowCharacter.data.Name + "]</color>对<color=Yellow>[" 
            + nowTargets[nowProcessing].data.Name + "]</color>使用的技能的数值骰结果为:<color=#FF88FF><b>" + result.ToString()+"</b></color>点。");
        if (result > 0)
            nowValues[nowProcessing] = result;
        else
            //这里采用命中至少造成一点效果的机制
            nowValues[nowProcessing] = 1;
        //找到下一个命中的目标打开窗口
        for (nowProcessing++; nowProcessing < nowValues.Count; nowProcessing++)
        {
            if (nowValues[nowProcessing] != -1)
            {
                ConstructValueRandomPanel(nowProcessing);
                break;
            }
        }
        //如果找不到新的了
        if (nowProcessing >= nowTargets.Count)
        {
            nowProcessing = 0;
            AfterCalculate();
        }
    }


    void AfterCalculate()
    {
        bool isCalling = false;
        string CallName = "";
        float CallArgv = 0;
        bool isCallingObj = false;
        float CallObjArgv1 = 0;
        float CallObjArgv2 = 0;
        string CallObjName = "";
        for (int ch = 0; ch < nowTargets.Count; ch++)
        {
            if (nowValues[ch] != -1)
            {
                //结果替代，生成最终的命令列表
                string[] args = new string[nowScript.args.Count];
                for (int i = 0; i < args.Length; i++)
                {
                    //这里用的是直接替代，也可以换成替换字符
                    if (nowScript.args[i] == "结果值")
                        args[i] = nowValues[ch].ToString();
                    else
                        args[i] = nowScript.args[i];
                }
                int p = 0;
                foreach (var v in nowScript.executors)
                {
                    switch (v)
                    {
                        case SkillResultScript.Executor.Damage:
                            int damage = ToInt32(args[p]);
                            if (nowCharacter.data.OccupationData.type == OccupationType.Acolyte && nowCharacter.data.OccupationData.branch == 0)
                                damage = (int)(damage * 1.15);
                            OnScene.Report("<color=#CC44EE>[技能结算]: </color><color=Orange>[" + nowCharacter.data.Name + "]</color>的技能<color=Yellow>[" 
                                + nowSkill.nameStr + "]</color>对<color=#FF4444>[" + nowTargets[ch].data.Name + "]</color>造成了<color=#FF1111><b>"
                                + damage.ToString() + "</b></color>点伤害。");
                            nowTargets[ch].HPGetDamage(damage);
                            nowCharacter.AfterCauseDamage(nowTargets[ch], damage);
                            p += 1;
                            break;
                        case SkillResultScript.Executor.Heal:
                            OnScene.Report("<color=#CC44EE>[技能结算]: </color><color=#33FF33>[" + nowTargets[ch].data.Name + "]</color>受到了来自<color=Orange>[" 
                                + nowCharacter.data.Name + "]</color>的技能<color=Yellow>[" + nowSkill.nameStr + "]</color>的<color=#99FF99><b>" 
                                + args[p] + "</b></color>点治疗。");
                            nowTargets[ch].HPGetHeal(ToInt32(args[p]));
                            p += 1;
                            break;
                        case SkillResultScript.Executor.AddState:
                            nowTargets[ch].AddBuff(args[p], ToInt32(args[p + 1]), ToSingle(args[p + 2]));
                            p += 3;
                            break;
                        case SkillResultScript.Executor.Call:
                            isCalling = true;
                            CallName = args[p];
                            CallArgv = ToSingle(args[p + 1]);
                            p += 2;
                            break;
                        case SkillResultScript.Executor.CallObj:
                            isCallingObj = true;
                            CallObjName = args[p];
                            CallObjArgv1 = ToSingle(args[p + 1]);
                            CallObjArgv2 = ToSingle(args[p + 2]);
                            p += 3;
                            break;
                        case SkillResultScript.Executor.Report:
                            translator.Set(nowCharacter.data, nowTargets[ch].data);
                            System.Text.StringBuilder sb = new System.Text.StringBuilder(64);
                            sb.Append("<color=#AAFFFF>[技能提示]:</color>");
                            for (int i = 0; i < args[p].Length; i++)
                            {
                                //从头扫描扫描到一个(
                                if (args[p][i] == '(')
                                {
                                    //找到'('对应的')'，或找到结尾
                                    for (int j = 1; i + j < args[p].Length; j++)
                                    {
                                        if (args[p][i + j] == ')')
                                        {
                                            //找到了的话就翻译
                                            sb.Append("<color=Yellow>[");
                                            sb.Append(translator.Convert(args[p].Substring(i, j + 1)));
                                            sb.Append("</color>]");
                                            i = i + j + 1;
                                            break;
                                        }
                                    }
                                    //没找到结尾的话就原样复制下来
                                    //sb.Append(args[p][i]);
                                }
                                else
                                {
                                    sb.Append(args[p][i]);
                                }
                            }
                            OnScene.Report(sb.ToString());
                            p += 1;
                            break;
                    }
                }
            }
        }
        //处理召唤事件
        if (isCalling)
        {
            OnScene.manager.SummonCall(nowSelectPosition, Quaternion.identity, nowCharacter, CallName, CallArgv);
        }
        if (isCallingObj)
        {
            OnScene.manager.SummonObjCall(nowSelectPosition, Quaternion.identity, nowCharacter, CallObjName, CallObjArgv1, CallObjArgv2);
        }
        //后面接第二个技能效果
        ContinueExecute();
    }
}

