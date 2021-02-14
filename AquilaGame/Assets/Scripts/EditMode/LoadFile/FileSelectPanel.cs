using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FileSelectPanel : MonoBehaviour
{
    public delegate void FileSelectBhv (System.IO.FileInfo fileInfo);
    public float BtnLocDelta = 55;
    FileSelectBhv ClickFileBtnBhv;
    [SerializeField] Button ExitBtn = null;
    [SerializeField] Text ExtNameTxt = null;
    [SerializeField] RectTransform Content = null;
    [SerializeField] GameObject FileSelectBtnObj;
    List<GameObject> FileSelectBtnList = new List<GameObject>();
    void Start()
    {
        ExitBtn.onClick.AddListener(OnClickExitBtn);
    }
    void OnClickExitBtn()
    {
        gameObject.SetActive(false);
    }
    public void SetUp(FileSelectBhv OnClickFileButton, System.IO.FileInfo[] fileInfos, string extName)
    {
        foreach (var go in FileSelectBtnList)
            Destroy(go);
        FileSelectBtnList.Clear();
        ClickFileBtnBhv = OnClickFileButton;
        ExtNameTxt.text = extName;
        Content.sizeDelta = new Vector2(0.0f, BtnLocDelta * fileInfos.Length);
        for (int i = 0; i < fileInfos.Length; i++)
        {
            GameObject go = Instantiate(FileSelectBtnObj, Content);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -BtnLocDelta * i);
            go.GetComponent<LoadFileBtn>().SetUp(ClickFileBtnBhv, fileInfos[i]);
        }
    }

}
