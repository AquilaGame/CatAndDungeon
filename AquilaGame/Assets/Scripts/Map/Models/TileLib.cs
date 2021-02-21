using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLib : MonoBehaviour
{
    public List<string> Name = new List<string>();
    public List<string> Log = new List<string>();
    public List<Material> TopMat = new List<Material>();
    public List<Material> SideMat = new List<Material>();
    private void Awake()
    {
        MapSet.lib = this;
        OnGame.Log("地图素材库建立连接");
    }
}
