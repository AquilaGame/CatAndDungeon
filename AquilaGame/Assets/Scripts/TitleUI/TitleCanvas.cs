using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TitleCanvas : MonoBehaviour
{
    [SerializeField] Button SetupBtn = null;
    [SerializeField] Button StartBtn = null;
    [SerializeField] Button IntroBtn = null;
    [SerializeField] Button ExitBtn = null;
    [SerializeField] Button DMmodeBtn = null;
    [SerializeField] Button PCmodeBtn = null;
    

    private void Start()
    {
        StartBtn.onClick.AddListener(OnClickStart);
        ExitBtn.onClick.AddListener(() => { Application.Quit(); });
        DMmodeBtn.onClick.AddListener(OnClickDMmode);
        PCmodeBtn.onClick.AddListener(OnClickPCmode);
    }

    void OnClickStart()
    {
        if (DMmodeBtn.gameObject.activeSelf)
        {
            DMmodeBtn.gameObject.SetActive(false);
            PCmodeBtn.gameObject.SetActive(false);
        }
        else
        {
            DMmodeBtn.gameObject.SetActive(true);
            PCmodeBtn.gameObject.SetActive(true);
        }
    }

    void OnClickDMmode()
    {
        gameObject.SetActive(false);
    }
    void OnClickPCmode()
    {
        //之后做联网用的
    }
}
