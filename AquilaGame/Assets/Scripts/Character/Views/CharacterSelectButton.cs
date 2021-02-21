using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] Text Txt = null;
    int index = 0;
    CharacterSelectPnl pnl = null;
    public void SetUp(CharacterSelectPnl _pnl,int _index, Character character)
    {
        Txt.text = "[" + character.BranchName + "] " + character.Nickname + ' ' + character.Name;
        index = _index;
        pnl = _pnl;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        pnl.OnSelect(index);
    }
}
