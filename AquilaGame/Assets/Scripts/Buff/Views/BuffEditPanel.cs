using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuffEditPanel : MonoBehaviour
{
    [SerializeField] Dropdown BuffListDpn = null;
    [SerializeField] InputField TimeFld = null;
    [SerializeField] InputField ValueFld = null;
    [SerializeField] Button ConfirmBtn = null;
    [SerializeField] Button CancelBtn = null;
    [SerializeField] Text PreviewNameStr = null;
    [SerializeField] Text PreviewInfoStr = null;
    [SerializeField] Text PreviewTimeStr = null;
    int nowEditIndex = 0;
    Buff nowEditBuff = null;
    public delegate void EditBuffBhv(int index, string name, int time, float value);
    EditBuffBhv editBhv = null;
    void Start()
    {
        CancelBtn.onClick.AddListener(() => { gameObject.SetActive(false); });
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
        TimeFld.onValueChanged.AddListener((string s)=> { FlushPreview(); });
        ValueFld.onValueChanged.AddListener((string s) => { FlushPreview(); });
        BuffListDpn.onValueChanged.AddListener((int i) => { FlushPreview(); });
        BuffListDpn.AddOptions(OnGame.buffLib.names);
    }

    //用于新增的
    public void SetUp(EditBuffBhv bhv)
    {
        nowEditBuff = null;
        editBhv = bhv;
        FlushPreview();
    }
    //用于编辑的
    public void SetUp(int index, Buff buff, EditBuffBhv bhv)
    {
        nowEditBuff = null;
        BuffListDpn.AddOptions(OnGame.buffLib.names);
        nowEditIndex = index;
        BuffListDpn.value = buff.index;
        TimeFld.text = buff.continueTime.ToString();
        ValueFld.text = buff.value.ToString();
        editBhv = bhv;
        FlushPreview();
    }
    void OnClickConfirm()
    {
        if (nowEditBuff != null)
        {
            editBhv?.Invoke(nowEditIndex, nowEditBuff.Name, nowEditBuff.continueTime, nowEditBuff.value);
            gameObject.SetActive(false);
        }
    }
    public void FlushPreview()
    {
        try
        {
            string name = OnGame.buffLib.names[BuffListDpn.value];
            int time = System.Convert.ToInt32(TimeFld.text);
            float value = System.Convert.ToSingle(ValueFld.text);
            nowEditBuff = (Buff)System.Activator.CreateInstance(OnGame.buffLib.buffs[name], null, time, value);
            PreviewNameStr.text = name;
            PreviewTimeStr.text = time <= 0 ? "无限制" : time.ToString() + "回合";
            PreviewInfoStr.text = nowEditBuff.Info();
        }
        catch (System.Exception e)
        {
            PreviewNameStr.text = "以下为出错信息";
            PreviewTimeStr.text = "遇到错误";
            PreviewInfoStr.text = e.ToString();
            nowEditBuff = null;
        }
    }
}
