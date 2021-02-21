using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SummonObjectBase : MonoBehaviour
{
    [HideInInspector] public string Name = "";
    [HideInInspector] public string Log = "";
    protected float[] Values = new float[2];
    protected CharacterScript Creator = null;
    public bool isSkipSummon = false;
    public AudioClip SummonSound = null;
    public AudioClip OnSelectSound = null;
    public AudioClip OnRoundStartSound = null;
    [SerializeField] EPOOutline.Outlinable outline = null;
    // Start is called before the first frame update
    public virtual void Start()
    {
        MapSet.nowCreator.BuildMesh();
        if (isSkipSummon) return;
        Sound.Play(SummonSound, this);
        OnScene.Report("<color=#55FFAA>[召唤法术]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>在指定位置召唤了<color=Yellow>[" + Name + "]</color>。");
    }

    public virtual void SetUp(CharacterScript calling, string name, float value0, float value1)
    {
        Creator = calling;
        Name = name;
        Values[0] = value0;
        Values[1] = value1;
    }

    public virtual void OnRoundStart()
    {

    }

    public virtual void ResetSelect()
    {
        outline.enabled = false;
    }
    public virtual void SetSelect()
    {
        outline.enabled = true;
        Sound.Play(OnSelectSound,this);
    }

    public virtual void RemoveThis()
    {
        Destroy(gameObject);
    }

    public virtual void OnRightClick(CharacterScript user)
    {
        OnScene.Report("<color=#55FFAA>[提示]: </color><color=Yellow>[" + Name + "]</color>无法被选定的角色使用。");
    }

    public virtual void OnDestroy()
    {
        MapSet.nowCreator.BuildMesh();
    }

}
