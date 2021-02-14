using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ReportWindow : MonoBehaviour
{
    public int Size = 30;
    [SerializeField] Text Logtext = null;
    [SerializeField] RectTransform TextTrans = null;
    [SerializeField] RectTransform ContentTrans = null;
    [SerializeField] Button ToSmallBtn = null;
    [SerializeField] Button ClearBtn = null;
    [SerializeField] Toggle isAutoFlushTog = null;
    [SerializeField] Scrollbar scrollbar = null;
    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    Queue<int> LogQueue = new Queue<int>();
    private void Awake()
    {
        OnScene.reporter = this;
        OnGame.Log("报告窗口已建立连接");
    }
    public void Show(string str)
    {
        if (LogQueue.Count > Size)
        {
            sb.Remove(0, LogQueue.Dequeue());
        }
        sb.Append(str);
        sb.Append('\n');
        LogQueue.Enqueue(str.Length + 1);
        Logtext.text = sb.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(TextTrans);
        ContentTrans.sizeDelta = new Vector2(0, TextTrans.rect.height);
        if (isAutoFlushTog.isOn)
            scrollbar.value = 0;
    }

    public void Clear()
    {
        sb.Clear();
        Logtext.text = sb.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(TextTrans);
        ContentTrans.sizeDelta = new Vector2(0, TextTrans.rect.height);
        if (isAutoFlushTog.isOn)
            scrollbar.value = 1;
    }

    private void Start()
    {
        ClearBtn.onClick.AddListener(Clear);
        ToSmallBtn.onClick.AddListener(() => {
            OnScene.Report("此功能暂未开放");
        });
    }
}
