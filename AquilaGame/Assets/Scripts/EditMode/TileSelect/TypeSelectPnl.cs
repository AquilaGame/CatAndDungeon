using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TypeSelectPnl : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public GameObject TileSetTypeObj;
    public GameObject MapObjectSelectTypeObj;
    public delegate void TypeSelectBhv(TileType type);
    public delegate void TypeSelectPointerBhv(Transform transform);
    [SerializeField] GameObject toastObj = null;
    GameObject toast = null;
    Vector2 toastOffset;
    public TileSetPanel TileSetPnl;
    public MapObjSetPanel MapObjSetPnl;
    public BluePrint_Panel BluePrintScr;
    //public Transform[] Btns_bottom = new TypeSelectBtn[9];
    int nowPointer = -1;
    int nowPage = 0;
    public UnityEngine.UI.Scrollbar sbar;
    float ScrollSpeed = 1.0f;
    bool isPointerActive = false;
    public List<Transform> Btns = new List<Transform>();
    List<SelectBtnBase> nowBtnScripts = new List<SelectBtnBase>();
    //TileType[] TileTypeList = (TileType[])System.Enum.GetValues(typeof(TileType));
    //MapObjType[] MapObjTypeList = (MapObjType[])System.Enum.GetValues(typeof(MapObjType));
    System.Array TypeList = null;
    public System.Type Type_TileType = TileType.Grass.GetType();
    public System.Type Type_MapObjType = MapObjType.Slope.GetType();
    void Start()
    {
        FlushSelectType(Type_TileType);
        sbar.onValueChanged.AddListener(OnDragBar);
    }

    void Update()
    {
        if (isPointerActive)
        {
            float f = Input.GetAxis("Mouse ScrollWheel");
            if (f != 0)
            {
                f = f > 0 ? -ScrollSpeed : ScrollSpeed;
                f += sbar.value;
                if (f < 0.0f)
                    f = 0.0f;
                else if (f > 1.0f)
                    f = 1.0f;
                sbar.value = f;
            }
        }
    }

    public void FlushSelectType(System.Type type)
    {
        ClearSlot();
        TypeList = System.Enum.GetValues(type);
        nowPage = 0;
        int page = TypeList.Length / 9 + 1;
        sbar.numberOfSteps = (page > 5) ? page - 4 : 1;
        ScrollSpeed = 1.0f / sbar.numberOfSteps;
        if(type == Type_TileType)
            for (int i = 0; i < Btns.Count; i++)
            {
                var go = Instantiate(TileSetTypeObj, Btns[i]);
                TileSelectBtn btn = go.GetComponent<TileSelectBtn>();
                nowBtnScripts.Add(btn);
                btn.index = i;
                btn.SetBehaviour(SetToast, ResetToast, OnClickTileSetBtn);
            }
        else
            for (int i = 0; i < Btns.Count; i++)
            {
                var go = Instantiate(MapObjectSelectTypeObj, Btns[i]);
                MapObjSelectBtn btn = go.GetComponent<MapObjSelectBtn>();
                nowBtnScripts.Add(btn);
                btn.index = i;
                btn.SetBehaviour(SetToast, ResetToast, OnClickMapObjSetBtn);
            }
        sbar.value = 0;
        SelectFlushDis(0);
    }

    public void ClearSlot()
    {
        foreach (var v in nowBtnScripts)
        {
            Destroy(v.gameObject);
        }
        nowBtnScripts.Clear();
    }

    void SelectFlushDis(int page)//有机会改一下，这个效率很低
    {
        int j = 0;

        if (TypeList.GetValue(0).GetType() == Type_TileType)
        {
            for (int i = page * 9; i < TypeList.Length && j < 45; i++, j++)
            {
                ((TileSelectBtn)nowBtnScripts[j]).SetBtn(TypeList.GetValue(i));
            }
            for (; j < 45; j++)
            {
                ((TileSelectBtn)nowBtnScripts[j]).SetDisable();
            }
        }
        else
        {
            for (int i = page * 9; i < TypeList.Length && j < 45; i++, j++)
            {
                ((MapObjSelectBtn)nowBtnScripts[j]).SetBtn(TypeList.GetValue(i));
            }
            for (; j < 45; j++)
            {
                ((MapObjSelectBtn)nowBtnScripts[j]).SetDisable();
            }
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
            if (nowPointer != -1)
                SetToast(nowPointer);
        }
    }

    void OnClickTileSetBtn(int index)
    {
        if (nowBtnScripts[index].isEnable)
        {
            BluePrintScr.NowType = ((TileType[])TypeList)[index + nowPage * 9];
            TileInfo info = MapSet.GetTileInfo(((TileType[])TypeList)[index + nowPage * 9]);
            TileSetPnl.ChangeName(info.name);
            TileSetPnl.ChangeLog(info.log);
        }
    }
    void OnClickMapObjSetBtn(int index)
    {
        if (nowBtnScripts[index].isEnable)
        {
            MapObjSetPnl.SetUp(((MapObjType[])TypeList)[index + nowPage * 9],index + nowPage * 9);
        }
    }

    void SetToast(int index)
    {
        nowPointer = index;
        if (nowBtnScripts[index].isEnable)
        {
            if (toast == null)
            {
                toast = Instantiate(toastObj, Btns[index]);
                toastOffset = toast.GetComponent<RectTransform>().anchoredPosition;
                toast.GetComponent<TypeSelectToast>().SetStr(TypeList.GetValue(index + nowPage * 9));
            }
            else
            {
                toast.transform.SetParent(Btns[index]);
                toast.GetComponent<RectTransform>().anchoredPosition = toastOffset;
                toast.GetComponent<TypeSelectToast>().SetStr(TypeList.GetValue(index + nowPage * 9));
                toast.transform.SetParent(transform);
                toast.SetActive(true);
            }
        }
    }
    void ResetToast(int index)
    {
        nowPointer = -1;
        if (nowBtnScripts[index].isEnable)
        {
            if (toast)
                toast.SetActive(false);
        }
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
