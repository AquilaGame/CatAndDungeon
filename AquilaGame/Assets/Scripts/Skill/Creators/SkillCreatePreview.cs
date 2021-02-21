using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillCreatePreview : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    bool isHighLight = false;
    [SerializeField] float RotateSpd = 5.0f;
    [SerializeField]RectTransform SelfTrans;
    [SerializeField] SkillCreatePanel Pnl;
    [SerializeField] Image PreviewIcon;
    [SerializeField] Image SelfImage;
    float r = 0, g = 0, b = 0;
    public float FadeSpeed = 2.0f;
    float alpha = 1.0f;
    bool isAlphaAdd = true;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void SetUp(Color color, Sprite sprite)
    {
        SelfTrans.localEulerAngles = Vector3.zero;
        PreviewIcon.color = color;
        SelfImage.color = color;
        r = color.r;
        g = color.g;
        b = color.b;
        PreviewIcon.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        SelfTrans.localEulerAngles = new Vector3(0, 0, SelfTrans.localEulerAngles.z + RotateSpd * Time.deltaTime);
        if(isHighLight)
        {
            if (isAlphaAdd)
            {
                alpha += FadeSpeed * Time.deltaTime;
                if (alpha > 1.0f)
                {
                    alpha = 1.0f;
                    isAlphaAdd = false;
                }
            }
            else
            {
                alpha -= FadeSpeed * Time.deltaTime;
                if (alpha < 0.3f)
                {
                    alpha = 0.3f;
                    isAlphaAdd = true;
                }
            }
            SelfImage.color = new Color(r,g,b,alpha);
        }
    }
    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        isHighLight = true;
        Pnl.SetPreviewPanel(true);
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        isHighLight = false;
        alpha = 1.0f;
        SelfImage.color = new Color(r, g, b, alpha);
        Pnl.SetPreviewPanel(false);
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Pnl.OnClickSubmit();
    }
}
