using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TypeSelectToast : MonoBehaviour
{
    public Text NameTxt;
    public Text LogText;
    System.Type Type_TileType = TileType.Grass.GetType();
    System.Type Type_MapObjType = MapObjType.Slope.GetType();
    public void SetStr(object type)
    {
        if (type.GetType() == Type_TileType)
        {
            TileInfo info = MapSet.GetTileInfo((TileType)type);
            NameTxt.text = info.name;
            LogText.text = info.log;
        }
        else if (type.GetType() == Type_MapObjType)
        {
            int num = (int)(MapObjType)type;
            NameTxt.text = MapSet.mapObjLib.Name[num];
            LogText.text = MapSet.mapObjLib.Log[num];
        }
    }
}
