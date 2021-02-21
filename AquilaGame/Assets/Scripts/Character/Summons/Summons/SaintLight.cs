using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaintLight : SummonScriptBase
{
    public float AnimStepSecond = 0.05f;
    float nextFlushTime = 0;
    public Sprite[] Sprites = new Sprite[30];
    int nowSprite = 0;
    Quaternion defaultRot = Quaternion.Euler(new Vector3(45, 0, 0));
    public SpriteRenderer Img;
    // Start is called before the first frame update
    public override void Start()
    {
        if (isSkipSummon)
        {
            return;
        }
        if (Value > 6)
        {
            base.Start();
            nextFlushTime = Time.time;
            Sound.Play(SummonSound, Self);
        }
        else
        {
            OnScene.Report("<color=#55FFAA>[召唤法术]: 由于神力不足，</color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的灵光消散了。");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextFlushTime)
        {
            nextFlushTime += AnimStepSecond;
            nowSprite++;
            if (nowSprite >= Sprites.Length)
            {
                nowSprite = 0;
            }
            Img.sprite = Sprites[nowSprite];
        }
        Img.transform.rotation = defaultRot;
    }
}
