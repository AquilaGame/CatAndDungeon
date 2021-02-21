using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectLib : MonoBehaviour
{
    public List<string> Name = new List<string>();
    public List<string> Log = new List<string>();
    public List<GameObject> Icons = new List<GameObject>();
    public List<GameObject> Objs = new List<GameObject>();
    public List<GameObject> Objs2D = new List<GameObject>();
    public List<Material> materials = new List<Material>();
    //允许更改的材质
    static readonly List<List<int>> SlopeMatList = new List<List<int>>{
        new List<int>{0,2,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24,27,28,29,
         30,31,32,33,34,37,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,59,61,62,64,65,66,67,68},
        new List<int>
        {1,3,4,6,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,
         28,29,30,31,32,33,34,37,38,40,42,43,44,45,46,47,48,49,50,51,52,53,
         54,55,56,57,59,61,63,64,65,66,67,68}
    };
    static readonly List<List<int>> StairsMatList = new List<List<int>>
    {
        new List<int>{29,0,1,2,3,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,
            25,26,27,28,30,31,32,33,34,35,37,38,39,41,42,43,44,45,46,47,48,49,50,
            51,52,53,54,55,56,57,58,59,61,62,63,64,65,66,68}
    };
    static readonly List<List<int>> FenceMatList = new List<List<int>>
    {
        new List<int>{34,33,32}
    };
    static readonly List<List<int>> WallMatList = new List<List<int>>
    {
        new List<int>{11,2,3,4,8,9,10,12,13,15,16,17,18,19,20,21,22,23,24,25,26,27,28,
        29,30,32,33,34,37,38,43,44,45,46,48,49,50,51,52,54,55,56,57,59,62,64,65,66,68}
    };
    static readonly List<List<int>> LiquidMatList = new List<List<int>>
    {
        new List<int>{60,59,66,67}
    };
    static readonly List<List<int>> TableMatList = new List<List<int>>
    {
        new List<int>{33,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24,27,28,29,
         30,31,32,34,37,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,58,59,61,62,64,65,66,67,68}
    };
    static readonly List<List<int>> TorchMatList = new List<List<int>>
    {
        new List<int>{78}
    };
    static readonly List<List<int>> WorkTableMatList = new List<List<int>>
    {
        new List<int>{87,89,91,93},
        new List<int>{88,90,92,94}
    };
    static readonly List<List<int>> AnvilMatList = new List<List<int>>
    {
        new List<int>{49,50,51,52},
    };
    static readonly List<List<int>> ChestMatList = new List<List<int>>
    {
        new List<int>{75,76,77},
    };
    static readonly List<List<int>> BarrelMatList = new List<List<int>>
    {
        new List<int>{33,49,50,51,52,34,32},
        new List<int>{ 49, 50, 51, 52 }
    };
    static readonly List<List<int>> JarMatList = new List<List<int>>
    {
        new List<int>{32, 49, 50,51,52,34,33},
        new List<int>{69,70,71,72,73,74}
    };
    static readonly List<List<int>> TrapDoorMatList = new List<List<int>>
    {
        new List<int>{50,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,
            25,26,27,28,29,30,31,32,33,34,35,37,38,39,41,42,43,44,45,46,47,48,49,
            51,52,53,54,55,56,57,59,61,62,63,64,65,66,68},
        new List<int>{33,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,
            25,26,27,28,29,30,31,32,34,35,37,38,39,41,42,43,44,45,46,47,48,49,50,
            51,52,53,54,55,56,57,59,61,62,63,64,65,66,68}
    };
    static readonly List<List<int>> DoorMatList = new List<List<int>>
    {
        new List<int>{33,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,
            25,26,27,28,29,30,31,32,34,35,37,38,39,41,42,43,44,45,46,47,48,49,50,
            51,52,53,54,55,56,57,59,61,62,63,64,65,66,68},
        new List<int>{50,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,
            25,26,27,28,29,30,31,32,33,34,35,37,38,39,41,42,43,44,45,46,47,48,49,
            51,52,53,54,55,56,57,59,61,62,63,64,65,66,68}
    };
    static readonly List<List<int>> LadderMatList = new List<List<int>>
    {
        new List<int>{34,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,
            25,26,27,28,29,30,31,32,33,35,37,38,39,41,42,43,44,45,46,47,48,49,50,
            51,52,53,54,55,56,57,59,61,62,63,64,65,66,68}
    };
    static readonly List<List<int>> SignMatList = new List<List<int>>
    {
        new List<int>{34,33,32}
    };
    static readonly List<List<int>> TaskBoardMatList = new List<List<int>>
    {
        new List<int>{34,33,32,55},
        new List<int>{11,8,9,10,12,13,14,15,16,17,18,19,20,21,24,27,28,29,
         30,31,32,33,34,37,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,59,61,62,64,65,66,67,68},
        new List<int>{30,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24,27,28,29,
         31,32,33,34,37,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,59,61,62,64,65,66,67,68}
    };
    static readonly List<List<int>> FlagMatList = new List<List<int>>
    {
        new List<int>{96,97,98,99,100,101,102,103,104,105,106,107,108,109,
        110,111,112,113,114,115,116,117},
        new List<int>{34,33,32},
        new List<int>{68}
    };
    static readonly List<List<int>> TreeMatList = new List<List<int>>
    {
        new List<int>{83,84,85,86},
        new List<int>{79,80,81,82},
    };
    static readonly List<List<int>> SmallPlantMatList = new List<List<int>>
    {
        new List<int>{137,121,122,123,124,125,126,127,128,129,130,131,132,
            133,134,135,136,120}
    };
    static readonly List<List<int>> AltarMatList = new List<List<int>>{
        new List<int>{18,0,2,4,5,7,8,9,10,11,12,13,14,15,16,17,19,20,21,24,27,28,29,
         30,31,32,33,34,37,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,59,61,62,64,65,66,67,68},
        new List<int>{95},
        new List<int>{65,0,2,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24,27,28,29,
         30,31,32,33,34,37,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,59,61,62,64,66,67,68}
    };
    static readonly List<List<int>> PortalMatList = new List<List<int>>{
        new List<int>{37,2,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24,27,28,29,
         30,31,32,33,34,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,59,61,62,64,65,66,67,68},
        new List<int>{118,119}
    };
    static readonly List<List<int>> TotemMatList = new List<List<int>>{
        new List<int>{23,2,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24,25,26,27,28,29,
         30,31,32,33,34,37,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,59,61,62,64,65,66,67,68},
        new List<int>{59,0,2,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24,27,28,29,
         30,31,32,33,34,37,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,60,61,62,64,65,66,67,68},
        new List<int>{50,2,4,5,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24,27,28,29,
         30,31,32,33,34,37,38,39,41,43,44,45,46,47,48,49,51,52,53,54,
         55,56,57,59,61,62,64,65,66,67,68}
    };
    static readonly List<List<int>> BonFireMatList = new List<List<int>>
    {
        new List<int>{33,8,9,10,11,12,13,14,15,16,17,18,19,20,21,24,27,28,29,
         30,31,32,34,37,38,39,41,43,44,45,46,47,48,49,50,51,52,53,54,
         55,56,57,58,59,61,62,64,65,66,67,68}
    };
    static readonly List<List<int>> GlassPlateMatList = new List<List<int>>
    {
        new List<int>{32, 49, 50,51,52,34,33},
        new List<int>{69,70,71,72,73,74}
    };
    private void Awake()
    {
        MapSet.mapObjLib = this;
        OnGame.Log("地图活动对象库建立连接");
    }
    public List<List<int>> GetMatList(MapObjType type)
    {
        switch (type)
        {
            case MapObjType.Slope: return SlopeMatList;
            case MapObjType.Stairs: return StairsMatList;
            case MapObjType.Fence: return FenceMatList;
            case MapObjType.Wall: return WallMatList;
            case MapObjType.Liquid: return LiquidMatList;
            case MapObjType.Table: return TableMatList;
            case MapObjType.Torch: return TorchMatList;
            case MapObjType.WorkTable: return WorkTableMatList;
            case MapObjType.Anvil: return AnvilMatList;
            case MapObjType.Chest: return ChestMatList;
            case MapObjType.Barrel: return BarrelMatList;
            case MapObjType.Jar: return JarMatList;
            case MapObjType.TrapDoor: return TrapDoorMatList;
            case MapObjType.Door: return DoorMatList;
            case MapObjType.Ladder: return LadderMatList;
            case MapObjType.Sign: return SignMatList;
            case MapObjType.TaskBoard: return TaskBoardMatList;
            case MapObjType.Flag: return FlagMatList;
            case MapObjType.Tree: return TreeMatList;
            case MapObjType.SmallPlant: return SmallPlantMatList;
            case MapObjType.Altar: return AltarMatList;
            case MapObjType.Portal: return PortalMatList;
            case MapObjType.Totem: return TotemMatList;
            case MapObjType.BonFire:return BonFireMatList;
            case MapObjType.GlassPlate: return GlassPlateMatList;
            default: return new List<List<int>> { };
        }
    }
}
