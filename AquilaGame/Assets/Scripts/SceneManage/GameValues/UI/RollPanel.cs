using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Convert;
public class RollPanel : MonoBehaviour
{
    [SerializeField] Text TitleTxt = null;
    [SerializeField] Text ResultTxt = null;
    [SerializeField] InputField FacesFld = null;
    [SerializeField] InputField RoundFld = null;
    [SerializeField] InputField AdjustPerRollFld = null;
    [SerializeField] InputField AdjustFld = null;
    [SerializeField] Button StartBtn = null;
    [SerializeField] Button ConfirmBtn = null;
    [SerializeField] float DeltaTime = 0.2f;
    int faces = 0;
    int round = 0;
    float adjustPerRoll = 0.0f;
    float adjust = 0.0f;
    Dice dice = null;
    int result = 0;
    float flushtime = 0.0f;
    bool isPerRollMulty = false;
    bool isWholeMulty = false;
    bool isRunning = false;
    public delegate void AfterRollBhv(int res);
    AfterRollBhv ParentBhv = null;
    private void Start()
    {
        StartBtn.onClick.AddListener(OnClickStart);
        ConfirmBtn.onClick.AddListener(OnClickConfirm);
    }
    public void SetUp(string title, AfterRollBhv afterCombat = null, int _faces = 4, int _round = 1, string adjustStr1 = "0", string adjustStr2 = "0")
    {
        TitleTxt.text = title;
        FacesFld.text = _faces.ToString();
        RoundFld.text = _round.ToString();
        AdjustPerRollFld.text = adjustStr1;
        AdjustFld.text = adjustStr2;
        ParentBhv = afterCombat;
        ConfirmBtn.gameObject.SetActive(false);
    }
    void OnClickStart()
    {
        if (isRunning)
        {
            ConfirmBtn.gameObject.SetActive(true);
            isRunning = false;
            Sound.StopDice();
            return;
        }
        try
        {
            //核算基准值
            faces = ToInt32(FacesFld.text);
            round = ToInt32(RoundFld.text);
            //核算加值
            if (AdjustPerRollFld.text.Contains("%"))
            {
                isPerRollMulty = true;
                adjustPerRoll = 1.0f + ToSingle(AdjustPerRollFld.text.Substring(0, AdjustPerRollFld.text.Length - 1)) / 100.0f;
            }
            else
            {
                isPerRollMulty = false;
                adjustPerRoll = ToInt32(AdjustPerRollFld.text);
            }
            if (AdjustFld.text.Contains("%"))
            {
                isWholeMulty = true;
                adjust = 1.0f + ToSingle(AdjustFld.text.Substring(0, AdjustFld.text.Length - 1)) / 100.0f;
            }
            else
            {
                isWholeMulty = false;
                adjust = ToInt32(AdjustFld.text);
            }
            //核算总值
            dice = new Dice(faces);
            flushtime = Time.time;
            isRunning = true;
            Sound.StartDice();
            ConfirmBtn.gameObject.SetActive(false);
        }
        catch (System.Exception)
        {
            TitleTxt.text = "遇到错误，可能是输入不正确。";
        }
    }

    void FlushResult()
    {
        float temp = dice.roll(round);
        if (isPerRollMulty)
            temp *= adjustPerRoll;
        else
            temp += round * adjustPerRoll;
        if (isWholeMulty)
            temp *= adjust;
        else
            temp += adjust;
        result = (int)temp;
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
