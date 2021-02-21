using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectPnl : MonoBehaviour
{
    [SerializeField] Transform ScrollViewContent = null;
    [SerializeField] Transform MagicBookViewContent = null;
    [SerializeField] GameObject CharacterSelectButtonObj = null;
    [SerializeField] GameObject CharacterCreatePnlObj = null;
    [SerializeField] GameObject MagicBookPageObj = null;
    GameObject CharacterCreateBtn = null;
    [SerializeField] Text NameTxt = null;
    [SerializeField] Text BranchNameTxt = null;
    [SerializeField] Text NickNameTxt = null;
    [SerializeField] Text LevelTxt = null;
    [SerializeField] Text HPTxt = null;
    [SerializeField] Text MPTxt = null;
    [SerializeField] Button CancelBtn = null;
    [SerializeField] Button ConfirmBtn = null;
    [SerializeField] Dropdown CharacterTypeDpn = null;
    [SerializeField] Toggle isCopyTog = null;
    Character characterOnSelect = null;
    List<Character> characters = new List<Character>();
    List<GameObject> MagicBookGoList = new List<GameObject>();
    private void Start()
    {
        OnScene.isPlayMode = false;
        CancelBtn.onClick.AddListener(Cancel);
        ConfirmBtn.onClick.AddListener(Confirm);
        System.IO.DirectoryInfo savefolder = new System.IO.DirectoryInfo(OnGame.CharecterSavePath);
        System.IO.FileInfo[] files = savefolder.GetFiles('*' + OnGame.CharecterFileExtName);
        int p = 0;
        foreach (var v in files)
        {
            if (v.Name[0] == '#') continue;
            Character character = OnScene.LoadCharacter(v.Name.Split('.')[0]);
            GameObject go = Instantiate(CharacterSelectButtonObj, ScrollViewContent);
            go.GetComponent<CharacterSelectButton>().SetUp(this, p++, character);
            characters.Add(character);
        }
        CharacterCreateBtn = Instantiate(CharacterSelectButtonObj, ScrollViewContent);
        CharacterCreateBtn.GetComponent<Text>().text = "+ [创建新的角色]...";
        CharacterCreateBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameObject pnl = Instantiate(CharacterCreatePnlObj, OnScene.manager.PlayModeCanvas);
            pnl.GetComponent<CreateCharecterPanel>().finishBhv = AddNew;
        });
    }
    public void Cancel()
    {
        OnScene.isPlayMode = true;
        Destroy(gameObject);
    }
    public void Confirm()
    {
        if (OnScene.onSelect.tile != null && characterOnSelect != null)
        {
            OnScene.isPlayMode = true;
            characterOnSelect.characterType = (CharacterType)CharacterTypeDpn.value;
            string Copyname = "";
            int i = -1;
            if (isCopyTog.isOn)
            {
                for (i = 1; i <= 1024; i++)
                {
                    Copyname = characterOnSelect.Name + '#' + i.ToString();
                    //先看存档里有没有重名的
                    if (characters.Find((Character c) => { return c.Name == Copyname; }) == null)
                    {
                        bool hasExist = false;
                        //再分别查看各个地图看有没有重名的
                        foreach (var v in MapSet.Creators)
                        {
                            if (v.characters.Find((CharacterScript cs) => { return cs.data.Name == Copyname;}) != null)
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
            }
            OnScene.manager.CreateCharacter(OnScene.onSelect.tile.transform.position,Quaternion.identity, characterOnSelect,i,true);
            Destroy(gameObject);
        }
    }
    public void OnSelect(int i)
    {
        characterOnSelect = characters[i];
        NameTxt.text = "<color=#aa00aa>" + characterOnSelect.Name + "</color>，";
        NickNameTxt.text = "<color=#4400ff>" + characterOnSelect.Nickname + "</color>，";
        BranchNameTxt.text = "<color=#440055>"+ characterOnSelect.BranchName + "</color>，";
        LevelTxt.text = characterOnSelect.BaseData.level.ToString();
        HPTxt.text = characterOnSelect.BaseData.HP.ToString();
        MPTxt.text = characterOnSelect.BaseData.MP.ToString();
        FlushMagicBook(characterOnSelect.magicBook);
    }
    public void AddNew(Character character)
    {
        Destroy(CharacterCreateBtn);
        GameObject go = Instantiate(CharacterSelectButtonObj, ScrollViewContent);
        characters.Add(character);
        go.GetComponent<CharacterSelectButton>().SetUp(this, characters.Count-1, character);
        CharacterCreateBtn = Instantiate(CharacterSelectButtonObj, ScrollViewContent);
        CharacterCreateBtn.GetComponent<Text>().text = "+ [创建新的角色]...";
        CharacterCreateBtn.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameObject pnl = Instantiate(CharacterCreatePnlObj, OnScene.manager.PlayModeCanvas);
            pnl.GetComponent<CreateCharecterPanel>().finishBhv = AddNew;
        });
    }
    public void FlushMagicBook(MagicBook magicBook)
    {
        foreach (GameObject go in MagicBookGoList)
        {
            Destroy(go);
        }
        foreach (Skill skill in magicBook.skills)
        {
            GameObject page = Instantiate(MagicBookPageObj, MagicBookViewContent);
            page.GetComponent<Text>().text = skill.GetRank(out int color) + skill.nameStr;
            //page.GetComponent<Text>().color = OnGame.MagicLib.SkillRankColors[color];
            page.GetComponent<Text>().color = skill.protype.GetColor();
            MagicBookGoList.Add(page);
        }
        
    }
}
