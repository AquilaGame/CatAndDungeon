using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Grass,
    Soil,
    FarmLand,
    MushroomSoil,//湿滑
    SnowSoil,
    LiteWood,
    DarkWood,
    TreatedWood,
    Sand,
    SandStone,
    FaceSandStone,
    RedSandStone,
    FaceRedSandStone,
    StoneBlock,//不稳
    GreenStoneBlock,//湿滑
    PolishedStone,
    SmoothStone,
    StoneBrick,
    FaceStoneBrick,
    GreenStoneBrick,//湿滑
    CrackedStoneBrick,//不稳
    StrangeStoneBrick,
    RedBrick,
    BlackBrick,
    CrackedBlackBrick,
    FaceBlackBrick,
    SeaBrick,
    DarkSeaBrick,
    NetherStone,
    RedSoil,
    GreenSoil,//湿滑
    NetherBrick,
    FaceNetherBrick,
    ChackedNetherBrick,
    RedNetherBrick,
    QuartzBlock,
    QuartzBrick,
    PolishedGranite,
    MagicBrick,
    AlchemicalBlock_A,
    AlchemicalBlock_B,
    EndStoneBrick,
    Ice,//湿滑
    Ore,
    Snow,//难行
    Water,
    Lava,//有害
    SoulSand,//难行
    RedStone,
    CreatureBlock,
    TaintStone,//有害
    GoldBlock,
    LiteMetalBlock,
    MetalBlock,
    HeavyMetalBlock,
    MetalBrick,
    MetalPlane,//不稳
    MetalFluidPlacer,
    Concrete,
    Glass,
    Wool,
    Flesh
}

public class TileInfo
{
    public string name;
    public string log;
    public Material topMat;
    public Material sideMat;
    public TileInfo(string _name, string _log, Material _mat1, Material _mat2)
    {
        name = _name;
        log = _log;
        topMat = _mat1;
        sideMat = _mat2;
    }
    public TileInfo(TileInfo source)
    {
        name = source.name;
        log = source.log;
        topMat = source.topMat;
        sideMat = source.sideMat;
    }
}

public static class MapSet
{
    public const string MapSavePath = @"saves\map\";
    public const string MapSaveFileExtName = @".gamemap";
    public const int mapsize = 40;
    public static int WaitToInit = 9;
    public static TileLib lib;
    public static MapObjectLib mapObjLib;
    public static MapCreator[] Creators = new MapCreator[9];
    public static MapCreator nowCreator = null;
    public static bool mapNeedFlush = false;
    public static Quaternion qFront = Quaternion.Euler(0f, 180f, 0f);
    public static Quaternion qBack = Quaternion.identity;
    public static Quaternion qRight = Quaternion.Euler(0f, -90f, 0f);
    public static Quaternion qLeft = Quaternion.Euler(0f, 90f, 0f);
    public static TileInfo GetTileInfo(TileType type)
    {
        int i = (int)type;
        return new TileInfo(
            lib.Name[i],
            lib.Log[i],
            lib.TopMat[i],
            lib.SideMat[i]);
    }
    public static void InitMap(MapCreator mapCreator,int index)
    {
        Creators[index] = mapCreator;
        mapCreator.gameObject.SetActive(false);
        WaitToInit--;
        if (0 == WaitToInit)
        {
            nowCreator = Creators[4];
            SetWorkMap(Creators[0]);
        }
    }
    public static void SetWorkMap(int i)
    {
        SetWorkMap(Creators[i]);
    }
    public static void SetWorkMap(MapCreator mapCreator)
    {
        OnScene.onSelect.SetSelectNull();
        if(nowCreator.gameObject.activeSelf)
            nowCreator.gameObject.SetActive(false);
        mapCreator.gameObject.SetActive(true);
        OnScene.mainCam.transform.position = OnScene.mainCam.transform.position + mapCreator.transform.position - nowCreator.transform.position;
        foreach (var v in nowCreator.characters)
        {
            OnScene.minimap.UnregistMiniObj(v.transform);
        }
        nowCreator = mapCreator;
        OnScene.Report("地图切换到:" + nowCreator.gameObject.name);
        foreach (var v in nowCreator.characters)
        {
            OnScene.minimap.RegistMiniObj(v.transform);
        }
        nowCreator.MiniMapCam.Render();
        mapNeedFlush = true;
    }


}
