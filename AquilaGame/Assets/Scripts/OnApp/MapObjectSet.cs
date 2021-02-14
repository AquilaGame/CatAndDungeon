using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapObjType
{
    Slope,
    Stairs,
    Fence,
    Wall,
    Liquid,
    Table,
    Torch,
    WorkTable,
    Anvil,
    Chest,
    Barrel,
    Jar,
    TrapDoor,
    Door,
    Ladder,
    Sign,
    TaskBoard,
    Flag,
    Tree,
    SmallPlant,
    Altar,
    Portal,
    Totem,
    BonFire,
    GlassPlate
}
public class MapObject
{
    public GameObject go = null;
    public Renderer render = null;
    public MapObjType type;
    public TerrainTile parent = null;
    public int[] MatNums;
    public Color[] Colors;
    public string name;
    public string log;
    public bool logChanged = false;
    public bool isVisibleToPlayer = true;
    public int dir = 0;
    public void setVisible(bool set_to)
    {
        isVisibleToPlayer = set_to;
    }
    public MapObject(MapObjSetPanel pnl, TerrainTile terrainTile)
    {
        type = pnl.nowType;
        dir = pnl.nowDir;
        name = MapSet.mapObjLib.Name[(int)type];
        log = MapSet.mapObjLib.Log[(int)type];
        parent = terrainTile;
        go = Object.Instantiate(pnl.nowObj, terrainTile.tileObj.transform);
        go.transform.localEulerAngles = new Vector3(0.0f, GetDir(dir), 0.0f);
        render = go.GetComponent<Renderer>();
        MatNums = new int[pnl.nowMatList.Count];
        System.Array.Copy(pnl.nowMatNum, MatNums, MatNums.Length);
        Colors = new Color[MatNums.Length];
        System.Array.Copy(pnl.nowColors, Colors, Colors.Length);
    }
    public MapObject(MapObjType _type, int _dir, int[] _matnums, Color[] _colors, TerrainTile terrainTile)
    {
        type = _type;
        dir = _dir;
        name = MapSet.mapObjLib.Name[(int)type];
        log = MapSet.mapObjLib.Log[(int)type];
        parent = terrainTile;
        go = Object.Instantiate(MapSet.mapObjLib.Objs[(int)type], terrainTile.tileObj.transform);
        go.transform.localEulerAngles = new Vector3(0.0f, GetDir(dir), 0.0f);
        render = go.GetComponent<Renderer>();
        MatNums = _matnums;
        Colors = _colors;
        SetMat();
    }

    public void SetMat()
    {
        Material[] materials = new Material[MatNums.Length];
        for (int i = 0; i < MatNums.Length; i++)
        {
            materials[i] = MapSet.mapObjLib.materials[MatNums[i]];
        }
        render.materials = materials;
        for (int i = 0; i < materials.Length; i++)
            render.materials[i].color = Colors[i];
    }

    public void SetMat(Renderer renderer)
    {
        Material[] materials = new Material[MatNums.Length];
        for (int i = 0; i < MatNums.Length; i++)
        {
            materials[i] = MapSet.mapObjLib.materials[MatNums[i]];
        }
        renderer.materials = materials;
        for (int i = 0; i < materials.Length; i++)
            renderer.materials[i].color = Colors[i];
    }



    static public float GetDir(int dir)
    {
        switch (dir)
        {
            //N
            case 0:
                return 0.0f;
            //E
            case 1:
                return 90.0f;
            //W
            case 2:
                return 270.0f;
            //S
            case 3:
                return 180.0f;
            default:
                return -1;
        }
    }

    public void ResetSelect()
    {
        if (go != null)
            go.GetComponent<EPOOutline.Outlinable>().enabled = false;
    }
    public void SetSelect()
    {
        if (go != null)
            go.GetComponent<EPOOutline.Outlinable>().enabled = true;
    }
    public void Destroy()
    {
        MapSet.nowCreator.RemakeMeshLater();
        OnScene.userPanel.SetSelectNull();
        parent.ClearMapObj();
    }



}
