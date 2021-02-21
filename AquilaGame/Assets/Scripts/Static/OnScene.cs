using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OnSelectStruct
{
    public StaticBlock tile;
    public MapObject mapObject;
    public CharacterScript character;
    public SummonObjectBase summon;
    public void SetSelect(StaticBlock sblock)
    {
        if (tile != null)
            tile.ResetSelect();
        tile = sblock;
        tile.SetSelect();
        if (mapObject != null)
            mapObject.ResetSelect();
        mapObject = null;
        if (character != null)
            character.ResetSelect();
        character = null;
        if (summon != null)
            summon.ResetSelect();
        summon = null;
        if (OnScene.userPanel != null)
            OnScene.userPanel.SetSelect(sblock);
    }
    public void SetSelect(MapObject obj)
    {
        if (tile != null)
            tile.ResetSelect();
        tile = null;
        if (mapObject != null)
            mapObject.ResetSelect();
        mapObject = obj;
        mapObject.SetSelect();
        if (character != null)
            character.ResetSelect();
        character = null;
        if (summon != null)
            summon.ResetSelect();
        summon = null;
        if (OnScene.userPanel != null)
            OnScene.userPanel.SetSelect(obj);
    }
    public void SetSelect(SummonObjectBase summonObject)
    {
        if (tile != null)
            tile.ResetSelect();
        tile = null;
        if (mapObject != null)
            mapObject.ResetSelect();
        mapObject = null;
        if (character != null)
            character.ResetSelect();
        character = null;
        if (summon != null)
            summon.ResetSelect();
        summon = summonObject;
        summon.SetSelect();
        if (OnScene.userPanel != null)
            OnScene.userPanel.SetSelect(summon);
    }
    public void SetSelect(CharacterScript character)
    {
        if (tile != null)
            tile.ResetSelect();
        tile = null;
        if (mapObject != null)
            mapObject.ResetSelect();
        mapObject = null;
        if (summon != null)
            summon.ResetSelect();
        summon = null;
        if (this.character != null)
            this.character.ResetSelect();
        character.SetSelect();
        this.character = character;
        if (OnScene.userPanel != null)
            OnScene.userPanel.SetSelect(character);
    }
    public void SetSelectNull()
    {
        if (tile != null)
            tile.ResetSelect();
        //因为在创建角色的时候要保存图块信息，所以这个地方不能置null
        //tile = null;
        if (mapObject != null)
            mapObject.ResetSelect();
        mapObject = null;
        if (character != null)
            character.ResetSelect();
        character = null;
        if (summon != null)
            summon.ResetSelect();
        summon = null;
        if (OnScene.userPanel != null)
            OnScene.userPanel.SetSelectNull();
    }

    public void DisableSelect()
    {
        if (tile != null)
        {
            tile.ResetSelect();
        }
        else if (mapObject != null)
        {
            mapObject.ResetSelect();
        }
        else if (character != null)
        {
            character.ResetSelect();
        }
        else if (summon != null)
        {
            summon.ResetSelect();
        }
    }
    public void EnableSelect()
    {
        if (tile != null)
        {
            tile.SetSelect();
        }
        else if (mapObject != null)
        {
            mapObject.SetSelect();
        }
        else if (character != null)
        {
            character.SetSelect();
        }
        else if (summon != null)
        {
            summon.SetSelect();
        }
    }
}

public static class OnScene
{
    public static MainCam mainCam;
    static bool isOnPlayMode = false;
    public static bool isTimerModeOn = false;
    public static bool isPlayMode
    {
        get { return isOnPlayMode; }
        set
        {
            if (value)
            {
                onSelect.EnableSelect();
            }
            else
            {
                onSelect.DisableSelect();
            }
            isOnPlayMode = value;
        }
    }
    public static MiniMap minimap = null;
    public static BottomPanel bottomPnl = null;
    public static UserPanel userPanel = null;
    public static ReportWindow reporter = null;
    public static SceneManager manager = null;
    public static OnSelectStruct onSelect = new OnSelectStruct();
    public static bool isAllowedToChat = true;
    public static bool isChating = false;
    public static bool isMainUILocked = false;
    public static Character LoadCharacter(string name)
    {
        Character character = null;
        using (System.IO.StreamReader sr = new System.IO.StreamReader(OnGame.CharecterSavePath + name + OnGame.CharecterFileExtName))
        {
            character = new Character(sr);
        }
        return character;
    }

    public static void SaveCharacter(CharacterScript character)
    {
        string filename = "";
        if (character.GetComponent<SummonScriptBase>() != null)
        {
            filename = OnGame.CharecterSavePath + '#' + character.data.Name + OnGame.CharecterFileExtName;
        }
        else
        {
            filename = OnGame.CharecterSavePath + character.data.Name + OnGame.CharecterFileExtName;
        }
        
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
        {
            character.data.Save(sw);
            Report("角色[" + character.data.Name + "]已经保存");
        }
    }

    public static void Report(string str)
    {
        if (reporter != null)
            reporter.Show(str);
        if (true)
        {
            OnGame.Log(str);
        }
    }
    
}
