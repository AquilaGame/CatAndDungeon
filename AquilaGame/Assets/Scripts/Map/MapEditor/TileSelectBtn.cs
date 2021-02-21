using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelectBtn : SelectBtnBase
{
    public UnityEngine.UI.RawImage imgTop;
    public UnityEngine.UI.RawImage imgSideLeft;
    public UnityEngine.UI.RawImage imgSideRight;

    public override void SetBtn(object type)
    {
        isEnable = true;
        imgTop.gameObject.SetActive(true);
        imgSideLeft.gameObject.SetActive(true);
        imgSideRight.gameObject.SetActive(true);
        imgTop.texture = MapSet.GetTileInfo((TileType)type).topMat.mainTexture;
        imgSideLeft.texture = MapSet.GetTileInfo((TileType)type).sideMat.mainTexture;
        imgSideRight.texture = imgSideLeft.texture;
    }
    public override void SetDisable()
    {
        isEnable = false;
        imgTop.gameObject.SetActive(false);
        imgSideLeft.gameObject.SetActive(false);
        imgSideRight.gameObject.SetActive(false);
    }

}
