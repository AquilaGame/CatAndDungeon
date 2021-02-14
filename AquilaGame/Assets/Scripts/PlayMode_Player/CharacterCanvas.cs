using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterCanvas : MonoBehaviour
{
    enum displayType {NameStr,HP}
    public Text NameTxt;
    public Slider HPSlider;
    public CharacterScript character;
    displayType nowDisType = displayType.NameStr;

    private void Start()
    {
        character = transform.parent.GetComponent<CharacterScript>();
        character.SetCanvas(this);
        if (character.data != null)
            SetDisplay();
        else
            Destroy(gameObject);
    }

    // 这个东西放在Update里只会被人说是哪个傻逼写的代码
    // 先放着明天再改
    // 咕咕咕
    // 帧率降到60以下一定改
    // 咕咕咕
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            nowDisType = displayType.HP;
            SetDisplay();
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            nowDisType = displayType.NameStr;
            SetDisplay();
        }
        if (nowDisType == displayType.HP)
        {
            float f = (float)character.data.BaseData.HP / character.data.BaseData.MaxHP;
            HPSlider.value = f > 0 ? f : 0;
        }
    }

    public void SetDisplay()
    {
        switch (nowDisType)
        {
            case displayType.NameStr:
                HPSlider.gameObject.SetActive(false);
                NameTxt.gameObject.SetActive(true);
                NameTxt.text = character.data.Name;
                NameTxt.color = character.isOperating ? Alignment.ColorList[3] : Alignment.ColorList[(int)character.data.characterType];
                break;
            case displayType.HP:
                HPSlider.gameObject.SetActive(true);
                NameTxt.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}
