using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreateCharecterPanel : MonoBehaviour
{
    public delegate void CreateCharacterBhv(Character character);
    [SerializeField] GameObject Page1 = null;
    [SerializeField] GameObject Page2 = null;
    [SerializeField] GameObject Page3 = null;
    [SerializeField] GameObject Page4 = null;
    [SerializeField] GameObject Page5 = null;
    [SerializeField] GameObject MagicUIObj = null;
    [SerializeField] InputField NameFld = null;
    [SerializeField] InputField NickNameFld = null;
    [SerializeField] InputField[] AttrabutesFld = new InputField[6];
    [SerializeField] InputField ViewLevelFld = null;
    [SerializeField] InputField ViewHPFld = null;
    [SerializeField] InputField ViewMPFld = null;
    [SerializeField] InputField ViewMagicSlotsFld = null;
    [SerializeField] InputField ViewMagicForceFld = null;
    [SerializeField] InputField LogFld = null;
    [SerializeField] Dropdown OccupationDpn = null;
    [SerializeField] Dropdown BranchDpn = null;
    [SerializeField] Button[] AlignmentBtn = new Button[9];
    [SerializeField] Button StartCreateDataBtn = null;
    [SerializeField] Button RestartCreateDataBtn = null;
    [SerializeField] Button MagicBookBtn = null;
    [SerializeField] Button Next1to2Btn = null;
    [SerializeField] Button Next2to3Btn = null;
    [SerializeField] Button Next3to4Btn = null;
    [SerializeField] Button Next4to5Btn = null;
    [SerializeField] Button Back2to1Btn = null;
    [SerializeField] Button Back3to2Btn = null;
    [SerializeField] Button Back4to3Btn = null;
    [SerializeField] Button Back5to4Btn = null;
    [SerializeField] Button FinishBtn = null;
    [SerializeField] Text OccupationNameTxt = null;
    [SerializeField] Text OccupationIntroTxt = null;
    [SerializeField] Text BranchIntroTxt = null;
    [SerializeField] Text BranchDataTxt = null;
    [SerializeField] Text AlignmentTxt = null;
    [SerializeField] Text BranchNameTxt = null;
    [SerializeField] Text[] AttributeDatasTxt = new Text[6];
    [SerializeField] Text CreateFinishPreviewTxt = null;
    [SerializeField] Color NormalAttributeDataColor;
    [SerializeField] Color HighlightAttributeDataColor;
    [SerializeField] MagicBookInCreate MagicBookPanel = null;
    public CreateCharacterBhv finishBhv = null;
    Character character = null;
    //-------Data----------
    Occupation occupation = new Occupation(OccupationType.Mage);
    BirthGroupType birthType = BirthGroupType.KingdomMale;
    AlignmentType alignmentType = AlignmentType.TrueNeutral;
    Attribute6 baseAttribute = null;
    Attribute6 exAttribute = Occupation.GetOcccupationBranchAttribute(OccupationType.Mage);
    AttributeData attributeData = null;
    int MagicSpellCount = 0;
    int MagicSpellForce = 0;
    //---------------------
    bool isAlignmentConfirm = false;


    void OnClickNext1to2Btn()
    {
        if (isAlignmentConfirm && OnGame.CheckFileName(NameFld.text) && (!string.IsNullOrWhiteSpace(NickNameFld.text)))
        {
            Page1.SetActive(false);
            Page2.SetActive(true);
        }
    }
    void OnClickNext2to3Btn()
    {
        if (baseAttribute != null)
        {
            Page2.SetActive(false);
            Page3.SetActive(true);
            FlushCharacterPreview();
        }
    }
    void OnClickNext3to4Btn()
    {
        if (true)
        {
            CloseCharacterPreview();
            MagicBookPanel.SetUp(occupation, attributeData.baseAttribute, MagicSpellCount);
            Page3.SetActive(false);
            Page4.SetActive(true);
        }
    }
    void OnClickNext4to5Btn()
    {
        if (true)
        {
            CreateFinishPreviewTxt.text = "现在，以伟大的<color=#230047>\n" + BranchNameTxt.text + " " + NickNameFld.text + " " + NameFld.text + "</color>\n之名——";
            Page4.SetActive(false);
            Page5.SetActive(true);
        }
    }
    void OnClickBack2to1Btn() { Page2.SetActive(false); Page1.SetActive(true); }
    void OnClickBack3to2Btn()
    {
        CloseCharacterPreview();
        Page3.SetActive(false);
        Page2.SetActive(true);
    }
    void OnClickBack4to3Btn()
    {
        FlushCharacterPreview();
        Page4.SetActive(false);
        Page3.SetActive(true);
    }
    void OnClickBack5to4Btn() { Page5.SetActive(false); Page4.SetActive(true); }

    void OnClickAlignmentBtn0() { OnSetAlignment((AlignmentType)0); }
    void OnClickAlignmentBtn1() { OnSetAlignment((AlignmentType)1); }
    void OnClickAlignmentBtn2() { OnSetAlignment((AlignmentType)2); }
    void OnClickAlignmentBtn3() { OnSetAlignment((AlignmentType)3); }
    void OnClickAlignmentBtn4() { OnSetAlignment((AlignmentType)4); }
    void OnClickAlignmentBtn5() { OnSetAlignment((AlignmentType)5); }
    void OnClickAlignmentBtn6() { OnSetAlignment((AlignmentType)6); }
    void OnClickAlignmentBtn7() { OnSetAlignment((AlignmentType)7); }
    void OnClickAlignmentBtn8() { OnSetAlignment((AlignmentType)8); }

    private void Start()
    {
        Next1to2Btn.onClick.AddListener(OnClickNext1to2Btn);
        Next2to3Btn.onClick.AddListener(OnClickNext2to3Btn);
        Next3to4Btn.onClick.AddListener(OnClickNext3to4Btn);
        Next4to5Btn.onClick.AddListener(OnClickNext4to5Btn);
        Back2to1Btn.onClick.AddListener(OnClickBack2to1Btn);
        Back3to2Btn.onClick.AddListener(OnClickBack3to2Btn);
        Back4to3Btn.onClick.AddListener(OnClickBack4to3Btn);
        Back5to4Btn.onClick.AddListener(OnClickBack5to4Btn);
        AlignmentBtn[0].onClick.AddListener(OnClickAlignmentBtn0);
        AlignmentBtn[1].onClick.AddListener(OnClickAlignmentBtn1);
        AlignmentBtn[2].onClick.AddListener(OnClickAlignmentBtn2);
        AlignmentBtn[3].onClick.AddListener(OnClickAlignmentBtn3);
        AlignmentBtn[4].onClick.AddListener(OnClickAlignmentBtn4);
        AlignmentBtn[5].onClick.AddListener(OnClickAlignmentBtn5);
        AlignmentBtn[6].onClick.AddListener(OnClickAlignmentBtn6);
        AlignmentBtn[7].onClick.AddListener(OnClickAlignmentBtn7);
        AlignmentBtn[8].onClick.AddListener(OnClickAlignmentBtn8);
        foreach (InputField fld in AttrabutesFld)
        {
            fld.onEndEdit.AddListener(GMFlushBaseAttribute);
        }
        ViewHPFld.onEndEdit.AddListener(GMFlushExAttribute); 
        ViewMPFld.onEndEdit.AddListener(GMFlushExAttribute);
        ViewLevelFld.onEndEdit.AddListener(GMFlushExAttribute);
        ViewMagicSlotsFld.onEndEdit.AddListener(GMFlushExAttribute);
        ViewMagicForceFld.onEndEdit.AddListener(GMFlushExAttribute);
        StartCreateDataBtn.onClick.AddListener(OnClickStartCreateDataBtn);
        RestartCreateDataBtn.onClick.AddListener(OnClickRestartCreateDataBtn);
        OccupationDpn.onValueChanged.AddListener(OnSelectOccupation);
        BranchDpn.onValueChanged.AddListener(OnSelectBranch);
        MagicBookBtn.onClick.AddListener(OnClickMagicBookBtn);
        FinishBtn.onClick.AddListener(OnClickConfirmBtn);
    }

    void OnSelectOccupation(int slc)
    {
        OccupationType type = (OccupationType)slc;
        occupation = new Occupation(type);
        OccupationNameTxt.text = Occupation.GetOcccupationName(type);
        OccupationIntroTxt.text = Occupation.GetOcccupationInfo(type);
        exAttribute = Occupation.GetOcccupationBranchAttribute(type);
        if (baseAttribute != null)
        {
            attributeData = new AttributeData(baseAttribute + exAttribute, occupation);
            MagicSpellCount = Occupation.GetOcccupationMaxSkillCount(occupation.type, attributeData.baseAttribute);
            MagicSpellForce = Occupation.GetOcccupationMaxSkillPower(occupation.type, attributeData.baseAttribute);
            FlushAttributeData();
        }
        BranchIntroTxt.text = Occupation.GetOcccupationBranchIntro(type);
        occupation.branch = BranchDpn.value;
        BranchDataTxt.text = Occupation.GetOcccupationBranchData(type, occupation.branch);
        AlignmentType[] alignmentTypes = Occupation.GetOccupationAlignmentList(type);
        int i = 0;
        foreach (var v in alignmentTypes)
        {
            for (; i < (int)v; i++)
            {
                AlignmentBtn[i].interactable = false;
            }
            AlignmentBtn[i++].interactable = true;
        }
        while (i < 9)
        {
            AlignmentBtn[i++].interactable = false;
        }
        isAlignmentConfirm = false;
        AlignmentTxt.text = "未选择";
        MagicBookPanel.Clear();
    }

    void OnSelectBranch(int slc)
    {
        occupation.branch = BranchDpn.value;
        BranchDataTxt.text = Occupation.GetOcccupationBranchData(occupation.type, occupation.branch);
        if (isAlignmentConfirm)
        {
            BranchNameTxt.text = Alignment.GetAlignOccupationName(occupation.type, alignmentType, occupation.branch);
        }
        if (occupation.type == OccupationType.Warrior && baseAttribute != null)
        {
            attributeData = new AttributeData(baseAttribute + exAttribute, occupation);
            MagicSpellCount = Occupation.GetOcccupationMaxSkillCount(occupation.type, attributeData.baseAttribute);
            MagicSpellForce = Occupation.GetOcccupationMaxSkillPower(occupation.type, attributeData.baseAttribute);
        }
        MagicBookPanel.Clear();
    }

    public void FlushCharacterPreview()
    {
        OnGame.characterPreviewer.gameObject.SetActive(true);
        OnGame.characterPreviewer.SetCam(birthType, occupation.type);
    }
    void CloseCharacterPreview()
    {
        OnGame.characterPreviewer.gameObject.SetActive(false);
    }
    public void SetBirth(int i)
    {
        birthType = (BirthGroupType)i;
    }
    public void SetPreviewRot(float val)
    {
        OnGame.characterPreviewer.SetRot(1.0f-val);
    }
    void OnSetAlignment(AlignmentType type)
    {
        AlignmentTxt.text = Alignment.GetAlignmentName(type);
        alignmentType = type;
        BranchNameTxt.text = Alignment.GetAlignOccupationName(occupation.type, alignmentType, occupation.branch);
        isAlignmentConfirm = true;
    }

    void OnClickStartCreateDataBtn()
    {
        if (baseAttribute == null)
            BuildAttributeData();
    }
    void OnClickRestartCreateDataBtn()
    {
        if (baseAttribute != null)
        {
            BuildAttributeData();
            MagicBookPanel.Clear();
        }
    }
    void BuildAttributeData()
    {
        Dice D6 = new Dice(6);
        baseAttribute = new Attribute6(
            D6.roll(3) + 3,
            D6.roll(3) + 3,
            D6.roll(3) + 3,
            D6.roll(3) + 3,
            D6.roll(3) + 3,
            D6.roll(3) + 3);
        attributeData = new AttributeData(baseAttribute + exAttribute, occupation);
        MagicSpellCount = Occupation.GetOcccupationMaxSkillCount(occupation.type, attributeData.baseAttribute);
        MagicSpellForce = Occupation.GetOcccupationMaxSkillPower(occupation.type, attributeData.baseAttribute);
        FlushAttributeData();
    }

    void GMFlushBaseAttribute(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return;
        try
        {
            baseAttribute = new Attribute6(
                System.Convert.ToInt32(AttrabutesFld[0].text) - exAttribute._str,
                System.Convert.ToInt32(AttrabutesFld[1].text) - exAttribute._dex,
                System.Convert.ToInt32(AttrabutesFld[2].text) - exAttribute._con,
                System.Convert.ToInt32(AttrabutesFld[3].text) - exAttribute._int,
                System.Convert.ToInt32(AttrabutesFld[4].text) - exAttribute._wis,
                System.Convert.ToInt32(AttrabutesFld[5].text) - exAttribute._cha
                );
            if (occupation.type == OccupationType.Warrior && occupation.branch == 0)
                baseAttribute._con -= 2;
        }
        catch (System.Exception e)
        {
            return;
        }
        attributeData = new AttributeData(baseAttribute + exAttribute, occupation);
        MagicSpellCount = Occupation.GetOcccupationMaxSkillCount(occupation.type, attributeData.baseAttribute);
        MagicSpellForce = Occupation.GetOcccupationMaxSkillPower(occupation.type, attributeData.baseAttribute);
        FlushAttributeData();
        MagicBookPanel.Clear();
    }

    void GMFlushExAttribute(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return;
        try
        {
            attributeData.MaxHP = System.Convert.ToInt32(ViewHPFld.text);
            attributeData.MaxMP = System.Convert.ToInt32(ViewMPFld.text);
            attributeData.level = System.Convert.ToInt32(ViewLevelFld.text);
            int temp = System.Convert.ToInt32(ViewMagicSlotsFld.text);
            if (temp != MagicSpellCount)
            {
                MagicSpellCount = temp;
                MagicBookPanel.Clear();
            }
            temp = System.Convert.ToInt32(ViewMagicForceFld.text);
            if (temp != MagicSpellForce)
            {
                MagicSpellForce = temp;
                MagicBookPanel.Clear();
            }
        }
        catch (System.Exception e)
        {
            return;
        }
        FlushAttributeData();
    }

    void FlushAttributeData()
    {
        AttributeDatasTxt[0].fontStyle = exAttribute._str == 0 ? FontStyle.Normal : FontStyle.Bold;
        AttributeDatasTxt[1].fontStyle = exAttribute._dex == 0 ? FontStyle.Normal : FontStyle.Bold;
        AttributeDatasTxt[2].fontStyle = exAttribute._con == 0 ? FontStyle.Normal : FontStyle.Bold;
        AttributeDatasTxt[3].fontStyle = exAttribute._int == 0 ? FontStyle.Normal : FontStyle.Bold;
        AttributeDatasTxt[4].fontStyle = exAttribute._wis == 0 ? FontStyle.Normal : FontStyle.Bold;
        AttributeDatasTxt[5].fontStyle = exAttribute._cha == 0 ? FontStyle.Normal : FontStyle.Bold;
        AttributeDatasTxt[0].color = exAttribute._str == 0 ? NormalAttributeDataColor : HighlightAttributeDataColor;
        AttributeDatasTxt[1].color = exAttribute._dex == 0 ? NormalAttributeDataColor : HighlightAttributeDataColor;
        AttributeDatasTxt[2].color = exAttribute._con == 0 ? NormalAttributeDataColor : HighlightAttributeDataColor;
        AttributeDatasTxt[3].color = exAttribute._int == 0 ? NormalAttributeDataColor : HighlightAttributeDataColor;
        AttributeDatasTxt[4].color = exAttribute._wis == 0 ? NormalAttributeDataColor : HighlightAttributeDataColor;
        AttributeDatasTxt[5].color = exAttribute._cha == 0 ? NormalAttributeDataColor : HighlightAttributeDataColor;
        AttrabutesFld[0].text = attributeData.STR.ToString();
        AttrabutesFld[1].text = attributeData.DEX.ToString();
        AttrabutesFld[2].text = attributeData.CON.ToString();
        AttrabutesFld[3].text = attributeData.INT.ToString();
        AttrabutesFld[4].text = attributeData.WIS.ToString();
        AttrabutesFld[5].text = attributeData.CHA.ToString();
        ViewHPFld.text = attributeData.MaxHP.ToString();
        ViewMPFld.text = attributeData.MaxMP.ToString();
        ViewMagicSlotsFld.text = MagicSpellCount.ToString();
        ViewMagicForceFld.text = MagicSpellForce.ToString();
    }
    void OnClickMagicBookBtn()
    {
        if (MagicBookPanel.isFilled)
            return;
        SkillCreatePanel magicPanel = Instantiate(MagicUIObj, transform).GetComponent<SkillCreatePanel>();
        magicPanel.SetUp(occupation,attributeData.baseAttribute, transform, AddSkill,MagicSpellForce);
    }
    void AddSkill(Skill skill)
    {
        if (MagicBookPanel.isFilled)
            return;
        if (MagicBookPanel.Contains(skill))
            return;
        MagicBookPanel.Add(skill);
    }

    void OnClickConfirmBtn()
    {
        SaveData();
        finishBhv?.Invoke(character);
        Destroy(gameObject);
    }

    void SaveData()
    {
        string filename = OnGame.CharecterSavePath + NameFld.text + OnGame.CharecterFileExtName;
        MagicBook magicBook = new MagicBook();
        foreach (var v in MagicBookPanel.List)
        {
            magicBook.Add(v.skill);
        }
        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename))
        {
            character = new Character(NameFld.text, LogFld.text, NickNameFld.text, BranchNameTxt.text,
                occupation,birthType, attributeData, alignmentType, magicBook, new ItemList());
            character.Save(sw);
        }
    }
}
