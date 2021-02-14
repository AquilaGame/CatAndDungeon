using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile
{
    public delegate void BlockBhv();
    public delegate void BlockSideBhv(int dir, int h);
    public BlockBhv FlushBlock = null;
    public BlockSideBhv FlushSide = null;
    public BlockSideBhv CutSide = null;
    public TileType type = TileType.Grass;
    public TileInfo info = null;
    public MapObject mapObj = null;
    public GameObject tileObj = null;
    public Color color = Color.white;
    public int xloc = 0;
    public int zloc = 0;
    public int height = 0;
    public bool logChanged = false;
    public TerrainTile(TileType type, GameObject obj)
    {
        info = MapSet.GetTileInfo(type);
        tileObj = obj;
    }
    public void SetNewTile(TileType _type, int _height, Color _color)
    {
        type = _type;
        for (int i = 0; i < 4; i++)
            CutSide(i, _height);
        info = MapSet.GetTileInfo(_type);
        height = _height;
        color = _color;
        ClearMapObj();
        FlushBlock();
    }

    public void ClearMapObj()
    {
        if (mapObj != null)
        {
            Object.Destroy(mapObj.go);
            mapObj = null;
        }
    }

    public void SetTileLog(string str)
    {
        info.log = str;
    }
    public void SetMapObj(MapObject obj)
    {
        mapObj = obj;
    }
}

public class StaticBlock : MonoBehaviour
{
    [SerializeField] GameObject sideObj = null;
    public TerrainTile tile = null;
    public EPOOutline.Outlinable outline = null;
    //side:0->z+,1->x+,2->z-,3->x-
    List<GameObject>[] sides = new List<GameObject>[4] 
    {
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>()
    };

    void Awake()//改成个SetUp大概更好一些
    {
        tile = new TerrainTile(TileType.Grass, gameObject);
        tile.FlushBlock += ReColor;
        tile.FlushBlock();
        tile.FlushBlock += RePos;
        tile.FlushSide += RemakeSides;
        tile.CutSide += CutSide;
    }


    public void ResetSelect()
    {
        outline.enabled = false;
    }
    public void SetSelect()
    {
        outline.enabled = true;
    }
    void ReColor()
    {
        GetComponent<Renderer>().material = tile.info.topMat;
        GetComponent<Renderer>().material.color = tile.color;
    }

    void RePos()
    {
        transform.position = new Vector3(transform.position.x, tile.height, transform.position.z);
    }

    void CutSide(int dir, int h)
    {
        h = tile.height - h;
        if (h <= 0) return;
        for (; h > 0; h--)
        {
            if (sides[dir].Count > 0)
            {
                Destroy(sides[dir][sides[dir].Count - 1]);
                sides[dir].RemoveAt(sides[dir].Count - 1);
            }
        }
    }
    void RemakeSides(int dir, int h)
    {
        h = tile.height - h;
        if (h <= 0) return;
        foreach (var it in sides[dir])
        {
            Destroy(it);
        }
        //以后可以优化,而且有一个bug
        sides[dir].Clear();
        float xOffset, zOffset;
        Quaternion rotate;
        if (dir % 2 == 0)
        {
            xOffset = 0f;
            zOffset = (dir == 0 ? 0.5f : -0.5f);
            rotate = (dir == 0 ? MapSet.qFront: MapSet.qBack);
        }
        else
        {
            zOffset = 0f;
            xOffset = (dir == 1 ? 0.5f : -0.5f);
            rotate = (dir == 1 ? MapSet.qRight : MapSet.qLeft);
        }
        for (int i = 0; i < h; i++)
        {
            GameObject go = Instantiate(sideObj, MapSet.nowCreator.transform);
            go.transform.localPosition = new Vector3(tile.xloc + xOffset, tile.height - 0.5f - i, tile.zloc + zOffset);
            go.transform.localRotation = rotate;
            go.transform.parent = transform;
            go.GetComponent<Renderer>().material = tile.info.sideMat;
            go.GetComponent<Renderer>().material.color = tile.color;
            sides[dir].Add(go);
        }
    }
}
