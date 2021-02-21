using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class TileSetPanel : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] Dropdown ColorSelect = null;
    [SerializeField] Slider HeightSld = null;
    [SerializeField] Color[] ColorList = new Color[16];
    [SerializeField] BluePrint_Panel BLP = null;
    [SerializeField] Text H_txt = null;
    [SerializeField] Text name_txt = null;
    [SerializeField] Text log_txt = null;
    bool isPointerActive = false;
    const float SlideSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        ColorSelect.onValueChanged.AddListener(ChangeColor);
        ChangeColor(0);
        HeightSld.onValueChanged.AddListener(ChangeHeight);
        ChangeHeight(0);
        TileInfo info = MapSet.GetTileInfo(TileType.Grass);
        ChangeName(info.name);
        ChangeLog(info.log);
    }

    void Update()
    {
        if (isPointerActive)
        {
            float f = Input.GetAxis("Mouse ScrollWheel");
            if (f != 0)
            {
                f = f > 0 ? SlideSpeed : -SlideSpeed;
                HeightSld.value += f;
            }
        }
    }

    void ChangeColor(int cl)
    {
        BLP.NowColor = ColorList[cl];
    }

    void ChangeHeight(float h)
    {
        BLP.NowHeight = (int)h;
        H_txt.text = BLP.NowHeight.ToString();
    }

    public void ChangeName(string str)
    {
        name_txt.text = str;
    }

    public void ChangeLog(string str)
    {
        log_txt.text = str;
    }

    public void BluePrintReflect(string name, string log, int height, Color color)
    {
        name_txt.text = name;
        log_txt.text = log;
        HeightSld.value = height;
        int i = 0;
        for (; i < ColorList.Length; i++)
            if (ColorList[i] == color)
                break;
        ColorSelect.value = i;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerActive = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerActive = false;
    }
}
