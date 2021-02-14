using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static System.Convert;
public class MapCreator : MonoBehaviour
{
    public GameObject TileObj;
    public TerrainTile[,] TerrainMap = new TerrainTile[MapSet.mapsize, MapSet.mapsize];
    public Camera MiniMapCam = null;
    public List<CharacterScript> characters = new List<CharacterScript>();
    public int index = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        //MapSet.Creators.Add(this);
        //MapSet.nowCreator = this;
        
    }
    void Start()
    {
        CreateTerrain();
        GetComponent<NavMeshSurface>().BuildNavMesh();
        OnGame.Log("图块创建器初始化:" + gameObject.name);
        MapSet.InitMap(this,index);
        //MapSet.mapNeedFlush = true;
    }

    void CreateTerrain()
    {
        GameObject go = null;
        for (int x = 0; x < TerrainMap.GetLength(0); x++)
            for (int z = 0; z < TerrainMap.GetLength(1); z++)
            {
                go = Instantiate(TileObj,transform);
                TerrainMap[x, z] = go.GetComponent<StaticBlock>().tile;
                TerrainMap[x, z].xloc = x;
                TerrainMap[x, z].zloc = TerrainMap.GetLength(1) - 1 - z;
                go.transform.localPosition = new Vector3(TerrainMap[x, z].xloc, 0, TerrainMap[x, z].zloc);
            }
        
    }

    public void BuildMesh()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
        MiniMapCam.Render();
    }
    public void RemakeMeshLater()
    {
        StartCoroutine(RemakeMesh());
    }
    IEnumerator RemakeMesh()
    {
        yield return 0;
        BuildMesh();
        MapSet.mapNeedFlush = true;
    }

    public void SetBlock(int x, int z, TileType type, int height, Color color)
    {
        TerrainMap[x, z].SetNewTile(type, height, color);
        //检查侧面，side:0->z+,1->x+,2->z-,3->x-
        //Z反相调整：对调z-和z+的side值
        if (x > 0)//x-
        {
            if (height > TerrainMap[x - 1, z].height)
            {
                TerrainMap[x, z].FlushSide(3, TerrainMap[x - 1, z].height);
            }
            else if (TerrainMap[x - 1, z].height > height)
            {
                TerrainMap[x - 1, z].FlushSide(1, height);
            }
        }
        if (x < TerrainMap.GetLength(0) - 1)//x+
        {
            if (height > TerrainMap[x + 1, z].height)
            {
                TerrainMap[x, z].FlushSide(1, TerrainMap[x + 1, z].height);
            }
            else if (TerrainMap[x + 1, z].height > height)
            {
                TerrainMap[x + 1, z].FlushSide(3, height);
            }
        }
        if (z > 0)//z-
        {
            if (height > TerrainMap[x, z - 1].height)
            {
                TerrainMap[x, z].FlushSide(0, TerrainMap[x, z - 1].height);
            }
            else if (TerrainMap[x, z - 1].height > height)
            {
                TerrainMap[x, z - 1].FlushSide(2, height);
            }
        }
        if (z < TerrainMap.GetLength(1) - 1)//z+
        {
            if (height > TerrainMap[x, z + 1].height)
            {
                TerrainMap[x, z].FlushSide(2, TerrainMap[x, z + 1].height);
            }
            else if (TerrainMap[x, z + 1].height > height)
            {
                TerrainMap[x, z + 1].FlushSide(0, height);
            }
        }
    }

    public void RegistCharacter(CharacterScript characterScript, bool isShowInMiniMap = false)
    {
        characters.Add(characterScript);
        if (isShowInMiniMap)
        {
            OnScene.minimap.RegistMiniObj(characterScript.transform);
        }
    }

    public void UnregistCharacter(CharacterScript characterScript)
    {
        characters.Remove(characterScript);
        OnScene.minimap.UnregistMiniObj(characterScript.transform);
    }

    public void LoadMap(string filename)
    {
        OnScene.onSelect.SetSelectNull();
        for (int i = characters.Count - 1; i >= 0; i--)
        {
            characters[i].RemoveThis();
        }
        characters.Clear();
        using (System.IO.StreamReader rfile = new System.IO.StreamReader(filename))
        {
            int MapSize = ToInt32(rfile.ReadLine());
            for (int z = 0; z < MapSize; z++)
            {
                string[] mapline = rfile.ReadLine().Split(',');
                for (int x = 0; x < MapSize; x++)
                {
                    string[] tileRecord = mapline[x].Split('*');
                    SetBlock(x, z,
                        (TileType)ToInt32(tileRecord[0]),
                        ToInt32(tileRecord[1]),
                        new Color(
                            ToSingle(tileRecord[2]),
                            ToSingle(tileRecord[3]),
                            ToSingle(tileRecord[4])));
                }
            }
            int MapObjCount = ToInt32(rfile.ReadLine());
            for (int i = 0; i < MapObjCount; i++)
            {
                string[] objline = rfile.ReadLine().Split('*');
                int x = ToInt32(objline[0]);
                int y = ToInt32(objline[1]);
                int materialCount = ToInt32(objline[4]);
                int[] matnums = new int[materialCount];
                Color[] colors = new Color[materialCount];
                for (int j = 0; j < materialCount; j++)
                {
                    string[] matline = rfile.ReadLine().Split('*');
                    matnums[j] = ToInt32(matline[0]);
                    colors[j] = new Color(
                        ToSingle(matline[1]),
                        ToSingle(matline[2]),
                        ToSingle(matline[3])
                        );
                }
                TerrainMap[x, y].mapObj = new MapObject(
                    (MapObjType)ToInt32(objline[3]),
                    ToInt32(objline[2]),
                    matnums, colors, TerrainMap[x, y]);
                if (ToInt32(objline[5]) != 0)
                {
                    ;//ChangeMapObjectLog
                }
            }
            int LogCount = ToInt32(rfile.ReadLine());
            for (int i = 0; i < LogCount; i++)
            {
                ;//ChangeTileLog
            }
            int CharacterCount = ToInt32(rfile.ReadLine());
            for (int i = 0; i < CharacterCount; i++)
            {
                string[] characterStrs = rfile.ReadLine().Split('*');
                Vector3 pos = transform.position + new Vector3(ToSingle(characterStrs[0]), ToSingle(characterStrs[1]), ToSingle(characterStrs[2]));
                Quaternion rot = new Quaternion(ToSingle(characterStrs[3]), ToSingle(characterStrs[4]), ToSingle(characterStrs[5]), ToSingle(characterStrs[6]));
                Character character = OnScene.LoadCharacter(characterStrs[7]);
                if (characterStrs[7][0] == '#')
                {
                    OnScene.manager.LoadSummon(pos, rot, character);
                }
                else
                {
                    OnScene.manager.CreateCharacter(pos, rot, character);
                }
            }
            MapSet.mapNeedFlush = true;
        }
        BuildMesh();
    }



    public void SaveMap(string filename)
    {
        int MapObjCount = 0;
        int LogCount = 0;
        System.Text.StringBuilder MapObjSb = new System.Text.StringBuilder();
        System.Text.StringBuilder LogSb = new System.Text.StringBuilder();
        System.Text.StringBuilder CharacterSb = new System.Text.StringBuilder();
        using (System.IO.StreamWriter wfile = new System.IO.StreamWriter(
            MapSet.MapSavePath + filename + MapSet.MapSaveFileExtName))
        {
            wfile.WriteLine(MapSet.mapsize);
            for (int z = 0; z < TerrainMap.GetLength(1); z++)
            {
                for (int x = 0; x < TerrainMap.GetLength(0); x++)
                {
                    MapObject mObj = TerrainMap[x, z].mapObj;
                    if (mObj != null)
                    {
                        MapObjCount++;
                        MapObjSb.Append(x);
                        MapObjSb.Append('*');
                        MapObjSb.Append(z);
                        MapObjSb.Append('*');
                        MapObjSb.Append(mObj.dir);
                        MapObjSb.Append('*');
                        MapObjSb.Append((int)mObj.type);
                        MapObjSb.Append('*');
                        MapObjSb.Append(mObj.MatNums.Length);
                        MapObjSb.Append('*');
                        MapObjSb.Append(mObj.logChanged ? '1' : '0');
                        MapObjSb.Append('\n');
                        for (int i = 0; i < mObj.MatNums.Length; i++)
                        {
                            MapObjSb.Append(mObj.MatNums[i]);
                            MapObjSb.Append('*');
                            MapObjSb.Append(mObj.Colors[i].r);
                            MapObjSb.Append('*');
                            MapObjSb.Append(mObj.Colors[i].g);
                            MapObjSb.Append('*');
                            MapObjSb.Append(mObj.Colors[i].b);
                            MapObjSb.Append('\n');
                        }
                        if (mObj.logChanged)
                        {
                            MapObjSb.Append(mObj.log);
                            MapObjSb.Append('\n');
                        }
                    }
                    if (TerrainMap[x, z].logChanged)
                    {
                        LogSb.Append(x);
                        LogSb.Append('*');
                        LogSb.Append(z);
                        LogSb.Append('*');
                        LogSb.Append(TerrainMap[x,z].info.log);
                        LogSb.Append('\n');
                    }
                    wfile.Write((int)TerrainMap[x, z].type);
                    wfile.Write("*");
                    wfile.Write(TerrainMap[x, z].height);
                    wfile.Write("*");
                    wfile.Write(TerrainMap[x, z].color.r);
                    wfile.Write("*");
                    wfile.Write(TerrainMap[x, z].color.g);
                    wfile.Write("*");
                    wfile.Write(TerrainMap[x, z].color.b);
                    if (x != TerrainMap.GetLength(0) - 1)
                    {
                        wfile.Write(",");
                    }
                }
                wfile.Write("\n");
                
            }
            wfile.WriteLine(MapObjCount);
            wfile.Write(MapObjSb);
            wfile.WriteLine(LogCount);
            wfile.Write(LogSb);
            wfile.WriteLine(characters.Count);
            CharacterSb.Clear();
            for (int i = 0; i < characters.Count; i++)
            {
                OnScene.SaveCharacter(characters[i]);
                Transform trans = characters[i].transform;
                Vector3 pos = trans.position - transform.position;
                Quaternion rot = trans.rotation;
                CharacterSb.Append(pos.x.ToString());
                CharacterSb.Append('*');
                CharacterSb.Append(pos.y.ToString());
                CharacterSb.Append('*');
                CharacterSb.Append(pos.z.ToString());
                CharacterSb.Append('*');
                CharacterSb.Append(rot.x.ToString());
                CharacterSb.Append('*');
                CharacterSb.Append(rot.y.ToString());
                CharacterSb.Append('*');
                CharacterSb.Append(rot.z.ToString());
                CharacterSb.Append('*');
                CharacterSb.Append(rot.w.ToString());
                CharacterSb.Append('*');
                if (characters[i].GetComponent<SummonScriptBase>() != null)
                {
                    CharacterSb.Append("#" + characters[i].data.Name);
                }
                else
                    CharacterSb.Append(characters[i].data.Name);
                CharacterSb.Append('\n');
            }
            wfile.Write(CharacterSb);
        }
    }

}
