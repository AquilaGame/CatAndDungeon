using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] GameObject SelectChacterPanelObj = null;
    [SerializeField] GameObject RollPanelObj = null;
    [SerializeField] GameObject CombatPanelObj = null;
    public RangeCharacterSelector CharacterSelector = null;
    [HideInInspector] public GameObject scPnl = null;
    public SkillExecutor skillExecutor = new SkillExecutor();
    public Transform PlayModeCanvas = null;
    private void Awake()
    {
        OnScene.manager = this;
        OnGame.Log("场景管理器就位");
    }

    public void OpenSelectCharacterPanel()
    {
        if (scPnl)
            Destroy(scPnl);
        else 
            scPnl = Instantiate(SelectChacterPanelObj, PlayModeCanvas);
    }

    public RollPanel OpenRollPanel()
    {
        GameObject rPnl = Instantiate(RollPanelObj, PlayModeCanvas);
        return rPnl.GetComponent<RollPanel>();
    }

    public CombatPanel OpenCombatPanel()
    {

        GameObject cPnl = Instantiate(CombatPanelObj, PlayModeCanvas);
        return cPnl.GetComponent<CombatPanel>();
    }

    public void CreateCharacter(Vector3 pos, Quaternion rot, Character character,int copyCount = -1, bool PlaySound = false)
    {
        //print("建立：" + character.Name);
        GameObject goObj = OnGame.characterLib.Protypes[(int)character.birth][(int)character.OccupationData.type];
        if (goObj != null)
        {
            GameObject gameObjectInstance = Instantiate(goObj, pos, rot);
            if (copyCount > 0)
            {
                character.Name += "#" + copyCount.ToString();
            }
            CharacterScript cs = gameObjectInstance.GetComponent<CharacterScript>();
            cs.SetUp(character);
            MapSet.nowCreator.RegistCharacter(cs,true);
            if (PlaySound)
                cs.PlaySound(CharacterSoundType.Birth);
        }

    }

    public void LoadSummon(Vector3 pos, Quaternion rot, Character character, int copyCount = -1)
    {
        GameObject goObj = OnGame.summonLib.GetSummonCall(character.Name.Split('#')[0]);
        if (goObj != null)
        {
            GameObject gameObjectInstance = Instantiate(goObj, pos, rot);
            if (copyCount > 0)
            {
                character.Name += "#" + copyCount.ToString();
            }
            gameObjectInstance.GetComponent<CharacterScript>().SetUp(character);
            MapSet.nowCreator.RegistCharacter(gameObjectInstance.GetComponent<CharacterScript>(), true);
            gameObjectInstance.GetComponent<SummonScriptBase>().isSkipSummon = true;
        }
    }


    public CharacterScript SummonCall(Vector3 pos, Quaternion rot,CharacterScript Creater, string name, float argv)
    {
        GameObject go = Instantiate(OnGame.summonLib.GetSummonCall(name), pos, rot);
        Character character = OnScene.LoadCharacter("#" + name);
        character.Name = name;
        System.IO.DirectoryInfo savefolder = new System.IO.DirectoryInfo(OnGame.CharecterSavePath);
        System.IO.FileInfo[] files = savefolder.GetFiles('*' + OnGame.CharecterFileExtName);
        List<string> ExistFileNames = new List<string>();
        foreach (var f in files)
            ExistFileNames.Add(f.Name.Split('.')[0]);
        int i; string Copyname;
        for (i = 1; i <= 1024; i++)
        {
            Copyname = "#" + name + '#' + i.ToString();
            //先看存档里有没有重名的
            if (ExistFileNames.IndexOf(Copyname) == -1)
            {
                bool hasExist = false;
                //再分别查看各个地图看有没有重名的
                foreach (var v in MapSet.Creators)
                {
                    if (v.characters.Find((CharacterScript cs) => { return "#" + cs.data.Name == Copyname; }) != null)
                    {
                        hasExist = true;
                        break;
                    }
                }
                //没有的话这个名字就可以用
                if (hasExist == false)
                {
                    break;
                }
            }
        }
        character.Name += "#" + i.ToString();
        character.characterType = Creater.data.characterType;
        CharacterScript chs = go.GetComponent<CharacterScript>();
        chs.SetUp(character);
        MapSet.nowCreator.RegistCharacter(chs, true);
        go.GetComponent<SummonScriptBase>().SetValue(Creater, chs, argv);
        return chs;
    }

    public void RemoveCharacter(CharacterScript cs)
    {
        MapSet.nowCreator.UnregistCharacter(cs);
        OnScene.onSelect.SetSelectNull();
        OnScene.bottomPnl.SetInfo("未选择", "--", "没有选中的对象");
        Destroy(cs.gameObject);
    }

    public void SummonObjCall(Vector3 pos, Quaternion rot, CharacterScript Creater, string name, float argv1, float argv2)
    {
        GameObject go = Instantiate(OnGame.summonLib.GetSummonObjCall(name), pos, rot, MapSet.nowCreator.transform);
        go.GetComponent<SummonObjectBase>().SetUp(Creater, name, argv1, argv2);
    }
}
