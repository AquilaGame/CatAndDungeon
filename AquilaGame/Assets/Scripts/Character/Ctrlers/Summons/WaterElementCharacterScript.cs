using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterElementCharacterScript : CharacterScript
{
    public override void AfterCauseDamage(CharacterScript enemy, int damage)
    {
        base.AfterCauseDamage(enemy, damage);
        enemy.AddBuff("冰冻", 1, 1);
    }
}
