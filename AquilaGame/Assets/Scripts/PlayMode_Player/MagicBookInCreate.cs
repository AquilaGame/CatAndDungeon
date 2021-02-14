using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBookInCreate : MonoBehaviour
{
    [SerializeField]RectTransform contentRect = null;
    List<MagicBookContentInCreate> contentList = new List<MagicBookContentInCreate>();
    [SerializeField] GameObject ContentObj = null;
    [SerializeField] RectTransform BaseContent = null;
    [SerializeField] UnityEngine.UI.Text BaseText = null;
    [SerializeField] CreateCharecterPanel Pnl;
    [SerializeField] GameObject infoWindow = null;
    int maxSkillCount = 0;
    int skillCount = 0;
    bool isSet = false;
    public void Add(Skill sk)
    {
        GameObject go = Instantiate(ContentObj, contentRect);
        go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40 * contentList.Count);
        MagicBookContentInCreate temp = go.GetComponent<MagicBookContentInCreate>();
        temp.SetUp(sk,infoWindow,this);
        contentList.Add(temp);
        skillCount++;
        FlushBase();
    }
    public void Clear()
    {
        if (!isSet) return;
        isSet = false;
        foreach (var v in contentList)
            Destroy(v.gameObject);
        contentList.Clear();
        maxSkillCount = 0;
        skillCount = 0;
        FlushBase();
    }
    public void SetUp(Occupation occupation,Attribute6 attribute,int exCount)
    {
        if (isSet)
            return;
        maxSkillCount = exCount > 0 ? exCount : Occupation.GetOcccupationMaxSkillCount(occupation.type, attribute);
        skillCount = 0;
        FlushBase();
        isSet = true;
    }
    public void Delete(MagicBookContentInCreate it)
    {
        Destroy(it.gameObject);
        contentList.Remove(it);
        for (int i = 0; i < contentList.Count; i++)
        {
            contentList[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40 * i);
        }
        skillCount--;
        FlushBase();
    }
    public bool Contains(Skill skill)
    {
        foreach (var v in contentList)
        {
            if (v.skill == skill)
                return true;
        }
        return false;
    }
    public int Count
    {
        get { return skillCount; }
    }
    public bool isFilled
    {
        get{
            return skillCount >= maxSkillCount;
        }
    }

    public List<MagicBookContentInCreate> List
    {
        get { return contentList; }
    }

    void FlushBase()
    {
        BaseContent.anchoredPosition = new Vector2(0, -40 * contentList.Count);
        contentRect.sizeDelta = new Vector2(0, 40 * contentList.Count + 1);
        BaseText.text = "你还能记录 <b>" + (maxSkillCount - skillCount).ToString() + "</b> 个法术";
    }
}
