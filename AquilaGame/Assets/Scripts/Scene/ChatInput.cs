using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatInput : MonoBehaviour
{
    [SerializeField] InputField inputField = null;
    [SerializeField] Button confirmBtn = null;
    public Color color = Color.magenta;
    string nameStr = "未设定";
    string colorStr = "<color=#FF66FF>[";
    const string endStr = "]</color>:";
    // Start is called before the first frame update
    void Start()
    {
        SetName("Aquila");
        confirmBtn.onClick.AddListener(ClickChat);
    }

    public void SetName(string name)
    {
        nameStr = colorStr + name + endStr;
    }

    void ClickChat()
    {
        Chat(inputField.text);
        OnScene.isChating = false;
    }

    void Chat(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return;
        OnScene.Report(nameStr + str);
        inputField.text = "";
    }

    private void Update()
    {
        if (OnScene.isAllowedToChat)
        {
            if (Input.GetButtonDown("Chat"))
            {
                if (OnScene.isChating)
                {
                    ClickChat();
                    inputField.DeactivateInputField();
                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                }
                else
                {
                    OnScene.isChating = true;
                    inputField.ActivateInputField();
                }
            }
        }
    }
}
