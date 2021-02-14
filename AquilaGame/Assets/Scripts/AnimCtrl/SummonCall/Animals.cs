using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animals : SummonScriptBase
{
    public override void Start()
    {
        if (isSkipSummon)
        {
            return;
        }
        base.Start();
        AttributeData attribute = Self.data.BaseData;
        attribute.STR += (int)Value;
        attribute.DEX += (int)Value;
        attribute.CON += (int)Value;
        attribute.INT += (int)Value;
        attribute.WIS += (int)Value;
        attribute.CHA += (int)Value;
        attribute.MaxHP = attribute.GetMaxHP();
        attribute.MaxMP = attribute.GetMaxMP(Self.data.OccupationData);
        attribute.HP = attribute.MaxHP;
        attribute.MP = attribute.MaxMP;
        Sound.Play(SummonSound, Self);
    }

}
