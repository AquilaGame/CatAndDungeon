using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAppearanceSettingPnl : MonoBehaviour
{
    [SerializeField] CreateCharecterPanel Pnl = null;
    [SerializeField] Image Background = null;
    [SerializeField] Slider RotSlider = null;
    [SerializeField] Text InfoTxt = null;
    [SerializeField] Sprite[] BackgroundSprites = null;
    [SerializeField] Button[] Buttons = null;
    [SerializeField] string[] Infos = null;
    private void Start()
    {
        Buttons[0].onClick.AddListener(() => { OnGame.characterPreviewer.gameObject.SetActive(true); InfoTxt.text = Infos[0]; Background.sprite = BackgroundSprites[0]; Pnl.SetBirth(0); RotSlider.value = 0.5f;Pnl.FlushCharacterPreview(); });
        Buttons[1].onClick.AddListener(() => { OnGame.characterPreviewer.gameObject.SetActive(true); InfoTxt.text = Infos[1]; Background.sprite = BackgroundSprites[1]; Pnl.SetBirth(1); RotSlider.value = 0.5f;Pnl.FlushCharacterPreview(); });
        Buttons[2].onClick.AddListener(() => { OnGame.characterPreviewer.gameObject.SetActive(true); InfoTxt.text = Infos[2]; Background.sprite = BackgroundSprites[2]; Pnl.SetBirth(2); RotSlider.value = 0.5f;Pnl.FlushCharacterPreview(); });
        Buttons[3].onClick.AddListener(() => { OnGame.characterPreviewer.gameObject.SetActive(true); InfoTxt.text = Infos[3]; Background.sprite = BackgroundSprites[3]; Pnl.SetBirth(3); RotSlider.value = 0.5f;Pnl.FlushCharacterPreview(); });
        Buttons[4].onClick.AddListener(() => { OnGame.characterPreviewer.gameObject.SetActive(true); InfoTxt.text = Infos[4]; Background.sprite = BackgroundSprites[4]; Pnl.SetBirth(4); RotSlider.value = 0.5f;Pnl.FlushCharacterPreview(); });
        Buttons[5].onClick.AddListener(() => { OnGame.characterPreviewer.gameObject.SetActive(true); InfoTxt.text = Infos[5]; Background.sprite = BackgroundSprites[5]; Pnl.SetBirth(5); RotSlider.value = 0.5f;Pnl.FlushCharacterPreview(); });
        Buttons[6].onClick.AddListener(() => { OnGame.characterPreviewer.gameObject.SetActive(true); InfoTxt.text = Infos[6]; Background.sprite = BackgroundSprites[6]; Pnl.SetBirth(6); RotSlider.value = 0.5f;Pnl.FlushCharacterPreview(); });
        Buttons[7].onClick.AddListener(() => { OnGame.characterPreviewer.gameObject.SetActive(true); InfoTxt.text = Infos[7]; Background.sprite = BackgroundSprites[7]; Pnl.SetBirth(7); RotSlider.value = 0.5f;Pnl.FlushCharacterPreview(); });
        Buttons[8].onClick.AddListener(() => { OnGame.characterPreviewer.gameObject.SetActive(true); InfoTxt.text = Infos[8]; Background.sprite = BackgroundSprites[8]; Pnl.SetBirth(8); RotSlider.value = 0.5f; Pnl.FlushCharacterPreview(); });
        RotSlider.onValueChanged.AddListener(Pnl.SetPreviewRot);
    }
}
