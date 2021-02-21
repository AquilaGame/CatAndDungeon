using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MatSelectPanel : MonoBehaviour
{
    public Scrollbar sbar;
    public List<MatSelectBtn> Btns = new List<MatSelectBtn>();
    public Slider[] HSV_slider = new Slider[3];
    public Button ConfirmBtn = null;
    public Renderer PreviewRender = null;
    public MapObjSetPanel MapObjSetPnl;
    int nowMatNum = 0;
    Material nowMat = null;
    Color nowColor = Color.white;
    int nowPage = 0;
    float ScrollSpeed = 1.0f;
    List<int> nowAllowed;
    private void Start()
    {
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
        sbar.onValueChanged.AddListener(OnDragBar);
        foreach (var v in HSV_slider)
        {
            v.onValueChanged.AddListener(OnChangeColor);
        }
    }
    public void SetUp(List<int> allowed)
    {
        nowPage = 0;
        nowAllowed = allowed;
        int page = allowed.Count / 9 + 1;
        sbar.numberOfSteps = (page > 5) ? page - 4 : 1;
        ScrollSpeed = 1.0f / sbar.numberOfSteps;
        for (int i = 0; i < Btns.Count; i++)
        {
            Btns[i].SetBehaviour((int index) => { }, (int index) => { }, OnClickSelectBtn);
        }
        sbar.value = 0;
        nowMatNum = MapObjSetPnl.nowMatNum[MapObjSetPnl.nowEditMat];
        nowMat = MapObjSetPnl.nowMats[MapObjSetPnl.nowEditMat];
        nowColor = MapObjSetPnl.nowColors[MapObjSetPnl.nowEditMat];
        PreviewRender.material = nowMat;
        PreviewRender.material.color = nowColor;
        Color.RGBToHSV(nowColor,out float h,out float s, out float v);
        HSV_slider[0].value = h;
        HSV_slider[1].value = s;
        HSV_slider[2].value = v;
        SelectFlushDis(0);
    }

    void SelectFlushDis(int page)//有机会改一下，这个效率很低
    {
        int j = 0;
        for (int i = page * 9; i < nowAllowed.Count && j < 45; i++, j++)
        {
            Btns[j].SetBtn(nowAllowed[i]);
        }
        for (; j < 45; j++)
        {
            Btns[j].SetDisable();
        }
    }
    void OnDragBar(float val)
    {
        int page = (int)System.Math.Floor(sbar.value * sbar.numberOfSteps);
        if (page == sbar.numberOfSteps)
            page--;
        if (page != nowPage)
        {
            SelectFlushDis(page);
            nowPage = page;
        }
    }
    void OnChangeColor(float i)
    {
        nowColor = Color.HSVToRGB(HSV_slider[0].value, HSV_slider[1].value, HSV_slider[2].value);
        PreviewRender.material.color = nowColor;
        //nowMat.color = color;
    }
    void OnClickSelectBtn(int index)
    {
        nowMatNum = nowAllowed[index + nowPage * 9];
        nowMat = MapSet.mapObjLib.materials[nowMatNum];
        nowColor = nowMat.color;
        Color.RGBToHSV(nowColor, out float h, out float s, out float v);
        HSV_slider[0].value = h;
        HSV_slider[1].value = s;
        HSV_slider[2].value = v;
        PreviewRender.material = nowMat;
    }
    void OnClickConfirm()
    {
        MapObjSetPnl.nowMatNum[MapObjSetPnl.nowEditMat] = nowMatNum;
        MapObjSetPnl.nowMats[MapObjSetPnl.nowEditMat] = nowMat;
        MapObjSetPnl.nowColors[MapObjSetPnl.nowEditMat] = nowColor;
        MapObjSetPnl.FlushMatButton(MapObjSetPnl.nowEditMat);
        gameObject.SetActive(false);
    }
}
