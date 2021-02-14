using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatSelectBtn : SelectBtnBase
{
    public Renderer MatRender;
    public override void SetBtn(object index)
    {
        isEnable = true;
        MatRender.gameObject.SetActive(true);
        MatRender.material = MapSet.mapObjLib.materials[(int)index];
    }
    public override void SetDisable()
    {
        isEnable = false;
        MatRender.gameObject.SetActive(false);
    }
}
