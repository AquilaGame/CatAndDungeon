using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ClockCtrl : MonoBehaviour
{
    [SerializeField] RectTransform ClockTrans = null;
    [SerializeField] Button TimeAddBtn = null;
    [SerializeField] Button TimeMinusBtn = null;
    [SerializeField] Button CaveModeBtn = null;
    [SerializeField] Button OutsideModeBtn = null;
    [SerializeField] Text TimeTxt = null;
    [SerializeField] SunCtrl sun = null;
    [SerializeField] int TimeChangeDelta = 30;
    [SerializeField] float RotateSpd = 10.0f;
    int hour = 12;
    int minute = 0;
    bool isClockwise = true;
    float nowTimeAngle = 180.0f;
    float targetTimeAngle = 180.0f;
    string nowTimeStr
    {
        get
        {
            int minute = (int)(nowTimeAngle * 4.0f);
            minute += 1440;
            minute %= 1440;
            return string.Format("{0:D2}:{1:D2}",minute/ 60,minute % 60);
        }
    }

    void AddTime()
    {
        minute += hour * 60;
        minute += TimeChangeDelta;
        minute %= 1440;
        hour = minute / 60;
        minute %= 60;
        targetTimeAngle = hour * 15 + minute * 0.25f;
        isClockwise = true;
        if (targetTimeAngle < nowTimeAngle)
            nowTimeAngle -= 360.0f;
    }

    void MinusTime()
    {
        minute += hour * 60;
        minute -= TimeChangeDelta;
        while (minute < 0)
            minute += 1440;
        hour = minute / 60;
        minute %= 60;
        targetTimeAngle = hour * 15 + minute * 0.25f;
        isClockwise = false;
        if (targetTimeAngle > nowTimeAngle)
            nowTimeAngle += 360.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        TimeAddBtn.onClick.AddListener(AddTime);
        TimeMinusBtn.onClick.AddListener(MinusTime);
        CaveModeBtn.onClick.AddListener(() => { sun.isCave = true; });
        OutsideModeBtn.onClick.AddListener(() => { sun.isCave = false; sun.SetSun(nowTimeAngle); });
    }

    // Update is called once per frame
    void Update()
    {
        if (nowTimeAngle != targetTimeAngle)
        {
            float rotDelta = RotateSpd * Time.deltaTime;
            if (isClockwise)
            {
                if (targetTimeAngle - nowTimeAngle > rotDelta)
                {
                    nowTimeAngle += rotDelta;
                }
                else
                {
                    nowTimeAngle = targetTimeAngle;
                }
            }
            else
            {
                if (nowTimeAngle - targetTimeAngle > rotDelta)
                {
                    nowTimeAngle -= rotDelta;
                }
                else
                {
                    nowTimeAngle = targetTimeAngle;
                }
            }
            ClockTrans.localEulerAngles = new Vector3(0, 0, nowTimeAngle);
            TimeTxt.text = nowTimeStr;
            sun.SetSun(nowTimeAngle);
        }
    }
}
