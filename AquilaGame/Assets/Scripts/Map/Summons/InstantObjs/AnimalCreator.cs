using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalCreator : SummonObjectBase
{
    public float AnimTime = 2.0f;
    public float EndTime = 1.0f;
    int state = 0;
    // Start is called before the first frame update
    override public void Start()
    {
        Sound.Play(SummonSound, this);
        if (isSkipSummon) return;
        Log = "猛兽动员令！";
        AnimTime += Time.time;
    }

    private void Update()
    {
        if (state == 0 && Time.time >= AnimTime)
        {
            switch ((int)Values[1])
            {
                case 1: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "草原狼", Values[0]); break;
                case 2: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "森林熊", Values[0]); break;
                case 3: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "棘刺野猪", Values[0]); break;
                case 4: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "猎豹", Values[0]); break;
                default: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "草原狼", Values[0]);break;
            }
            EndTime += Time.time;
            state = 1;
        }
        else if (state == 1 && Time.time >= EndTime)
        {
            Destroy(gameObject);
        }
    }

}
