using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanel : MonoBehaviour
{
    public GameObject MapBottomPnl;
    public GameObject CharacterPnl;
    public CharacterPanel CharacterPanelScript;
    public Text NameTxt;
    public Text TypeTxt;
    public InputField MapObjInfoFld;
    private void Awake()
    {
        OnScene.bottomPnl = this;
        OnGame.Log("属性面板建立连接");
    }

    public void SetInfo(string name, string typestr, string info)
    {
        CharacterPnl.SetActive(false);
        MapBottomPnl.SetActive(true);
        NameTxt.text = name;
        TypeTxt.text = typestr;
        MapObjInfoFld.text = info;
    }

    public void SetInfo(CharacterScript character)
    {
        CharacterPnl.SetActive(true);
        MapBottomPnl.SetActive(false);
        character.dataPnl = CharacterPanelScript;
        CharacterPanelScript.Set(character.data);
    }
}
