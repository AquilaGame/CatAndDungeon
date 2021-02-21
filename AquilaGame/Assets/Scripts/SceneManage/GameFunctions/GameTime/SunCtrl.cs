using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCtrl : MonoBehaviour
{
    [SerializeField] Light NatureLight = null;
    [SerializeField] Color DawnColor = Color.white;
    [SerializeField] Color NoonColor = Color.white;
    [SerializeField] Color DuskColor = Color.white;
    float sunHighAngle = 60.0f;
    float sunDirAngle = 0.0f;
    bool isSunWork
    {
        get { return NatureLight.gameObject.activeSelf; }
        set { NatureLight.gameObject.SetActive(value); }
    }
    bool _isCave = false;
    public bool isCave
    {
        get { return _isCave; }
        set { _isCave = value; isSunWork = !value; }
    }
    public void SetSun(float nowTimeAngle)
    {
        if (isCave)
        {
            return;
        }
        //早6点对应90
        //正午对应180
        //晚6点对应270
        if (nowTimeAngle > 180.0f)
        {
            if (nowTimeAngle < 270.0f)//180->270
            {
                float delta = (270.0f - nowTimeAngle) / 90.0f;
                isSunWork = true;
                NatureLight.color = Color.Lerp(DuskColor, NoonColor, delta);
                NatureLight.intensity = delta * 3.0f > 1.0f ? 1.0f : delta * 3.0f;
                sunDirAngle = 90.0f * (1.0f - delta);
                sunHighAngle = 30.0f * (delta + 1.0f);
                NatureLight.transform.localEulerAngles = new Vector3(sunHighAngle, 0, 0);
                transform.localEulerAngles = new Vector3(0, sunDirAngle, 0);
            }
            else
            {
                isSunWork = false;
            }
        }
        else
        {
            if (nowTimeAngle > 90.0f)//90->180
            {
                float delta = (nowTimeAngle - 90.0f) / 90.0f;
                isSunWork = true;
                NatureLight.color = Color.Lerp(DawnColor, NoonColor,delta);
                NatureLight.intensity = delta * 3.0f > 1.0f ? 1.0f : delta * 3.0f;
                sunDirAngle = 90.0f * (delta - 1.0f);
                sunHighAngle = 30.0f * (delta + 1.0f);
                NatureLight.transform.localEulerAngles = new Vector3(sunHighAngle, 0, 0);
                transform.localEulerAngles = new Vector3(0, sunDirAngle, 0);
            }
            else
            {
                isSunWork = false;
            }
        }
    }


}
