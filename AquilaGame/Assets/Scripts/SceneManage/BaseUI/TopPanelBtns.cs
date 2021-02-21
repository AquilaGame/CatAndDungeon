using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TopPanelBtns : MonoBehaviour
{
    [SerializeField] Button EditMapBtn = null;
    [SerializeField] Button CharacterCreateBtn = null;
    [SerializeField] Button CombatBtn = null;
    [SerializeField] Button RollBtn = null;
    [SerializeField] Button AttackOrderBtn = null;
    [SerializeField] GameObject PlayModeCanvas = null;
    [SerializeField] GameObject EditMapCanvas = null;
    [SerializeField] GameObject CharacterCreatePnlObj = null;
    [SerializeField] GameObject CombatPnlObj = null;
    [SerializeField] GameObject RollPnlObj = null;
    [SerializeField] GameObject AttackOrderPnl = null;
    

    GameObject CharacterCreatePnl = null;
    GameObject CombatPnl = null;
    GameObject RollPnl = null;
    // Start is called before the first frame update
    void Start()
    {
        EditMapBtn.onClick.AddListener(OnClickEditMapBtn);
        CharacterCreateBtn.onClick.AddListener(OnClickCreateCharacterBtn);
        CombatBtn.onClick.AddListener(OnClickCombatBtn);
        RollBtn.onClick.AddListener(OnClickRollBtn);
        AttackOrderBtn.onClick.AddListener(OnClickAttackOrderBtn);
    }

    void OnClickEditMapBtn()
    {
        OnScene.mainCam.cam.orthographic = true;
        EditMapCanvas.SetActive(true);
        PlayModeCanvas.SetActive(false);
        OnScene.isPlayMode = false;
    }
    void OnClickCreateCharacterBtn()
    {
        if (PlayModeCanvas.activeSelf)
        {
            if (CharacterCreatePnl)
                Destroy(CharacterCreatePnl);
            else
                CharacterCreatePnl = Instantiate(CharacterCreatePnlObj, PlayModeCanvas.transform);
        }
    }
    void OnClickCombatBtn()
    {
        if (PlayModeCanvas.activeSelf)
        {
            if (CombatPnl)
                Destroy(CombatPnl);
            else
            {
                CombatPnl = Instantiate(CombatPnlObj, PlayModeCanvas.transform);
                CombatPnl.GetComponent<CombatPanel>().SetUp("自由检定","角色1","角色2", 0, 0);
            }
        }
    }
    void OnClickRollBtn()
    {
        if (PlayModeCanvas.activeSelf)
        {
            if (RollPnl)
                Destroy(RollPnl);
            else
            {
                RollPnl = Instantiate(RollPnlObj, PlayModeCanvas.transform);
                RollPnl.GetComponent<RollPanel>().SetUp("自由掷骰");
            }
        }
    }
    void OnClickAttackOrderBtn()
    {
        if (PlayModeCanvas.activeSelf)
        {
            if (AttackOrderPnl.activeSelf)
            {
                AttackOrderPnl.SetActive(false);
            }
            else
            {
                AttackOrderPnl.SetActive(true);
            }
        }
    }

}
