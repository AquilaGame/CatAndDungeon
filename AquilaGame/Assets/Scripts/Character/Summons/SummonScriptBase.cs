using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SummonScriptBase : MonoBehaviour
{
    protected CharacterScript Creator;
    protected CharacterScript Self;
    [HideInInspector] public float Value;
    public bool isSkipSummon = false;
    [SerializeField] protected AudioClip SummonSound = null;
    public virtual void SetValue(CharacterScript calling, CharacterScript self, float value)
    {
        Creator = calling;
        Self = self;
        Value = value;
    }

    public virtual void Start()
    {
        if (isSkipSummon) return;
        OnScene.Report("<color=#55FFAA>[召唤法术]: </color><color=Orange>[" + Creator.data.Name
            + "]</color>召唤了一只<color=Yellow>[" + Self.data.Name + "]</color>。");
    }

}
