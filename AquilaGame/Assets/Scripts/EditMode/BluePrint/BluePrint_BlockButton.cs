using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BluePrint_BlockButton : MonoBehaviour,
    IPointerDownHandler,IPointerEnterHandler
{
    public delegate void OnBluePrintPointerBhv(int x, int y);
    [SerializeField] Text height_txt = null;
    public int[] loc = new int[2];
    public OnBluePrintPointerBhv setBlock;
    public OnBluePrintPointerBhv setType;
    public void SetBlockBtn(TerrainTile tile)
    {
        GetComponent<RawImage>().texture = tile.info.topMat.mainTexture;
        GetComponent<RawImage>().color = tile.color;
        height_txt.text = tile.height.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (eventData.pointerId)
        {
            //鼠标右键
            case -2:
                setType(loc[0], loc[1]);
                break;
            //鼠标中键
            case -3:
                break;
            //鼠标左键
            default:
                setBlock(loc[0], loc[1]);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) == true)
            setBlock(loc[0], loc[1]);
    }
}
