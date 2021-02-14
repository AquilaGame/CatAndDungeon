using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AttackOrderPanel : MonoBehaviour
{
    class RollRes
    {
        public CharacterScript characterScript;
        public int res;
        public RollRes(CharacterScript cs)
        {
            characterScript = cs;
            res = 0;
        }
    }
    [SerializeField] Button FlushBtn= null;
    [SerializeField] RangeCharacterSelector RangeSelector = null;
    [SerializeField] GameObject TxtPrefab = null;
    [SerializeField] Transform ContentTrans = null;
    List<GameObject> Txts = new List<GameObject>();
    List<RollRes> rolls = new List<RollRes>();
    int rollcount = 0;
    // Start is called before the first frame update
    void Start()
    {
        FlushBtn.onClick.AddListener(() => { RangeSelector.StartRectCatch(RangeCharacterSelector.SelectorType.CanvasRect,AfterSelect);gameObject.SetActive(false); });
    }

    void AfterSelect(List<CharacterScript> characters,Vector3 noUse)
    {
        gameObject.SetActive(true);
        rolls.Clear();
        rollcount = 0;
        foreach (var v in characters)
        {
            rolls.Add(new RollRes(v));
        }
        if (rolls.Count > 0)
        {
            RollPanel rollPanel = OnScene.manager.OpenRollPanel();
            rollPanel.SetUp("先攻检定:" + rolls[rollcount].characterScript.data.Name, ProcessRoll, rolls[rollcount].characterScript.data.BaseData.DEX);
        }
        else
        {
            FlushList();
        }
    }

    void ProcessRoll(int value)
    {
        rolls[rollcount].res = value;
        OnScene.Report("<color=#CC44EE>[先攻检定]: </color><color=Yellow>[" + rolls[rollcount].characterScript.data.Name + "]</color>的先攻检定结果为:<color=#FF88FF><b>" 
            + value.ToString() + "</b></color>点。");
        rollcount++;
        if (rollcount >= rolls.Count)
        {
            FlushList();
        }
        else
        {
            RollPanel rollPanel = OnScene.manager.OpenRollPanel();
            rollPanel.SetUp("先攻检定:" + rolls[rollcount].characterScript.data.Name, ProcessRoll, rolls[rollcount].characterScript.data.BaseData.DEX);
        }
    }

    void FlushList()
    {
        foreach (var v in Txts)
        {
            Destroy(v);
        }
        Txts.Clear();
        if (rolls.Count == 0)
            return;
        //a大于b返回1，等于时返回0，小于时返回-1
        rolls.Sort((RollRes a, RollRes b) => { return -a.res.CompareTo(b.res); });
        foreach (var v in rolls)
        {
            GameObject txt = Instantiate(TxtPrefab, ContentTrans);
            txt.GetComponent<Text>().color = Alignment.ColorList[(int)v.characterScript.data.characterType];
            txt.GetComponent<Text>().text = v.characterScript.data.Name + '(' + v.res.ToString() + ')';
            Txts.Add(txt);
        }
    }

}
