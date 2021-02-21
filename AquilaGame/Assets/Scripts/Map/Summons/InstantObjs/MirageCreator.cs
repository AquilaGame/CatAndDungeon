using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirageCreator : SummonObjectBase
{
    public float AnimTime = 2;
    public float EndTime = 2;
    int state = 0;
    // Start is called before the first frame update
    override public void Start()
    {
        if (isSkipSummon) return;
        Sound.Play(SummonSound, this);
        Log = "凝聚智慧";
        AnimTime += Time.time;
    }

    private void Update()
    {
        if (state == 0 && Time.time >= AnimTime)
        {
            CharacterScript cs = Instantiate(Creator.gameObject, transform.position, transform.rotation).GetComponent<CharacterScript>();
            Occupation occupation = new Occupation(Creator.data.OccupationData.type, Creator.data.OccupationData.branch);
            AttributeData attribute = new AttributeData(new Attribute6(Creator.data.BaseData.baseAttribute), occupation);
            attribute.MaxHP = Creator.data.BaseData.MaxHP;
            attribute.HP = (int)(Creator.data.BaseData.HP * Values[1] / 100);
            attribute.MP = 0;
            attribute.level = Creator.data.BaseData.level;
            Character newc = new Character(Creator.data.Name + "(镜像)", Creator.data.Log, Creator.data.Nickname, "镜像", occupation,
                                           Creator.data.birth, attribute, Creator.data.alignmentType, new MagicBook(), new ItemList());
            newc.characterType = Creator.data.characterType;
            cs.SetUp(newc);
            Creator.AddBuff("潜行", (int)Values[0], 1);
            EndTime += Time.time;
            state = 1;
        }
        else if (state == 1 && Time.time >= EndTime)
        {
            Destroy(gameObject);
        }
    }

}
