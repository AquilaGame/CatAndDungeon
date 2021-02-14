using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjSelectBtn : SelectBtnBase
{
    public Transform Seat;
    GameObject go;
    public override void SetBtn(object type)
    {
        if (go)
            Destroy(go);
        go = Instantiate(MapSet.mapObjLib.Icons[(int)(MapObjType)type], Seat);
        isEnable = true;
    }
    public override void SetDisable()
    {
        if (go)
            Destroy(go);
        isEnable = false;
    }
}
