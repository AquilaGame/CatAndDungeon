using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadFileBtn : MonoBehaviour
{
    public FileSelectPanel.FileSelectBhv OnClickBtn;
    [SerializeField] Text FileNameTxt = null;
    [SerializeField] Text LastWtTimeTxt = null;
    System.IO.FileInfo info = null;
    public void SetUp(FileSelectPanel.FileSelectBhv bhv, System.IO.FileInfo fileInfo)
    {
        OnClickBtn = bhv;
        FileNameTxt.text = fileInfo.Name;
        LastWtTimeTxt.text = fileInfo.LastWriteTime.ToString();
        info = fileInfo;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        OnClickBtn(info);
    }

}
