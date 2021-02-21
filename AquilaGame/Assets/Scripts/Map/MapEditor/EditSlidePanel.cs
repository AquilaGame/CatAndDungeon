using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EditSlidePanel : MonoBehaviour
{
    public Button TileEditBtn;
    public Button ObjEditBtn;
    public Button SaveBtn;
    public Button LoadBtn;
    public TypeSelectPnl typeSelectPnl;
    public BluePrint_Panel bluePrintPnl;
    public GameObject fileSlctPnl;
    public GameObject tileSetPnl;
    public GameObject objSetPnl;
    public InputField FileNameTxt;
    public Button ClearMapButton;
    private void Start()
    {
        TileEditBtn.onClick.AddListener(OnClickTileEditBtn);
        ObjEditBtn.onClick.AddListener(OnClickObjEditBtn);
        SaveBtn.onClick.AddListener(OnClickSaveBtn);
        LoadBtn.onClick.AddListener(OnClickLoadBtn);
        ClearMapButton.onClick.AddListener(OnClickClearMapBtn);
        FileNameTxt.text = AutoMapName(1);
    }

    string AutoMapName(int i)
    {
        string s = "新建地图" + i;
        var files = new List<System.IO.FileInfo> (new System.IO.DirectoryInfo(MapSet.MapSavePath).GetFiles('*' + MapSet.MapSaveFileExtName));
        while (files.Find((info) => info.Name.Split('.')[0] == s) != null)
        {
            i++;
            s = "新建地图" + i;
        }
        return s;
    }

    void OnClickClearMapBtn()
    {
        MapSet.nowCreator.LoadMap(OnGame.defaultPath + OnGame.defaultMapName);
        FileNameTxt.text = AutoMapName(1);
    }

    void OnClickTileEditBtn()
    {
        typeSelectPnl.FlushSelectType(typeSelectPnl.Type_TileType);
        bluePrintPnl.isMatObjEditMode = false;
        objSetPnl.SetActive(false);
        tileSetPnl.SetActive(true);
    }
    void OnClickObjEditBtn()
    {
        typeSelectPnl.FlushSelectType(typeSelectPnl.Type_MapObjType);
        bluePrintPnl.isMatObjEditMode = true;
        tileSetPnl.SetActive(false);
        objSetPnl.SetActive(true);
    }
    void OnClickSaveBtn()
    {
        try
        {
            MapSet.nowCreator.SaveMap(FileNameTxt.text);
        }
        catch (System.Exception e)
        {
            OnGame.Log("存储地图时出现错误" + e.ToString());
        }
        
    }

    void OnClickLoadBtn()
    {
        System.IO.DirectoryInfo savefolder = new System.IO.DirectoryInfo(MapSet.MapSavePath);
        fileSlctPnl.SetActive(true);
        fileSlctPnl.GetComponent<FileSelectPanel>().SetUp(
            (System.IO.FileInfo info) => 
            {
                FileNameTxt.text = info.Name.Split('.')[0];
                MapSet.nowCreator.LoadMap(info.FullName);
            },
            savefolder.GetFiles('*' + MapSet.MapSaveFileExtName),
            MapSet.MapSaveFileExtName);
    }
}

    