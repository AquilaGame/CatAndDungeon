using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BuffPanel : MonoBehaviour
{
    [SerializeField] CharacterPanel CharacterPnl;
    [SerializeField] GameObject InfoPnl;
    [SerializeField] Transform PnlContentTrans;
    [SerializeField] Text NameStr;
    [SerializeField] Text InfoStr;
    [SerializeField] Text TimeStr;
    [SerializeField] GameObject IconBtnObj;
    [SerializeField] Button AddBuffBtn;
    [SerializeField] BuffEditPanel editPnl;
    List<Buff> bufflist = new List<Buff>();
    List<BuffIconBtn> iconlist = new List<BuffIconBtn>();
    private void Start()
    {
        AddBuffBtn.onClick.AddListener(OnClickAddBtn);
    }
    void OnClickAddBtn()
    {
        editPnl.gameObject.SetActive(true);
        editPnl.SetUp(OnScene.onSelect.character.AddBuffCallback);
    }

    public void Flush(List<Buff> buffs)
    {
        InfoPnl.SetActive(false);
        foreach (var v in iconlist)
        {
            Destroy(v.gameObject);
        }
        iconlist.Clear();
        bufflist.Clear();
        foreach (Buff buff in buffs)
        {
            ListAddBuffBtn(buff);
        }
    }

    void ListAddBuffBtn(Buff buff)
    {
        bufflist.Add(buff);
        iconlist.Add(AddBtn(buff));
    }

    BuffIconBtn AddBtn(Buff buff)
    {
        BuffIconBtn btn = Instantiate(IconBtnObj, PnlContentTrans).GetComponent<BuffIconBtn>();
        btn.SetUp(this,buff);
        return btn;
    }
    //左键是改
    public void OnPointerLeftClick(BuffIconBtn btn)
    {
        int index = iconlist.IndexOf(btn);
        editPnl.gameObject.SetActive(true);
        editPnl.SetUp(index, bufflist[index],OnScene.onSelect.character.ChangeBuffCallback);
    }
    //右键是删
    public void OnPointerRightClick(BuffIconBtn btn)
    {
        int index = iconlist.IndexOf(btn);
        OnScene.onSelect.character.RemoveBuff(index);
    }

    public void OnPointerEnter(BuffIconBtn btn)
    {
        InfoPnl.SetActive(true);
        Buff buff = bufflist[iconlist.IndexOf(btn)];
        NameStr.text = buff.Name;
        TimeStr.text = buff.continueTime <= 0 ? "无限制" : buff.continueTime.ToString() + "回合";
        InfoStr.text = buff.Info();
    }
    public void OnPointerExit()
    {
        InfoPnl.SetActive(false);
    }
}
