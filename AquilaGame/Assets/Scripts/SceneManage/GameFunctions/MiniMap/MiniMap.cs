using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapPoint
{
    public GameObject pointObj = null;
    public RectTransform point = null;
    public Transform trans = null;
    public void FlushMiniMapObj(Vector2 wndSize)
    {
        Vector3 temp = trans.position - MapSet.nowCreator.transform.position;
        point.anchoredPosition = new Vector2(
            temp.x * wndSize.x / MapSet.mapsize,
            15.0f + temp.z * wndSize.y / MapSet.mapsize
            );
    }
    public MiniMapPoint(GameObject Obj, Transform target, Transform panel)
    {
        pointObj = Object.Instantiate(Obj,panel);
        point = pointObj.GetComponent<RectTransform>();
        trans = target;
        pointObj.GetComponent<MiniMapObj>().SetUp(target.GetComponent<CharacterScript>().data);
    }
}

public class MiniMap : MonoBehaviour
{
    [SerializeField]GameObject MiniMapObj = null;
    List<MiniMapPoint> MiniMapObjs = new List<MiniMapPoint>();
    [SerializeField]RectTransform MiniMap_Map = null;
    // Start is called before the first frame update
    void Awake()
    {
        OnScene.minimap = this;
        OnGame.Log("小地图功能已注册");
    }

    // Update is called once per frame
    void Update()
    {
        if (OnScene.isPlayMode)
        {
            foreach (var v in MiniMapObjs)
            {
                v.FlushMiniMapObj(MiniMap_Map.sizeDelta);
            }
        }
    }

    public void RegistMiniObj(Transform obj)
    {
        if (MiniMapObjs.Exists((o) => o.trans == obj))
        {
            ;
        }
        else
        {
            MiniMapObjs.Add(new MiniMapPoint(MiniMapObj, obj, MiniMap_Map));
        }
    }

    public void UnregistMiniObj(Transform obj)
    {
        int index = MiniMapObjs.FindIndex((o) => o.trans == obj);
        if (index != -1)
        {
            Destroy(MiniMapObjs[index].pointObj);
            MiniMapObjs.RemoveAt(index);
        }
    }
}
