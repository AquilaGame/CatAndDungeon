using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//在{3}米内任意位置召唤一只地狱火，并给予其基础属性[3]点加值，同时对召唤位置周围直径[2]米周围区域造成[1]d4伤害。
public class HellFire : SummonScriptBase
{
    public float FallSpeed = 10.0f;
    Vector3 targetPos;
    bool isBirth = false;
    bool isDeployed = false;
    Stack<CharacterScript> targets = new Stack<CharacterScript>();
    Queue<int> damages = new Queue<int>();
    [SerializeField] GameObject BOOOOOM = null;
    // Start is called before the first frame update
    public override void Start()
    {
        if (isSkipSummon)
        {
            isDeployed = true;
            return;
        }
        base.Start();
        AttributeData attribute = Self.data.BaseData;
        attribute.STR += 3 * (int)Value;
        attribute.DEX += 3 * (int)Value;
        attribute.CON += 3 * (int)Value;
        attribute.INT += 3 * (int)Value;
        attribute.WIS += 3 * (int)Value;
        attribute.CHA += 3 * (int)Value;
        attribute.MaxHP = attribute.GetMaxHP();
        attribute.MaxMP = attribute.GetMaxMP(Self.data.OccupationData);
        attribute.HP = attribute.MaxHP;
        attribute.MP = attribute.MaxMP;
        targetPos = transform.position;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = false;
        transform.position = targetPos + new Vector3(0, 12.0f, 0);
        HellFireAttack();
    }

    void HellFireAttack()
    {
        float distance = OnGame.WorldScale * Value/*乘2再除2*/;
        foreach (var v in MapSet.nowCreator.characters)
        {
            if (RangeCharacterSelector.CheckDistance(transform.position, v.transform.position, distance))
            {
                if (v == GetComponent<CharacterScript>())
                    continue;
                targets.Push(v);
                RollPanel roll = OnScene.manager.OpenRollPanel();
                roll.SetUp("地狱火:冲击伤害(目标:" + v.data.Name + ")", AttackCallback, 4, (int)Value);
            }
        }
        if (targets.Count == 0)
        {
            isBirth = true;
            Sound.Play(SummonSound, Self);
        }
    }

    void AttackCallback(int val)
    {
        damages.Enqueue(val > 0 ? val : 1);
        if (damages.Count == targets.Count)
        {
            isBirth = true;
            Sound.Play(SummonSound, Self);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDeployed)
        {
            this.enabled = false;
            return;
        }
        if (isBirth)
        {
            float delta = FallSpeed * Time.deltaTime;
            if (transform.position.y - delta <= targetPos.y)
            {
                transform.position = targetPos;
                isDeployed = true;
                GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = true;
                while (targets.Count != 0)
                {
                    CharacterScript c = targets.Pop();
                    int d = damages.Dequeue();
                    OnScene.Report("<color=#CC44EE>[召唤法术]: </color><color=#EE77CC>[" + c.data.Name + "]</color>受到了来自<color=Orange>[地狱火]</color>的<color=#FF1111><b>"
                                + d.ToString() + "</b></color>点冲击伤害。");
                    c.HPGetDamage(d);
                }
                BOOOOOM.SetActive(true);
                this.enabled = false;
            }
            else
            {
                transform.Translate(new Vector3(0, -delta, 0));
            }
        }
    }
}
