using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPanel : MonoBehaviour
{
    public enum UserPanelMode {Empty,Tile,MapObject,Character,SummonObj}
    public enum ButtonType
    {
        TileAddCharacter,
        TileChangeInfo,
        MapObjRemove,
        MapObjChangeInfo,
        ChestEdit,
        CharacterRemove,
        CharacterTranslate,
        CharacterTalk,
        CharacterAttack,
        CharacterStartRound,
        CharacterEndRound,
        CharacterUpgrade,
        CharacterSneak,
        CharacterWarp
            
    }
    [SerializeField] UserPanelBtn[] btns = new UserPanelBtn[16];
    int nowbtn = 0;
    [HideInInspector]public UserPanelMode mode = UserPanelMode.Empty;
    [Header("选择图块")]
    [SerializeField] List<Sprite> tileSpritesLst = new List<Sprite>();
    private void Awake()
    {
        OnScene.userPanel = this;
        OnGame.Log("用户按钮区就位");
    }
    void SetBtnDisable(int i)
    {
        if(i < nowbtn)
            btns[i].GetComponent<UnityEngine.UI.Button>().interactable = false;
    }
    void SetBtnEnable(int i)
    {
        if (i < nowbtn)
            btns[i].GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    void AddBtn(ButtonType buttonType, Color color, UserPanelBtn.UserPanelBtnBhv bhvClick, UserPanelBtn.UserPanelBtnBhv bhvEnter = null, UserPanelBtn.UserPanelBtnBhv bhvExit = null)
    {
        if (nowbtn >= 16)
            return;
        btns[nowbtn].gameObject.SetActive(true);
        btns[nowbtn].SetUp(tileSpritesLst[(int)buttonType], color, bhvClick, bhvEnter, bhvExit);
        nowbtn++;
    }

    void ClearBtns()
    {
        for (int i = 0; i < nowbtn; i++)
            btns[i].gameObject.SetActive(false);
        nowbtn = 0;
    }

    public void SetSelect(StaticBlock sblock)
    {
        ClearBtns();
        AddBtn(ButtonType.TileAddCharacter, Color.white, OnScene.manager.OpenSelectCharacterPanel);
        mode = UserPanelMode.Tile;
    }
    public void SetSelect(MapObject obj)
    {
        ClearBtns();
        AddBtn(ButtonType.MapObjRemove,Color.white,obj.Destroy);
        mode = UserPanelMode.MapObject;

    }
    public void SetSelect(SummonObjectBase summonObject)
    {
        ClearBtns();
        AddBtn(
            ButtonType.CharacterEndRound,
            Color.white,
            summonObject.OnRoundStart
            );
        AddBtn(
        ButtonType.CharacterRemove,
        Color.white,
        summonObject.RemoveThis
        );
        mode = UserPanelMode.SummonObj;
    }
    public void SetSelect(CharacterScript character)
    {
        CharacterScript cs = OnScene.onSelect.character;
        ClearBtns();
        AddBtn(
            ButtonType.CharacterAttack,
            Color.white,
            cs.OnClickAttackBtn,
            cs.StartAttackRangePreview,
            cs.EndAttackRangePreview
            );
        AddBtn(
            ButtonType.CharacterRemove,
            Color.white,
            cs.RemoveThis
            );
        AddBtn(
            ButtonType.CharacterStartRound,
            Color.white,
            cs.OnRoundStart
            );
        AddBtn(
            ButtonType.CharacterEndRound,
            Color.white,
            cs.OnRoundEnd
            );
        AddBtn(
            ButtonType.CharacterSneak,
            Color.white,
            cs.Sneak
            );
        //换阵营时候还应该更新小地图，这个以后再改
        AddBtn(
            ButtonType.TileAddCharacter,
            Alignment.ColorList[0],
            () =>{ cs.data.characterType = CharacterType.Enemy;cs.canvas.SetDisplay(); }
            );
        AddBtn(
            ButtonType.TileAddCharacter,
            Alignment.ColorList[1],
            () => { cs.data.characterType = CharacterType.Neutral; cs.canvas.SetDisplay(); }
            );
        AddBtn(
            ButtonType.TileAddCharacter,
            Alignment.ColorList[2],
            () => { cs.data.characterType = CharacterType.Friend; cs.canvas.SetDisplay(); }
            );
        AddBtn(
            ButtonType.CharacterWarp,
            Color.white,
            cs.StartWarp
            );
        mode = UserPanelMode.Character;
    }
    public void SetSelectNull()
    {
        ClearBtns();
        mode = UserPanelMode.Empty;
    }
}
