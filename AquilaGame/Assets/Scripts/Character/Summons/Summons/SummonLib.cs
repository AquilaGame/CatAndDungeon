using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonLib : MonoBehaviour
{
    public List<GameObject> Calls = new List<GameObject>();
    public List<string> CallNames = new List<string>();
    public List<GameObject> CallObjs = new List<GameObject>();
    public List<string> CallObjNames = new List<string>();
    private void Awake()
    {
        OnGame.summonLib = this;
        OnGame.Log("召唤物库已建立连接");
    }
    public GameObject GetSummonCall(string name)
    {
        return Calls[CallNames.IndexOf(name)];
    }
    public GameObject GetSummonObjCall(string name)
    {
        return CallObjs[CallObjNames.IndexOf(name)];
    }
}
