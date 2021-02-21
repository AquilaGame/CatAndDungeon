using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderElement : SummonScriptBase
{
    public float FallSpeed = 10.0f;
    Vector3 targetPos;
    bool isDeployed = false;
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
        targetPos = transform.position;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = false;
        transform.position = targetPos + new Vector3(0, 10.0f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDeployed)
        {
            return;
        }
        float delta = FallSpeed * Time.deltaTime;
        if (transform.position.y - delta <= targetPos.y)
        {
            transform.position = targetPos;
            isDeployed = true;
            GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = true;
            Sound.Play(SummonSound, Self);
            this.enabled = false;
        }
        else
        {
            transform.Translate(new Vector3(0, -delta, 0));
        }
    }
}
