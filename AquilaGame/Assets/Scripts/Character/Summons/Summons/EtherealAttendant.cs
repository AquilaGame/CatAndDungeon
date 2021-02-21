using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherealAttendant : SummonScriptBase
{
    public float UpSpeed = 50.0f;
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
        targetPos = transform.position;
        GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = false;
        transform.position = targetPos + new Vector3(0, -2.0f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDeployed)
        {
            return;
        }
        float delta = UpSpeed* Time.deltaTime;
        if (transform.position.y + delta >= targetPos.y)
        {
            transform.position = targetPos;
            isDeployed = true;
            GetComponent<UnityEngine.AI.NavMeshAgent>().updatePosition = true;
            Sound.Play(SummonSound, Self);
            this.enabled = false;
        }
        else
        {
            transform.Translate(new Vector3(0, delta, 0));
        }
    }
}
