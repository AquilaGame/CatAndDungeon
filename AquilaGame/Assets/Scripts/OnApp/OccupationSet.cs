using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OccupationType
{
    Mage,
    Shaman,
    Warlock,
    Priest,
    Acolyte,
    Paladin,
    Darknight,
    Warrior,
    Hunter,
    Druid,
    Rogue
}

public class Occupation
{
    public OccupationType type = OccupationType.Mage;
    public int branch = 0;
    public static Occupation identity = new Occupation(OccupationType.Mage, 0);
    public Occupation(OccupationType _type,int _branch = 0)
    {
        type = _type;
        branch = _branch;
    }
    public static AlignmentType[] GetOccupationAlignmentList(OccupationType occ)
    {
        AlignmentType[] ret = null;
        switch (occ)
        {
            case OccupationType.Mage:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulGood, AlignmentType.NeutralGood, AlignmentType.ChaoticGood,
                    AlignmentType.LawfulNeutral, AlignmentType.TrueNeutral, AlignmentType.ChaoticNeutral };
                break;
            case OccupationType.Shaman:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulGood, AlignmentType.NeutralGood, AlignmentType.ChaoticGood,
                    AlignmentType.LawfulNeutral, AlignmentType.TrueNeutral, AlignmentType.ChaoticNeutral,
                    AlignmentType.LawfulEvil,AlignmentType.NeutralEvil,AlignmentType.ChaoticEvil
                };
                break;
            case OccupationType.Warlock:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulNeutral, AlignmentType.TrueNeutral, AlignmentType.ChaoticNeutral,
                    AlignmentType.LawfulEvil,AlignmentType.NeutralEvil,AlignmentType.ChaoticEvil
                };
                break;
            case OccupationType.Priest:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulGood,
                    AlignmentType.LawfulNeutral,
                };
                break;
            case OccupationType.Acolyte:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulEvil,AlignmentType.NeutralEvil,AlignmentType.ChaoticEvil
                };
                break;
            case OccupationType.Paladin:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulGood
                };
                break;
            case OccupationType.Darknight:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulEvil
                };
                break;
            case OccupationType.Warrior:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulGood, AlignmentType.NeutralGood, AlignmentType.ChaoticGood,
                    AlignmentType.LawfulNeutral, AlignmentType.TrueNeutral, AlignmentType.ChaoticNeutral,
                    AlignmentType.LawfulEvil,AlignmentType.NeutralEvil,AlignmentType.ChaoticEvil
                };
                break;
            case OccupationType.Hunter:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulGood, AlignmentType.NeutralGood, AlignmentType.ChaoticGood,
                    AlignmentType.LawfulNeutral, AlignmentType.TrueNeutral, AlignmentType.ChaoticNeutral,
                    AlignmentType.LawfulEvil,AlignmentType.NeutralEvil,AlignmentType.ChaoticEvil
                };
                break;
            case OccupationType.Druid:
                ret = new AlignmentType[] {
                    AlignmentType.LawfulGood, AlignmentType.NeutralGood,
                    AlignmentType.LawfulNeutral, AlignmentType.TrueNeutral
                };
                break;
            case OccupationType.Rogue:
                ret = new AlignmentType[] {
                    AlignmentType.NeutralGood, AlignmentType.ChaoticGood,
                    AlignmentType.TrueNeutral, AlignmentType.ChaoticNeutral,
                    AlignmentType.NeutralEvil,AlignmentType.ChaoticEvil
                };
                break;
            default:
                ret = null;
                break;
        }
        return ret;
    }
        public static string GetOcccupationName(OccupationType type)
    {
        switch (type)
        {
            case OccupationType.Mage:
                return "法师";
            case OccupationType.Shaman:
                return "萨满";
            case OccupationType.Warlock:
                return "术士";
            case OccupationType.Priest:
                return "牧师";
            case OccupationType.Acolyte:
                return "侍僧";
            case OccupationType.Paladin:
                return "圣骑士";
            case OccupationType.Darknight:
                return "黑武士";
            case OccupationType.Warrior:
                return "战士";
            case OccupationType.Hunter:
                return "猎人";
            case OccupationType.Druid:
                return "德鲁伊";
            case OccupationType.Rogue:
                return "游荡者";
            default:
                return "未知职业";
        }
    }
    public static string GetOcccupationInfo(OccupationType type)
    {
        switch (type)
        {
            case OccupationType.Mage:
                return "沉迷于搜集和掌握一切神秘深奥的知识，驱使魔法元素，并且利用它们创造凡人无法企及的奇迹。";
            case OccupationType.Shaman:
                return "能够通过古老的力量引导元素之力，召唤元素之灵，或是直接向自己的敌人倾泻元素洪流。";
            case OccupationType.Warlock:
                return "渴求力量，从而成为了是邪能魔法的践行者。诅咒、死灵和腐化的力量使你变得更为强大。";
            case OccupationType.Priest:
                return "在信仰和神迹的引领下传播秩序和光明，虔心崇拜，诸神之力就是你最强大的武器。";
            case OccupationType.Acolyte:
                return "自愿将灵魂奉献给伟大而黑暗的存在，忠诚地执行神灵的命令，哪怕是目睹自己的死亡。";
            case OccupationType.Paladin:
                return "使用利剑与邪恶进行不懈的斗争，你是骑士、虔信者和执法人，不惜以生命为代价维护秩序和正义。";
            case OccupationType.Darknight:
                return "是由邪能魔法造就的战士，被你的主宰驱策散播痛苦和仇恨，帮助他们冲破凡间的枷锁。";
            case OccupationType.Warrior:
                return "学习武器和防具的运用能力，研究格斗的技法，最终让自己变为最为强大的武器。";
            case OccupationType.Hunter:
                return "是知识渊博并且富有耐心的老练猎手。有着出色的洞察力和适应力，擅长在环境的遮蔽下猎杀敌人。";
            case OccupationType.Druid:
                return "擅长与对手进行周旋，施展增益、驱使野兽、驾驭治疗的力量，或是向敌人倾泻自然的愤怒，保护自然的平衡。";
            case OccupationType.Rogue:
                return "从不缺席每一场历险。你是暗示与诱导的专家，灵巧的杂耍艺人，又是如影随形的跟踪者。";
            default:
                return "未知职业";
        }
    }
    public static string GetOcccupationBranchIntro(OccupationType type)
    {
        switch (type)
        {
            case OccupationType.Mage:
                return "有些法师会专注于特定魔法领域的研究，成为该领域的大师；有些则渴望不受限制地掌控一切可以获取的魔法。";
            case OccupationType.Shaman:
                return "有些萨满祭司通过元素仪式与某种元素缔结契约，成为元素的唤醒者；而另一些则依靠布置强大的图腾掌控元素之力。";
            case OccupationType.Warlock:
                return "有些术士热衷强化自身与虚空的协调来获得召唤恐怖黑暗生物的能力，而另一些则沉迷于力量强大的邪恶魔法。";
            case OccupationType.Priest:
                return "牧师根据其信仰的神灵不同而有所各异，有些牧师擅长缓解痛苦，斥退死亡；而有些牧师则用自己的勇气捍卫神灵的荣耀。";
            case OccupationType.Acolyte:
                return "有些神灵会在收割生命时享受到愉悦；而另一些则喜好传播混乱与腐化，他们的侍从也自当如此。";
            case OccupationType.Paladin:
                return "有些圣武士被神灵选中，立誓铲除邪恶；有些则立志扶助弱小，为伙伴提供神圣的庇护。";
            case OccupationType.Darknight:
                return "黑暗魔法强化了黑武士的武器，而另一些黑武士则渴求敌人的鲜血。";
            case OccupationType.Warrior:
                return "战士执着于强化自己的力量，有些通过锻炼自己的体魄，有些磨砺自己的精神，从而让自己变得更为强大。";
            case OccupationType.Hunter:
                return "有些猎人擅长布置致命的陷阱；有些则擅长驯服体型与性情各异的野兽。";
            case OccupationType.Druid:
                return "有些德鲁伊喜欢运筹帷幄，用自然的力量干扰敌人，另一些则亲自化身可怕的猛兽，向敌人发动迅猛残忍的攻击。";
            case OccupationType.Rogue:
                return "游荡者擅长奇袭敌人，有些游荡者在陷阱中伺机待发；有些则深藏于暗影。";
            default:
                return "未知职业";
        }
    }
    public static string GetOcccupationBranchData(OccupationType type,int branch)
    {
        switch (branch)
        {
            case 0:
                switch (type)
                {
                    case OccupationType.Mage:
                        return "[领域专精]\n你的所有领域能力得到额外的 +1 强化等级";
                    case OccupationType.Shaman:
                        return "[元素仪式]\n当你释放一个元素法术时，你可以选择过载两个行动点，使法术再释放一次。";
                    case OccupationType.Warlock:
                        return "[扭曲虚空]\n你召唤的恶魔获得额外的2点全属性加值";
                    case OccupationType.Priest:
                        return "[祝福灵光]\n你的生命系法术获得额外的 +1 强化等级";
                    case OccupationType.Acolyte:
                        return "[死亡之拥]\n你的直接伤害法术造成的伤害提高 15%";
                    case OccupationType.Paladin:
                        return "[光耀剑刃]\n你的武力技能得到额外的 +1 强化等级";
                    case OccupationType.Darknight:
                        return "[邪灵契约]\n你在攻击非邪恶阵营目标造成伤害时，额外造成一次相当于原伤害 15% 的伤害";
                    case OccupationType.Warrior:
                        return "[强壮体魄]\n你的武力技能得到额外的 +1 强化等级， 此外，你的体质获得2点永久加值";
                    case OccupationType.Hunter:
                        return "[隐秘猎手]\n你的束缚系和工具系能力得到额外的 +1 强化等级";
                    case OccupationType.Druid:
                        return "[自然联结]\n你的野兽系和植物系能力得到额外的 +1 强化等级";
                    case OccupationType.Rogue:
                        return "[隐秘连环]\n当你对任意目标造成伤害时，恢复 %5 的MP（最少1点）";
                    default:
                        return "未知职业";
                }
            case 1:
                switch (type)
                {
                    case OccupationType.Mage:
                        return "[法力渴求]\n你施放一个法术后，为自身回复相当于其消耗 %25 的MP";
                    case OccupationType.Shaman:
                        return "[图腾潮涌]\n你召唤的图腾效果获得强化";
                    case OccupationType.Warlock:
                        return "[暗影邪能]\n你的混沌系和腐化系能力得到额外的 +1 强化等级，但每次使用对自身造成2点伤害";
                    case OccupationType.Priest:
                        return "[崇高信仰]\n你的秩序系和防护系能力获得额外的 +1 强化等级，但无法使用生命系和金属系法术";
                    case OccupationType.Acolyte:
                        return "[堕落混乱]\n你的腐化系能力得到额外的 +1 强化等级";
                    case OccupationType.Paladin:
                        return "[崇高之盾]\n你的防护系技能得到额外的 +1 强化等级";
                    case OccupationType.Darknight:
                        return "[鲜血激励]\n你在攻击非邪恶阵营目标造成伤害时，回复此次伤害15%的生命，最少1点";
                    case OccupationType.Warrior:
                        return "[不屈战意]\n当你进行异常状态检定时，获得 %33 的抵抗加成";
                    case OccupationType.Hunter:
                        return "[动物伙伴]\n你可以选择一个初始的动物伙伴";
                    case OccupationType.Druid:
                        return "[野性变身]\n当你使用变身技能时，你的力量和体质获得 2 点的额外加值";
                    case OccupationType.Rogue:
                        return "[暗影奇袭]\n你的黑暗系法术行动力消耗降低%40";
                    default:
                        return "未知职业";
                }
            default: return "未知分支";
        }

    }
    public static Attribute6 GetOcccupationBranchAttribute(OccupationType type)
    {
        switch (type)
        {
            case OccupationType.Mage:
                return new Attribute6(0, 0, 0, 2, 0, 0);
            case OccupationType.Shaman:
                return new Attribute6(0, 0, 0, 1, 0, 1);
            case OccupationType.Warlock:
                return new Attribute6(0, 0, 1, 1, 0, 0);
            case OccupationType.Priest:
                return new Attribute6(0, 0, 0, 0, 1, 1);
            case OccupationType.Acolyte:
                return new Attribute6(0, 0, 1, 0, 1, 0);
            case OccupationType.Paladin:
                return new Attribute6(1, 0, 0, 0, 1, 0);
            case OccupationType.Darknight:
                return new Attribute6(1, 0, 1, 0, 0, 0);
            case OccupationType.Warrior:
                return new Attribute6(1, 1, 1, 0, 0, 0);
            case OccupationType.Hunter:
                return new Attribute6(0, 1, 0, 0, 1, 0);
            case OccupationType.Druid:
                return new Attribute6(0, 0, 0, 0, 1, 1);
            case OccupationType.Rogue:
                return new Attribute6(0, 1, 0, 0, 1, 1);
            default:
                return new Attribute6(0, 0, 0, 0, 0, 0);
        }
    }

    public static List<ElementType> GetOcccupationElementList(Occupation occupation)
    {
        switch (occupation.type)
        {
            case OccupationType.Mage:
                return new List<ElementType> {ElementType.Fire,ElementType.Water,ElementType.Air,ElementType.Metal};
            case OccupationType.Shaman:
                return new List<ElementType> {ElementType.Fire,ElementType.Water,ElementType.Air,ElementType.Chaos};
            case OccupationType.Warlock:
                return new List<ElementType> {ElementType.Fire,ElementType.Chaos,ElementType.Dead,ElementType.Corrupt };
            case OccupationType.Priest:
                if(occupation.branch == 1)
                    return new List<ElementType> { ElementType.Order, ElementType.Light };
                else
                    return new List<ElementType> { ElementType.Order, ElementType.Light, ElementType.Life, ElementType.Metal };
            case OccupationType.Acolyte:
                return new List<ElementType> {ElementType.Chaos,ElementType.Dark,ElementType.Dead,ElementType.Corrupt};
            case OccupationType.Paladin:
                return new List<ElementType> {ElementType.Fire,ElementType.Order,ElementType.Light,ElementType.Life,ElementType.Metal };
            case OccupationType.Darknight:
                return new List<ElementType> {ElementType.Chaos,ElementType.Dark,ElementType.Dead,ElementType.Corrupt};
            case OccupationType.Warrior:
                return new List<ElementType> {ElementType.Fire,ElementType.Water,ElementType.Air,ElementType.Metal};
            case OccupationType.Hunter:
                return new List<ElementType> {ElementType.Water,ElementType.Earth };
            case OccupationType.Druid:
                return new List<ElementType> {ElementType.Air,ElementType.Earth,ElementType.Life,ElementType.Dead,ElementType.Water};
            case OccupationType.Rogue:
                return new List<ElementType> {ElementType.Air,ElementType.Chaos,ElementType.Dark,ElementType.Metal};
            default:
                return new List<ElementType> { };
        }
    }

    public static List<FieldType> GetOcccupationFieldList(OccupationType type)
    {
        switch (type)
        {
            case OccupationType.Mage:
                return new List<FieldType> {FieldType.Shield,FieldType.Move,FieldType.Trap,FieldType.Sense,FieldType.Tool,FieldType.Energy };
            case OccupationType.Shaman:
                return new List<FieldType> {FieldType.Move,FieldType.Trap,FieldType.Sense,FieldType.Energy,FieldType.Wild,FieldType.Plant };
            case OccupationType.Warlock:
                return new List<FieldType> {FieldType.Move,FieldType.Trap,FieldType.Sense,FieldType.Energy,FieldType.Wild,FieldType.Plant };
            case OccupationType.Priest:
                return new List<FieldType> { FieldType.Shield, FieldType.Move, FieldType.Sense, FieldType.Tool, FieldType.Wild, FieldType.Plant };
            case OccupationType.Acolyte:
                return new List<FieldType> { FieldType.Shield, FieldType.Move, FieldType.Sense, FieldType.Tool, FieldType.Wild, FieldType.Plant };
            case OccupationType.Paladin:
                return new List<FieldType> {FieldType.Sword,FieldType.Shield,FieldType.Trap,FieldType.Energy};
            case OccupationType.Darknight:
                return new List<FieldType> {FieldType.Sword,FieldType.Shield,FieldType.Trap,FieldType.Energy,FieldType.Wild};
            case OccupationType.Warrior:
                return new List<FieldType> {FieldType.Shield,FieldType.Sword};
            case OccupationType.Hunter:
                return new List<FieldType> {FieldType.Shield,FieldType.Move,FieldType.Trap,FieldType.Sense,FieldType.Tool,FieldType.Wild};
            case OccupationType.Druid:
                return new List<FieldType> {FieldType.Sword,FieldType.Shield,FieldType.Trap,FieldType.Wild,FieldType.Plant};
            case OccupationType.Rogue:
                return new List<FieldType> {FieldType.Sword,FieldType.Move,FieldType.Trap,FieldType.Sense,FieldType.Tool};
            default:
                return new List<FieldType> { };
        }
    }
    public static int GetOcccupationMaxSkillCount(Character character)
    {
        return GetOcccupationMaxSkillCount(character.OccupationData.type, character.BaseData.baseAttribute);
    }
    public static int GetOcccupationMaxSkillCount(OccupationType type, Attribute6 attribute)
    {
        switch (type)
        {
            //主施法职业，智力影响最大技能数，且额外获得数量+1
            case OccupationType.Mage:
                return attribute._int / 4 + 1;
            case OccupationType.Shaman:
                return attribute._int / 4 + 1;
            case OccupationType.Warlock:
                return attribute._int / 4 + 1;
            //神职和半神职职业，感知影响最大技能数
            case OccupationType.Priest:
                return attribute._wis / 4;
            case OccupationType.Acolyte:
                return attribute._wis / 4;
            case OccupationType.Paladin:
                return attribute._wis / 4;
            case OccupationType.Druid:
                return attribute._wis / 4;
            //战士，力量影响最大技能数
            case OccupationType.Warrior:
                return attribute._str / 4;
            case OccupationType.Darknight:
                return attribute._str / 4;
            //敏捷职业，敏捷影响最大技能数
            case OccupationType.Hunter:
                return attribute._dex / 4;
            case OccupationType.Rogue:
                return attribute._dex / 4;
            default:
                return 0;
        }
    }

    public static int GetOcccupationMaxSkillPower(OccupationType type, Attribute6 attribute)
    {
        switch (type)
        {
            //暂定5级一个法术Force，15级以上中阶,25级以上高阶，加上天赋可以到达终极
            //主施法职业，智力影响法术等级
            case OccupationType.Mage:
                return attribute._int / 5;
            case OccupationType.Shaman:
                return attribute._int / 5;
            case OccupationType.Warlock:
                return attribute._int / 5;
            //神职和半神职职业，感知影响法术等级
            case OccupationType.Priest:
                return attribute._wis / 5;
            case OccupationType.Acolyte:
                return attribute._wis / 5;
            case OccupationType.Paladin:
                return attribute._wis / 5;
            case OccupationType.Druid:
                return attribute._wis / 5;
            //战士，力量影响法术等级
            case OccupationType.Warrior:
                return attribute._str / 5;
            case OccupationType.Darknight:
                return attribute._str / 5;
            //敏捷职业，敏捷影响法术等级
            case OccupationType.Hunter:
                return attribute._dex / 5;
            case OccupationType.Rogue:
                return attribute._dex / 5;
            default:
                return 0;
        }
    }
    public static int GetBranchAddForce(Occupation occupation, ElementType elementType, FieldType fieldType)
    {
        //元素+1返回1，领域+1返回2，不加值返回0
        switch (occupation.branch)
        {
            case 0:
                switch (occupation.type)
                {
                    case OccupationType.Mage:
                        return 2;
                    case OccupationType.Shaman:
                        return 0;
                    case OccupationType.Warlock:
                        return 0;
                    case OccupationType.Priest:
                        return elementType == ElementType.Life ? 1 : 0;
                    case OccupationType.Acolyte:
                        return 0;
                    case OccupationType.Paladin:
                        return fieldType == FieldType.Sword ? 2 : 0;
                    case OccupationType.Darknight:
                        return 0;
                    case OccupationType.Warrior:
                        return fieldType == FieldType.Sword ? 2 : 0;
                    case OccupationType.Hunter:
                        return fieldType == FieldType.Tool || fieldType == FieldType.Trap ? 2 : 0;
                    case OccupationType.Druid:
                        return fieldType == FieldType.Wild || fieldType == FieldType.Plant ? 2 : 0;
                    case OccupationType.Rogue:
                        return 0;
                    default:
                        return 0;
                }
            case 1:
                switch (occupation.type)
                {
                    case OccupationType.Mage:
                        return 0;
                    case OccupationType.Shaman:
                        return 0;
                    case OccupationType.Warlock:
                        return elementType == ElementType.Chaos || elementType == ElementType.Corrupt ? 1 : 0;
                    case OccupationType.Priest:
                        if (elementType == ElementType.Order)
                            return 1;
                        else if (fieldType == FieldType.Shield)
                            return 2;
                        else
                            return 0;
                    case OccupationType.Acolyte:
                        return elementType == ElementType.Corrupt ? 1 : 0;
                    case OccupationType.Paladin:
                        return fieldType == FieldType.Shield ? 2 : 0;
                    case OccupationType.Darknight:
                        return 0;
                    case OccupationType.Warrior:
                        return 0;
                    case OccupationType.Hunter:
                        return 0;
                    case OccupationType.Druid:
                        return 0;
                    case OccupationType.Rogue:
                        return 0;
                    default:
                        return 0;
                }
            default: return 0;
        }
    }
}
