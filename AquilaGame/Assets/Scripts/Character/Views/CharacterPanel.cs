using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Convert;
public class CharacterPanel : MonoBehaviour
{
    [SerializeField] Transform OnPlayCanvasPanel = null;
    [SerializeField] GameObject MagicBookPnl = null;
    [SerializeField] GameObject ItemListPnl = null;
    [SerializeField] BuffPanel buffPnl = null;
    [SerializeField] GameObject MagicBookChildObj = null;
    [SerializeField] GameObject ItemListChildObj = null;
    [SerializeField] Transform MagicBookContentTrans = null;
    [SerializeField] Transform ItemListContentTrans = null;
    [SerializeField] Sprite MagicBookIcon = null;
    [SerializeField] Sprite ItemListIcon = null;
    [SerializeField] SkillInfoWindow skillInfoWindow = null;
    [SerializeField] Button skillAddNewBtn = null;
    [SerializeField] Button itemAddNewBtn = null;
    [SerializeField] Text skillAddNewTxt = null;
    [SerializeField] ItemInfoWindow itemInfoWindow = null;
    [SerializeField] InputField NameTxt = null;
    [SerializeField] InputField OccupationTxt = null;
    [SerializeField] InputField HPTxt = null;
    [SerializeField] InputField MPTxt = null;
    [SerializeField] Button HPHealBtn = null;
    [SerializeField] Button HPDamageBtn = null;
    [SerializeField] Button MPHealBtn = null;
    [SerializeField] Button MPDamageBtn = null;
    [SerializeField] InputField ExpTxt = null;
    [SerializeField] InputField STRTxt = null;
    [SerializeField] InputField DEXTxt = null;
    [SerializeField] InputField CONTxt = null;
    [SerializeField] InputField INTTxt = null;
    [SerializeField] InputField WISTxt = null;
    [SerializeField] InputField CHATxt = null;
    [SerializeField] Button STRIconBtn = null;
    [SerializeField] Button DEXIconBtn = null;
    [SerializeField] Button CONIconBtn = null;
    [SerializeField] Button INTIconBtn = null;
    [SerializeField] Button WISIconBtn = null;
    [SerializeField] Button CHAIconBtn = null;
    [SerializeField] Slider HPSlider = null;
    [SerializeField] Slider MPSlider = null;
    [SerializeField] Slider ExpSlider = null;
    [SerializeField] Button SlideSwithBtn = null;
    [SerializeField] Button SaveBtn = null;
    [SerializeField] Image SlideSwithBtnIcon = null;
    [SerializeField] GameObject EditSkillPnlObj = null;
    [SerializeField] GameObject EditItemPnlObj = null;
    [SerializeField] ActionPointPanel AP_Pnl = null;
    enum SidePanelType { Side_Magicbook, Side_ItemList }
    public Character characterOnSelect = null;
    SidePanelType nowSideType = SidePanelType.Side_Magicbook;
    List<GameObject> Childs = new List<GameObject>();
    
    private void Start()
    {
        SlideSwithBtn.onClick.AddListener(OnClickSwitchBtn);
        STRIconBtn.onClick.AddListener(STRCheck);
        DEXIconBtn.onClick.AddListener(DEXCheck);
        CONIconBtn.onClick.AddListener(CONCheck);
        INTIconBtn.onClick.AddListener(INTCheck);
        WISIconBtn.onClick.AddListener(WISCheck);
        CHAIconBtn.onClick.AddListener(CHACheck);
        HPHealBtn.onClick.AddListener(OnClickHPHealBtn);
        HPDamageBtn.onClick.AddListener(OnClickHPDamageBtn);
        MPHealBtn.onClick.AddListener(OnClickMPHealBtn);
        MPDamageBtn.onClick.AddListener(OnClickMPDamageBtn);
        SaveBtn.onClick.AddListener(() => { SaveData(OnScene.onSelect.character); });
        skillAddNewBtn.onClick.AddListener(OnClickNewSkill);
        itemAddNewBtn.onClick.AddListener(OnClickNewItem);
        ExpTxt.onEndEdit.AddListener(SetEXP);
        STRTxt.onEndEdit.AddListener(GetInputSTR);
        DEXTxt.onEndEdit.AddListener(GetInputDEX);
        CONTxt.onEndEdit.AddListener(GetInputCON);
        INTTxt.onEndEdit.AddListener(GetInputINT);
        WISTxt.onEndEdit.AddListener(GetInputWIS);
        CHATxt.onEndEdit.AddListener(GetInputCHA);
    }
    public void Set(Character character)
    {
        characterOnSelect = character;
        FlushName();
        FlushHP();
        FlushMP();
        FlushAP();
        FlushEXP();
        FlushAttributeData(-1);
        FlushBuff();
        switch (nowSideType)
        {
            case SidePanelType.Side_Magicbook:
                MagicBookPnl.SetActive(true);
                ItemListPnl.SetActive(false);
                ShowMagicBook();
                break;
            case SidePanelType.Side_ItemList:
                MagicBookPnl.SetActive(false);
                ItemListPnl.SetActive(true);
                ShowItemList();
                break;
            default:
                break;
        }
    }

    void SetEXP(string val)
    {
        int i = 0;
        try
        {
            i = ToInt32(val);
        }
        catch (System.Exception)
        {
            OnScene.Report("应该输入一个非负整数！");
        }
        if (i > 0)
        {
            OnScene.onSelect.character.AddEXP(i);
        }
    }

    public void ShowSkillInfo(Skill skill)
    {
        skillInfoWindow.gameObject.SetActive(true);
        skillInfoWindow.SetUp(skill);
    }
    public void HideSkillInfo()
    {
        skillInfoWindow.gameObject.SetActive(false);
    }

    public void ShowItemInfo(Item item)
    {
        itemInfoWindow.gameObject.SetActive(true);
        itemInfoWindow.SetUp(item);
    }
    public void HideItemInfo()
    {
        itemInfoWindow.gameObject.SetActive(false);
    }

    public void ShowMagicBook()
    {
        HideSkillInfo();
        foreach (var v in Childs)
        {
            Destroy(v);
        }
        foreach (var v in characterOnSelect.magicBook.skills)
        {
            GameObject child = Instantiate(MagicBookChildObj, MagicBookContentTrans);
            child.GetComponent<CharacterPnlMagicBookChild>().SetUp(v, this);
            Childs.Add(child);
        }
        skillAddNewTxt.text = "[剩余法术位:" + characterOnSelect.SkillAssignableCount.ToString() + "]";
    }
    public void ShowItemList()
    {
        HideItemInfo();
        foreach (var v in Childs)
        {
            Destroy(v);
        }
        foreach (var v in characterOnSelect.package.items)
        {
            GameObject child = Instantiate(ItemListChildObj, ItemListContentTrans);
            child.GetComponent<CharacterPnlItemListChild>().SetUp(v, this);
            Childs.Add(child);
        }
    }

    public void OnClickSwitchBtn()
    {
        switch (nowSideType)
        {
            case SidePanelType.Side_Magicbook:
                MagicBookPnl.SetActive(false);
                ItemListPnl.SetActive(true);
                ShowItemList();
                SlideSwithBtnIcon.sprite = MagicBookIcon;
                nowSideType = SidePanelType.Side_ItemList;
                break;
            case SidePanelType.Side_ItemList:
                MagicBookPnl.SetActive(true);
                ItemListPnl.SetActive(false);
                ShowMagicBook();
                SlideSwithBtnIcon.sprite = ItemListIcon;
                nowSideType = SidePanelType.Side_Magicbook;
                break;
            default:
                break;
        }
    }
    public void FlushAttributeData(int index)
    {
        switch (index)
        {
            case 0: STRTxt.text = characterOnSelect.BaseData.STR.ToString(); return;
            case 1: DEXTxt.text = characterOnSelect.BaseData.DEX.ToString(); return;
            case 2: CONTxt.text = characterOnSelect.BaseData.CON.ToString(); return;
            case 3: INTTxt.text = characterOnSelect.BaseData.INT.ToString(); return;
            case 4: WISTxt.text = characterOnSelect.BaseData.WIS.ToString(); return;
            case 5: CHATxt.text = characterOnSelect.BaseData.CHA.ToString(); return;
            default:
                STRTxt.text = characterOnSelect.BaseData.STR.ToString();
                DEXTxt.text = characterOnSelect.BaseData.DEX.ToString();
                CONTxt.text = characterOnSelect.BaseData.CON.ToString();
                INTTxt.text = characterOnSelect.BaseData.INT.ToString();
                WISTxt.text = characterOnSelect.BaseData.WIS.ToString();
                CHATxt.text = characterOnSelect.BaseData.CHA.ToString();
                return;
        }
    }

    public void FlushItemList()
    {
        if(nowSideType == SidePanelType.Side_ItemList)
            ShowItemList();
    }

    public void FlushMagicBook()
    {
        if(nowSideType == SidePanelType.Side_Magicbook)
            ShowMagicBook();
    }

    public void FlushHP()
    {
        HPSlider.maxValue = characterOnSelect.BaseData.MaxHP;
        HPSlider.value = characterOnSelect.BaseData.HP > 0 ? characterOnSelect.BaseData.HP : 0;
        HPTxt.text = characterOnSelect.BaseData.HP.ToString() + '/' + characterOnSelect.BaseData.MaxHP;
    }

    public void FlushMP()
    {
        MPSlider.maxValue = characterOnSelect.BaseData.MaxMP;
        MPSlider.value = characterOnSelect.BaseData.MP > 0 ? characterOnSelect.BaseData.MP : 0;
        MPTxt.text = characterOnSelect.BaseData.MP.ToString() + '/' + characterOnSelect.BaseData.MaxMP;
    }

    public void FlushEXP()
    {
        ExpSlider.maxValue = characterOnSelect.BaseData.NextEXP;
        ExpSlider.value = characterOnSelect.BaseData.EXP;
        ExpTxt.text = characterOnSelect.BaseData.EXP.ToString() + '/' + characterOnSelect.BaseData.NextEXP;
    }

    public void FlushName()
    {
        NameTxt.text = characterOnSelect.Nickname + " - " + characterOnSelect.Name;
        OccupationTxt.text = characterOnSelect.BaseData.level.ToString() + "级  " + characterOnSelect.BranchName + '('+ Alignment.GetAlignmentName(characterOnSelect.alignmentType)+')';
    }

    public void FlushBuff()
    {
        buffPnl.Flush(characterOnSelect.buffs);
    }

    public void FlushAP()
    {
        AP_Pnl.SetValue(characterOnSelect.AP);
    }

    public void PreviewAPCost(object obj)
    {
        AP_Pnl.SetPreview(Character.GetAPCost(obj));
    }
    public void PreviewAPCost(float val)
    {
        AP_Pnl.SetPreview(val);
    }

    public void SetSkillRangePreview(Skill skill)
    {

    }

    public void ResetSkillRangePreview()
    {

    }

    public void STRCheck() { CombatPanel pnl = OnScene.manager.OpenCombatPanel(); pnl.SetUp("力量检定", characterOnSelect.Name, "环境", characterOnSelect.BaseData.STR, 0, null, "0", "0", "-10"); }
    public void DEXCheck() { CombatPanel pnl = OnScene.manager.OpenCombatPanel(); pnl.SetUp("敏捷检定", characterOnSelect.Name, "环境", characterOnSelect.BaseData.DEX, 0, null, "0", "0", "-10"); }
    public void CONCheck() { CombatPanel pnl = OnScene.manager.OpenCombatPanel(); pnl.SetUp("体质检定", characterOnSelect.Name, "环境", characterOnSelect.BaseData.CON, 0, null, "0", "0", "-10"); }
    public void INTCheck() { CombatPanel pnl = OnScene.manager.OpenCombatPanel(); pnl.SetUp("智力检定", characterOnSelect.Name, "环境", characterOnSelect.BaseData.INT, 0, null, "0", "0", "-10"); }
    public void WISCheck() { CombatPanel pnl = OnScene.manager.OpenCombatPanel(); pnl.SetUp("感知检定", characterOnSelect.Name, "环境", characterOnSelect.BaseData.WIS, 0, null, "0", "0", "-10"); }
    public void CHACheck() { CombatPanel pnl = OnScene.manager.OpenCombatPanel(); pnl.SetUp("魅力检定", characterOnSelect.Name, "环境", characterOnSelect.BaseData.CHA, 0, null, "0", "0", "-10"); }


    void OnClickHPHealBtn()
    {
        OnScene.onSelect.character.HealRollHP();
    }
    void OnClickHPDamageBtn()
    {
        OnScene.onSelect.character.DamageRollHP();
    }
    void OnClickMPHealBtn()
    {
        OnScene.onSelect.character.HealRollMP();
    }
    void OnClickMPDamageBtn()
    {
        OnScene.onSelect.character.DamageRollMP();
    }

    void OnClickNewSkill()
    {
        if (characterOnSelect.SkillAssignableCount > 0)
        {
            SkillCreatePanel magicPanel = Instantiate(EditSkillPnlObj, OnPlayCanvasPanel).GetComponent<SkillCreatePanel>();
            magicPanel.SetUp(characterOnSelect.OccupationData, characterOnSelect.BaseData.baseAttribute, transform, OnScene.onSelect.character.AddSkill);
        }
    }

    void OnClickNewItem()
    {
        ItemEditPanel itemPanel = Instantiate(EditItemPnlObj, OnPlayCanvasPanel).GetComponent<ItemEditPanel>();
        itemPanel.SetUp(OnScene.onSelect.character.AddItem);
    }

    public void ChangeSkill(Skill old)
    {
        SkillCreatePanel magicPanel = Instantiate(EditSkillPnlObj, OnPlayCanvasPanel).GetComponent<SkillCreatePanel>();
        magicPanel.SetUp(characterOnSelect.OccupationData, characterOnSelect.BaseData.baseAttribute, transform, 
            (Skill newSkill) => {OnScene.onSelect.character.ChangeSkill(old, newSkill); });
    }

    public void EditItem(Item item)
    {
        ItemEditPanel itemPanel = Instantiate(EditItemPnlObj, OnPlayCanvasPanel).GetComponent<ItemEditPanel>();
        itemPanel.SetUp(EditItemCallback, true, item);
    }

    void EditItemCallback(Item item)
    {
        if (item.count <= 0)
        {
            characterOnSelect.package.Remove(item);
        }
        FlushItemList();
    }

    void SaveData(CharacterScript character)
    {
        OnScene.SaveCharacter(character);
    }

    void GetInputSTR(string s) { OnScene.onSelect.character.InputSTR(s); }
    void GetInputDEX(string s) { OnScene.onSelect.character.InputDEX(s); }
    void GetInputCON(string s) { OnScene.onSelect.character.InputCON(s); }
    void GetInputINT(string s) { OnScene.onSelect.character.InputINT(s); }
    void GetInputWIS(string s) { OnScene.onSelect.character.InputWIS(s); }
    void GetInputCHA(string s) { OnScene.onSelect.character.InputCHA(s); }

    void CheckQuickKeyboardInput()
    {
        if (OnScene.isChating || OnScene.onSelect.character == null)
            return;
        if (Input.GetButtonDown("RoundQuickCtrl"))
        {
            if (OnScene.isTimerModeOn)
            {
                OnScene.onSelect.character.OnRoundEnd();
            }
            else
            {
                OnScene.onSelect.character.OnRoundStart();
            }
        }
    }

    private void Update()
    {
        CheckQuickKeyboardInput();
    }
}
