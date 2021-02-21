using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Convert;
public class CombatPanel : MonoBehaviour
{
    [Header("对抗方1")]
    [SerializeField] Text C1NameTxt = null;
    [SerializeField] Text C1Result = null;
    [SerializeField] InputField C1BaseFld = null;
    [SerializeField] InputField C1AddFld = null;
    [SerializeField] Image C1Image = null;
    int C1base = 0;
    int C1Add = 0;
    int C1value = 0;
    Dice C1dice = null;
    [Header("对抗方2")]
    [SerializeField] Text C2NameTxt = null;
    [SerializeField] Text C2Result = null;
    [SerializeField] InputField C2BaseFld = null;
    [SerializeField] InputField C2AddFld = null;
    [SerializeField] Image C2Image = null;
    int C2base = 0;
    int C2Add = 0;
    int C2value = 0;
    Dice C2dice = null;
    [Header("结果和调整")]
    [SerializeField] Sprite WinSprite = null;
    [SerializeField] Sprite LoseSprite = null;
    [SerializeField] Button StartButton = null;
    [SerializeField] Text TitleTxt = null;
    [SerializeField] Text ResultTxt = null;
    int result = 0;
    [SerializeField] InputField AdjustField = null;
    [SerializeField] Button ConfirmBtn = null;
    [SerializeField] float DeltaTime = 0.2f;
    float flushtime = 0.0f;
    bool isMultyMode = false;
    float adjust = 0.0f;
    bool isRunning = false;
    public delegate void AfterCombatBhv(int res);
    AfterCombatBhv ParentBhv = null;
    private void Start()
    {
        StartButton.onClick.AddListener(OnClickStart);
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
    }
    public void SetUp(string title, string name1, string name2, int base1, int base2, AfterCombatBhv afterCombat = null, string add1 = "0", string add2 = "0", string _adjust = "0")
    {
        TitleTxt.text = title;
        C1NameTxt.text = name1;
        C2NameTxt.text = name2;
        C1BaseFld.text = base1.ToString();
        C2BaseFld.text = base2.ToString();
        C1AddFld.text = add1;
        C2AddFld.text = add2;
        AdjustField.text = _adjust;
        ParentBhv = afterCombat;
        ConfirmBtn.gameObject.SetActive(false);
    }
    void OnClickStart()
    {
        if (isRunning)
        {
            ConfirmBtn.gameObject.SetActive(true);
            C1Image.gameObject.SetActive(true);
            C2Image.gameObject.SetActive(true);
            C1Image.sprite = result > 0 ? WinSprite : LoseSprite;
            C2Image.sprite = result <= 0 ? WinSprite : LoseSprite;
            isRunning = false;
            Sound.StopDice();
            return;
        }
        try
        {
            //核算基准值
            C1base = ToInt32(C1BaseFld.text);
            C2base = ToInt32(C2BaseFld.text);
            //核算加值
            if (C1AddFld.text.Contains("%"))
            {
                C1Add = (int)(C1base * (0.0f + ToSingle(C1AddFld.text.Substring(0, C1AddFld.text.Length - 1)) / 100.0f));
            }
            else
            {
                C1Add = ToInt32(C1AddFld.text);
            }
            if (C2AddFld.text.Contains("%"))
            {
                C2Add = (int)(C2base * (0.0f + ToSingle(C2AddFld.text.Substring(0, C2AddFld.text.Length - 1)) / 100.0f));
            }
            else
            {
                C2Add = ToInt32(C2AddFld.text);
            }
            if (AdjustField.text.Contains("%"))
            {
                isMultyMode = true;
                adjust = 1.0f + ToSingle(AdjustField.text.Substring(0, AdjustField.text.Length - 1)) / 100.0f;
            }
            else
            {
                isMultyMode = false;
                adjust = ToInt32(AdjustField.text);
            }
            //核算总值
            C1value = C1base + C1Add;
            C2value = C2base + C2Add;
            C1value = C1value < 0 ? 0 : C1value;
            C2value = C2value < 0 ? 0 : C2value;
            C1dice = new Dice(C1value);
            C2dice = new Dice(C2value);
            flushtime = Time.time;
            isRunning = true;
            Sound.StartDice();
            ConfirmBtn.gameObject.SetActive(false);
            C1Image.gameObject.SetActive(false);
            C2Image.gameObject.SetActive(false);
        }
        catch (System.Exception)
        {
            TitleTxt.text = "遇到错误，可能是输入不正确。";
        }
    }

    void FlushResult()
    {
        C1value = C1dice.roll();
        C2value = C2dice.roll();
        C1Result.text = C1value.ToString();
        C2Result.text = C2value.ToString();
        result = C1value - C2value;
        if (isMultyMode)
        {
            result = (int)(result * adjust);
        }
        else
        {
            result = (int)(result + adjust);
        }
        ResultTxt.text = result.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            if (Time.time > flushtime)
            {
                FlushResult();
                flushtime += DeltaTime;
            }
        }
    }
    void OnClickConfirm()
    {
        ParentBhv?.Invoke(result);
        Destroy(gameObject);
    }
}
