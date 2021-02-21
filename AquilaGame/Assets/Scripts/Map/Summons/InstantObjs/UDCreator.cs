using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDCreator : SummonObjectBase
{
    public float AnimTime = 1.5f;
    public float EndTime = 1.5f;
    int state = 0;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        Sound.Play(SummonSound, this);
        Log = "拥抱死亡";
        AnimTime += Time.time;
    }

    private void Update()
    {
        if (state == 0 && Time.time >= AnimTime)
        {
            CharacterScript cs;
            switch ((int)Values[1])
            {
                case 1: cs = OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "尸鬼", 0); break;
                case 2: cs = OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "拾荒者", 0); break;
                case 3: cs = OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "亡骨士兵", 0); break;
                case 4: cs = OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "瘟妖", 0); break;
                default: cs = OnScene.manager.SummonCall(transform.position, transform.rotation, Creator, "尸鬼", 0);break;
            }
            cs.AddBuff("死灵", -1, 1);
            EndTime += Time.time;
            state = 1;
        }
        else if (state == 1 && Time.time >= EndTime)
        {
            Destroy(gameObject);
        }
    }

}
