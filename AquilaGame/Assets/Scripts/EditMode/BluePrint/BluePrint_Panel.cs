using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class BluePrintBlock
{
    public GameObject obj;
    public BluePrint_BlockButton btnStript;
    public GameObject MapObject2D = null;
    public BluePrintBlock(GameObject go)
    {
        obj = go;
        btnStript = go.GetComponent<BluePrint_BlockButton>();
    }
}

public class BluePrint_Panel : MonoBehaviour
{
    [SerializeField]  GameObject BlockButton = null;
    BluePrintBlock[,] Blocklist = new BluePrintBlock[MapSet.mapsize, MapSet.mapsize];
    public Color NowColor = Color.white;
    public int NowHeight = 0;
    public TileType NowType = TileType.Grass;
    public TileSetPanel tileSetPanel;
    public MapObjSetPanel mapObjSetPanel;
    public float CleanAssetsDeltaTime = 2;
    float TimeToCleanAssets = 0;
    public bool isMatObjEditMode = false;
    // Start is called before the first frame update
    void Start()
    {
        CreateMap();
        TimeToCleanAssets = Time.time;
    }

    void Update()
    {
        if (MapSet.mapNeedFlush)
        {
            FlushMap(MapSet.nowCreator);
            MapSet.mapNeedFlush = false;
        }
        //内存清理大师
        if (Time.time > TimeToCleanAssets)
        {
            Resources.UnloadUnusedAssets();
            TimeToCleanAssets += CleanAssetsDeltaTime;
        }
    }

    void CreateMap()
    {
        for (int y = 0; y < MapSet.mapsize; y++)
            for (int x = 0; x < MapSet.mapsize; x++)
            {
                BluePrintBlock bl = new BluePrintBlock(Instantiate(BlockButton, transform));
                bl.obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * 16.0f + 8.0f, y * -16.0f - 8.0f);
                //bl.obj.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(x * 16.0f + 8.0f, y * -16.0f - 8.0f,10);
                bl.btnStript.loc[0] = x;
                bl.btnStript.loc[1] = y;
                bl.btnStript.setBlock = OnPointerSetBlock;
                bl.btnStript.setType = OnPointerSetType;
                Blocklist[x, y] = bl;
            }
    }

    void FlushMap(MapCreator creator)
    {
        for (int y = 0; y < MapSet.mapsize; y++)
            for (int x = 0; x < MapSet.mapsize; x++)
            {
                if (Blocklist[x, y].MapObject2D != null)
                    Destroy(Blocklist[x, y].MapObject2D);
                Blocklist[x, y].btnStript.SetBlockBtn(creator.TerrainMap[x,y]);
                if (creator.TerrainMap[x, y].mapObj != null)
                    Set2DMapObj(x, y, creator.TerrainMap[x, y].mapObj);
            }
    }

    void Set2DMapObj(int x, int y, MapObject mObj)
    {
        Blocklist[x, y].MapObject2D = Instantiate(MapSet.mapObjLib.Objs2D[(int)mObj.type], Blocklist[x, y].obj.transform);
        Blocklist[x, y].MapObject2D.transform.localEulerAngles = new Vector3(0, 0, MapObject.GetDir(mObj.dir));
        mObj.SetMat(Blocklist[x, y].MapObject2D.GetComponent<Renderer>());
    }

    void OnPointerSetBlock(int x, int y)
    {
        if (isMatObjEditMode)
        {
            if (Blocklist[x, y].MapObject2D != null)
                Destroy(Blocklist[x, y].MapObject2D);
            if (MapSet.nowCreator.TerrainMap[x, y].mapObj != null)
            {
                Destroy(MapSet.nowCreator.TerrainMap[x, y].mapObj.go);
                //MapSet.nowCreater.TerrainMap[x, y].mapObj = null;这句话不用了因为下面直接赋值了
            }
            MapObject Mobj = MapSet.nowCreator.TerrainMap[x, y].mapObj = new MapObject(mapObjSetPanel, MapSet.nowCreator.TerrainMap[x, y]);
            mapObjSetPanel.SetMat(Mobj.render);
            Set2DMapObj(x, y, Mobj);
        }
        else
        {
            MapSet.nowCreator.SetBlock(x, y, NowType, NowHeight, NowColor);
            Blocklist[x, y].btnStript.SetBlockBtn(MapSet.nowCreator.TerrainMap[x, y]);
        }
    }
    void OnPointerSetType(int x, int y)
    {
        if (isMatObjEditMode)
        {
            if (MapSet.nowCreator.TerrainMap[x, y].mapObj != null)
            {
                Destroy(MapSet.nowCreator.TerrainMap[x, y].mapObj.go);
                MapSet.nowCreator.TerrainMap[x, y].mapObj = null;
                Destroy(Blocklist[x, y].MapObject2D);
            }
        }
        else
        {
            NowType = MapSet.nowCreator.TerrainMap[x, y].type;
            NowHeight = MapSet.nowCreator.TerrainMap[x, y].height;
            NowColor = MapSet.nowCreator.TerrainMap[x, y].color;
            TileInfo info = MapSet.GetTileInfo(NowType);
            tileSetPanel.BluePrintReflect(info.name, info.log, NowHeight, NowColor);
        }
    }

}
