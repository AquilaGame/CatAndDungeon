using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActionPointPanel : MonoBehaviour
{
    [SerializeField] CharacterPanel Pnl = null;
    [SerializeField] Slider SlotFld = null;
    [SerializeField] Slider PreviewFld = null;
    [SerializeField] Image PreviewImg = null;
    [SerializeField] float ShineSpeed = 0.3f;
    [SerializeField] Color nowColor = Color.white;

    bool isAlphaAdd = false;

    private void Start()
    {
        SlotFld.maxValue = Character.ONE_TURN_AP * 2;
        PreviewFld.maxValue = Character.ONE_TURN_AP * 2;
        PreviewFld.value = 0;
        SlotFld.onValueChanged.AddListener(SetValueOnDrag);
    }

    public void SetValueOnDrag(float value)
    {
        SlotFld.value = value;
        Pnl.characterOnSelect.AP = value;
    }

    public void SetValue(float value)
    {
        SlotFld.value = value;
    }

    public void SetPreview(float value)
    {
        PreviewFld.value = value;
    }

    // Update is called once per frame
    void Update()
    {
        if (PreviewFld.value <= 0)
            return;
        float alphaDelta = ShineSpeed * Time.deltaTime;
        if (isAlphaAdd)
        {
            if (nowColor.a + alphaDelta > 1.0f)
            {
                nowColor.a = 1.0f;
                isAlphaAdd = false;
            }
            else
                nowColor.a += alphaDelta;
        }
        else
        {
            if (nowColor.a - alphaDelta < 0.3f)
            {
                nowColor.a = 0.3f;
                isAlphaAdd = true;
            }
            else
                nowColor.a -= alphaDelta;
        }
        PreviewImg.color = nowColor;
    }
}
