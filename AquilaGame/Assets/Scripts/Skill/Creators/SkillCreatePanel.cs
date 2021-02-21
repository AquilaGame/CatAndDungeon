using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SkillCreatePanel : MonoBehaviour, IPointerExitHandler
{
    public delegate void CreateSkillBhv(Skill skill);
    [SerializeField] Button ExitBtn;
    [SerializeField] Vector3 trans;
    [SerializeField] Transform[] EleSeats = new Transform[6];
    [SerializeField] Transform[] FldSeats = new Transform[6];
    [SerializeField] SkillCreateUnitListSeat[] Seats = new SkillCreateUnitListSeat[6];
    [SerializeField] SkillCreateUnitListSeat ExSeat = null;
    [SerializeField] GameObject OnPanelUnitObj = null;
    [SerializeField] GameObject PointerUnitObj = null;
    GameObject PointerUnit = null;
    List<SkillUnitOnPanel> OnPanelUnits = new List<SkillUnitOnPanel>();
    [HideInInspector] public bool isPointerObjActive = false;
    Transform ParentPanelTrans = null;
    CreateSkillBhv finishBhv = null;
    List<ElementType> eleList;
    List<FieldType> fieldList;
    RectTransform Rtrans = null;
    [HideInInspector]public Color pointerColor = Color.white;
    [HideInInspector] public Sprite pointerSprite = null;
    object pointerUnit = null;
    [HideInInspector] public SkillCreateUnitListSeat ActiveSeat = null;
    public float FadeSpeed = 2.0f;
    float alpha = 1.0f;
    bool isAlphaAdd = true;
    int UnlockSeatCount = 0;
    ElementType nowEle = ElementType.Air;
    FieldType nowFld = FieldType.Energy;
    List<int> EleSlots = new List<int>();
    List<int> FldSlots = new List<int>();
    Occupation occ = null;
    [Header("生成预览")]
    [SerializeField] SkillCreatePreview preview = null;
    [SerializeField] SkillInfoWindow infoWindow = null;

    Skill skillOnEdit = null;
    public void SetUp(Occupation occupation, Attribute6 attribute, Transform parentTrans, CreateSkillBhv bhv,int exCount = -1)
    {
        Rtrans = GetComponent<RectTransform>();
        occ = occupation;
        eleList = Occupation.GetOcccupationElementList(occupation);
        fieldList = Occupation.GetOcccupationFieldList(occupation.type);
        ParentPanelTrans = parentTrans;
        finishBhv = bhv;
        for (int i = 0; i < 6; i++)
        {
            GameObject go = Instantiate(OnPanelUnitObj, EleSeats[i]);
            if (i < eleList.Count)
            {
                OnPanelUnits.Add(go.GetComponent<SkillUnitOnPanel>());
                go.GetComponent<SkillUnitOnPanel>().SetActive(eleList[i], this);
            }
        }
        for (int i = 0; i < 6; i++)
        {
            GameObject go = Instantiate(OnPanelUnitObj, FldSeats[i]);
            if (i < fieldList.Count)
            {
                OnPanelUnits.Add(go.GetComponent<SkillUnitOnPanel>());
                go.GetComponent<SkillUnitOnPanel>().SetActive(fieldList[i], this);
            }
        }
        UnlockSeatCount = exCount > 0 ? exCount : Occupation.GetOcccupationMaxSkillPower(occupation.type, attribute);
        UnlockSeatCount = UnlockSeatCount > 6 ? 6 : UnlockSeatCount;
        for (int i = 0; i < UnlockSeatCount; i++)
        {
            Seats[i].Unlock();
        }
        ExitBtn.onClick.AddListener(() => { Destroy(gameObject); });
    }

    void FlushOnPanelUnits()
    {

        if (isAlphaAdd)
        {
            alpha += FadeSpeed * Time.deltaTime;
            if (alpha > 1.0f)
            {
                alpha = 1.0f;
                isAlphaAdd = false;
            }
        }
        else
        {
            alpha -= FadeSpeed * Time.deltaTime;
            if (alpha < 0.3f)
            {
                alpha = 0.3f;
                isAlphaAdd = true;
            }
        }

        foreach (var v in OnPanelUnits)
        {
            if (v.isActive)
            {
                if (v.isHighLight)
                {
                    v.SetColorAlpha(1.0f);
                }
                else
                {
                    v.SetColorAlpha(alpha);
                }
            }
        }
    }

    void FlushPreview()
    {
        if (EleSlots.Count != 0 && FldSlots.Count != 0)
        {
            preview.gameObject.SetActive(true);
            preview.SetUp(Skill.ProtypeList[(int)nowEle, (int)nowFld].GetColor(), OnGame.MagicLib.FldSprites[(int)nowFld]);
            int ExForce = Occupation.GetBranchAddForce(occ, nowEle, nowFld);
            if (ExForce == 1)
            {
                ExSeat.SetFill(OnGame.MagicLib.EleSprites[(int)nowEle], OnGame.MagicLib.EleColors[(int)nowEle]);
                skillOnEdit = new Skill(Skill.ProtypeList[(int)nowEle, (int)nowFld], EleSlots.Count + 1, FldSlots.Count);
            }
            else if (ExForce == 2)
            {
                ExSeat.SetFill(OnGame.MagicLib.FldSprites[(int)nowFld], OnGame.MagicLib.FldColors[(int)nowFld]);
                skillOnEdit = new Skill(Skill.ProtypeList[(int)nowEle, (int)nowFld], EleSlots.Count, FldSlots.Count + 1);
            }
            else
            {
                ExSeat.ResetFill();
                skillOnEdit = new Skill(Skill.ProtypeList[(int)nowEle, (int)nowFld], EleSlots.Count, FldSlots.Count);
            }
        }
        else
        {
            preview.gameObject.SetActive(false);
        }
    }

    public void SetPreviewPanel(bool isSet)
    {
        if (isSet)
        {
            infoWindow.gameObject.SetActive(true);
            infoWindow.SetUp(skillOnEdit);
        }
        else
        {
            infoWindow.gameObject.SetActive(false);
        }
    }

    public void SetMouseDown(object obj, Color color, Sprite sprite)
    {
        pointerColor = color;
        pointerSprite = sprite;
        pointerUnit = obj;
        isPointerObjActive = true;
        PointerUnit = Instantiate(PointerUnitObj, transform);
        PointerUnit.GetComponent<SkillUnitPointer>().SetUp(color, sprite);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ActiveSeat != null)
        {
            int loc = ActiveSeat.number;
            if (pointerUnit.GetType() == nowEle.GetType())//是元素
            {
                nowEle = (ElementType)pointerUnit;
                if (FldSlots.Contains(loc))
                {
                    FldSlots.Remove(loc);
                }
                if (!EleSlots.Contains(loc))
                {
                    EleSlots.Add(loc);
                }
                foreach (int i in EleSlots)
                {
                    Seats[i].SetFill();
                }
            }
            else
            {
                nowFld = (FieldType)pointerUnit;
                if (EleSlots.Contains(loc))
                {
                    EleSlots.Remove(loc);
                }
                if (!FldSlots.Contains(loc))
                {
                    FldSlots.Add(loc);
                }
                foreach (int i in FldSlots)
                {
                    Seats[i].SetFill();
                }
            }
            FlushPreview();
        }
        CancelPointerEle();
    }

    public void OnClickSeatToClear(int loc)
    {
        if (EleSlots.Contains(loc))
            EleSlots.Remove(loc);
        else if (FldSlots.Contains(loc))
            FldSlots.Remove(loc);
        Seats[loc].ResetFill();
        FlushPreview();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        CancelPointerEle();
    }

    void CancelPointerEle()
    {
        isPointerObjActive = false;
        if (PointerUnit != null)
            Destroy(PointerUnit);
    }

    public void OnClickSubmit()
    {
        finishBhv(skillOnEdit);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (isPointerObjActive)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(Rtrans, Input.mousePosition, Camera.main, out Vector2 MousePos);
            PointerUnit.GetComponent<RectTransform>().anchoredPosition = MousePos;
        }
        FlushOnPanelUnits();
    }
}
