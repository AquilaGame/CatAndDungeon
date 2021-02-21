using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AlignmentType
{
    LawfulGood,
    NeutralGood,
    ChaoticGood,
    LawfulNeutral,
    TrueNeutral,
    ChaoticNeutral,
    LawfulEvil,
    NeutralEvil,
    ChaoticEvil
}
public class Alignment
{
    public static Color[] ColorList =
{
        new Color(0.8679245f, 0.02046992f, 0.2820109f),
        new Color(0.8980392f, 0.8252381f, 0.2509804f),
        new Color(0,0.7486f,1),
        new Color(0.2f,1.0f,0.2f)
    };
    public static string GetAlignmentName(AlignmentType type)
    {
        switch (type)
        {
            case AlignmentType.LawfulGood: return "守序善良";
            case AlignmentType.NeutralGood: return "中立善良";
            case AlignmentType.ChaoticGood: return "混乱善良";
            case AlignmentType.LawfulNeutral: return "守序中立";
            case AlignmentType.TrueNeutral: return "绝对中立";
            case AlignmentType.ChaoticNeutral:return "混乱中立";
            case AlignmentType.LawfulEvil: return "守序邪恶";
            case AlignmentType.NeutralEvil:return "中立邪恶";
            case AlignmentType.ChaoticEvil:return "混乱邪恶";
            default: return "错误阵营";
        }
    }
    public static string GetAlignOccupationName(OccupationType Occ, AlignmentType Ali, int Branch)
    {
        switch (Occ)
        {
            //法师6个
            case OccupationType.Mage:
                switch (Ali)
                {
                    case AlignmentType.LawfulGood: return "观察者";
                    case AlignmentType.NeutralGood: return "奥术法师";
                    case AlignmentType.ChaoticGood: return "探灵者";
                    case AlignmentType.LawfulNeutral: return "守秘人";
                    case AlignmentType.TrueNeutral: return "奥秘学者";
                    case AlignmentType.ChaoticNeutral: return "嗜法者";
                    default: return "不可选择";
                }
            //萨满9个
            case OccupationType.Shaman:
                switch (Ali)
                {
                    case AlignmentType.LawfulGood: return "守灵者";
                    case AlignmentType.NeutralGood: return "元素之子";
                    case AlignmentType.ChaoticGood: return "风暴使者";
                    case AlignmentType.LawfulNeutral: return "大地之怒";
                    case AlignmentType.TrueNeutral: return "灵巫";
                    case AlignmentType.ChaoticNeutral: return "熔岩火巫";
                    case AlignmentType.LawfulEvil: return "混沌御者";
                    case AlignmentType.NeutralEvil: return "瘟疫使者";
                    case AlignmentType.ChaoticEvil: return "复仇之灵";
                    default: return "不可选择";
                }
            //术士6个
            case OccupationType.Warlock:
                switch (Ali)
                {
                    case AlignmentType.LawfulNeutral: return "亡骨法师";
                    case AlignmentType.TrueNeutral: return "荒原学者";
                    case AlignmentType.ChaoticNeutral: return "深渊信徒";
                    case AlignmentType.LawfulEvil: return "腐化学者";
                    case AlignmentType.NeutralEvil: return "虚空领主";
                    case AlignmentType.ChaoticEvil: return "湮灭者";
                    default: return "不可选择";
                }
            //牧师2个
            case OccupationType.Priest:
                switch (Ali)
                {
                    case AlignmentType.LawfulGood: return "红衣主教";
                    case AlignmentType.LawfulNeutral: return "高阶祭司";
                    default: return "不可选择";
                }
            //侍僧3个
            case OccupationType.Acolyte:
                switch (Ali)
                {
                    case AlignmentType.LawfulEvil: return "虔信者";
                    case AlignmentType.NeutralEvil: return "夺灵者";
                    case AlignmentType.ChaoticEvil: return "末日使者";
                    default: return "不可选择";
                }
            //圣骑士2个
            case OccupationType.Paladin:
                switch (Ali)
                {
                    case AlignmentType.LawfulGood:
                        if (Branch == 0)
                            return "审判者";
                        else
                            return "圣武士";
                    default: return "不可选择";
                }
            //黑武士2个
            case OccupationType.Darknight:
                switch (Ali)
                {
                    case AlignmentType.LawfulEvil:
                        if (Branch == 0)
                            return "黑锋骑士";
                        else
                            return "鲜血猎手";
                    default: return "不可选择";
                }
            //战士9个
            case OccupationType.Warrior:
                switch (Ali)
                {
                    case AlignmentType.LawfulGood: return "骑士";
                    case AlignmentType.NeutralGood: return "士兵";
                    case AlignmentType.ChaoticGood: return "部族勇士";
                    case AlignmentType.LawfulNeutral: return "护卫者";
                    case AlignmentType.TrueNeutral: return "佣兵";
                    case AlignmentType.ChaoticNeutral: return "侠客";
                    case AlignmentType.LawfulEvil: return "奴隶贩子";
                    case AlignmentType.NeutralEvil: return "海盗船长";
                    case AlignmentType.ChaoticEvil: return "摧心者";
                    default: return "不可选择";
                }
            //猎人9个
            case OccupationType.Hunter:
                switch (Ali)
                {
                    case AlignmentType.LawfulGood: return "虚空猎手";
                    case AlignmentType.NeutralGood: return "荒野导师";
                    case AlignmentType.ChaoticGood: return "兽王";
                    case AlignmentType.LawfulNeutral: return "御龙者";
                    case AlignmentType.TrueNeutral: return "狩猎大师";
                    case AlignmentType.ChaoticNeutral: return "食腐者";
                    case AlignmentType.LawfulEvil: return "赏金猎人";
                    case AlignmentType.NeutralEvil: return "猎杀者";
                    case AlignmentType.ChaoticEvil: return "唤鸦者";
                    default: return "不可选择";
                }
            //德鲁伊4个
            case OccupationType.Druid:
                switch (Ali)
                {
                    case AlignmentType.LawfulGood: return "自然守卫";
                    case AlignmentType.NeutralGood: return "林木导师";
                    case AlignmentType.LawfulNeutral: return "守护者";
                    case AlignmentType.TrueNeutral: return "流放者";
                    default: return "不可选择";
                }
            //游荡者6个
            case OccupationType.Rogue:
                switch (Ali)
                {
                    case AlignmentType.NeutralGood: return "追踪者";
                    case AlignmentType.ChaoticGood: return "暗影导师";
                    case AlignmentType.TrueNeutral: return "影刃";
                    case AlignmentType.ChaoticNeutral: return "欺诈者";
                    case AlignmentType.NeutralEvil: return "送葬者";
                    case AlignmentType.ChaoticEvil: return "幽魂";
                    default: return "不可选择";
                }
            default: return "未知职业";
        }
    }
}
