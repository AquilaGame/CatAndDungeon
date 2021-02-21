using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFloor : SummonObjectBase
{
    [SerializeField] Transform ObjTrans = null;
    public int LifeTime = 3;
    public float AnimStepSecond = 0.05f;
    float nextFlushTime = 0;
    public Sprite[] Sprites = new Sprite[3];
    int nowSprite = 0;
    public SpriteRenderer Img;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        base.Start();
        Log = "地板上充满了岩浆，对接触到的目标造成" + Values[1].ToString() + "回合的燃烧效果";
        ObjTrans.localScale = new Vector3(Values[0] * OnGame.WorldScale, 1, Values[0] * OnGame.WorldScale);
        nextFlushTime = Time.time;
        OnRoundStart();
    }

    void GiveBuff(string name, int time, float val)
    {
        float distance = OnGame.WorldScale * Values[0]/2.0f;

        foreach (var v in MapSet.nowCreator.characters)
        {
            if (RangeCharacterSelector.CheckDistance(transform.position, v.transform.position, distance))
                v.AddBuff(name, time, val);
        }
    }

    override public void OnRoundStart()
    {
        GiveBuff("燃烧", (int)Values[1], 5);
        LifeTime--;
        Sound.Play(OnRoundStartSound, this);
        if (LifeTime <= 0)
        {
            OnScene.Report("<color=#55FFAA>[法术消散]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤的<color=#FFE0CC>[" + Name + "]</color>消散了。");
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
    }
}
