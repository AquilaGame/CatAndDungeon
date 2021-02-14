using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObjSetPanel : MonoBehaviour
{
    [SerializeField] Text nowSelectNameTxt = null;
    [SerializeField] Toggle[] DirTog = new Toggle[4];
    [SerializeField] Button[] Mats = new Button[4];
    [SerializeField] Renderer[] MatRenders = new Renderer[4];
    [SerializeField] Button SetVisBtn = null;
    [SerializeField] Transform PreViewSeat = null;
    [SerializeField] GameObject PreViewGo = null;
    [SerializeField] GameObject MatSelectPnlObj = null;
    [SerializeField] MatSelectPanel MatSelectPnl = null;
    public MapObjType nowType = MapObjType.Stairs;
    public int[] nowMatNum = new int[4];
    public Material[] nowMats = new Material[4];
    public Color[] nowColors = new Color[4];
    public int nowEditMat = 0;
    public int nowDir = 0;
    public List<List<int>> nowMatList = null;
    public GameObject nowObj = null;
    public GameObject nowObj2D = null;
    GameObject nowIcon = null;
    private void Start()
    {
        Mats[0].onClick.AddListener(OnClickMatSelectBtn0);
        Mats[1].onClick.AddListener(OnClickMatSelectBtn1);
        Mats[2].onClick.AddListener(OnClickMatSelectBtn2);
        Mats[3].onClick.AddListener(OnClickMatSelectBtn3);
        DirTog[0].onValueChanged.AddListener(OnClickDirTog0);
        DirTog[1].onValueChanged.AddListener(OnClickDirTog1);
        DirTog[2].onValueChanged.AddListener(OnClickDirTog2);
        DirTog[3].onValueChanged.AddListener(OnClickDirTog3);
        SetUp(MapObjType.Slope,0);
    }

    public void FlushPreView(bool needFlushObj)
    {
        if (needFlushObj)
        {
            Destroy(PreViewGo);
            PreViewGo = Instantiate(nowIcon, PreViewSeat);
        }
        SetMat(PreViewGo.GetComponent<Renderer>());
    }

    public void SetMat(Renderer render)
    {
        Material[] materials = new Material[nowMatList.Count];
        System.Array.Copy(nowMats, materials, materials.Length);
        render.GetComponent<Renderer>().materials = materials;
        for (int i = 0; i < materials.Length; i++)
           render.GetComponent<Renderer>().materials[i].color = nowColors[i];
    }

    public void SetUp(MapObjType _type, int index)
    {
        if (_type == nowType)
            return;
        nowType = _type;
        nowMatList = MapSet.mapObjLib.GetMatList(nowType);
        nowObj = MapSet.mapObjLib.Objs[index];
        nowObj2D = MapSet.mapObjLib.Objs2D[index];
        nowIcon = MapSet.mapObjLib.Icons[index];
        nowSelectNameTxt.text = MapSet.mapObjLib.Name[index] + nowMatList.Count;
        int i = 0;
        for (; i < nowMatList.Count; i++)
        {
            nowMatNum[i] = nowMatList[i][0];
            nowMats[i] = MapSet.mapObjLib.materials[nowMatNum[i]];
            nowColors[i] = nowMats[i].color;
            MatRenders[i].material = nowMats[i];
            Mats[i].interactable = true;
        }
        for (; i < Mats.Length; i++)
        {
            Mats[i].interactable = false;
        }
        FlushPreView(true);
    }
    public void FlushMatButton(int i)
    {
        MatRenders[i].material = nowMats[i];
        MatRenders[i].material.color = nowColors[i];
        FlushPreView(false);
    }
    void OnClickMatSelectBtn0() { nowEditMat = 0; MatSelectPnlObj.SetActive(true); MatSelectPnl.SetUp(nowMatList[0]); }
    void OnClickMatSelectBtn1() { nowEditMat = 1; MatSelectPnlObj.SetActive(true); MatSelectPnl.SetUp(nowMatList[1]); }
    void OnClickMatSelectBtn2() { nowEditMat = 2; MatSelectPnlObj.SetActive(true); MatSelectPnl.SetUp(nowMatList[2]); }
    void OnClickMatSelectBtn3() { nowEditMat = 3; MatSelectPnlObj.SetActive(true); MatSelectPnl.SetUp(nowMatList[3]); }
    void OnClickDirTog0(bool val) { if (val) nowDir = 0; }
    void OnClickDirTog1(bool val) { if (val) nowDir = 1; }
    void OnClickDirTog2(bool val) { if (val) nowDir = 2; }
    void OnClickDirTog3(bool val) { if (val) nowDir = 3; }
}
