using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilCreator : SummonObjectBase
{
    public float AnimTime = 2.5f;
    public float EndTime = 2.5f;
    int state = 0;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        Sound.Play(SummonSound, this);
        Log = "拥抱虚空";
        AnimTime += Time.time;
    }

    private void Update()
    {
        if (state == 0 && Time.time >= AnimTime)
        {
            switch ((int)Values[1])
            {
                case 1: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "邪魔侍从", Values[0]); break;
                case 2: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "暗夜舞者", Values[0]); break;
                case 3: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "恐惧骑士", Values[0]); break;
                case 4: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "暗影", Values[0]); break;
                default: OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "暗影", Values[0]);break;
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
