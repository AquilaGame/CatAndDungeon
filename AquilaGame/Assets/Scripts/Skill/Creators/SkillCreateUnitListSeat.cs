using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillCreateUnitListSeat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public int number;
    [SerializeField]Sprite[] sprites = null;
    [SerializeField] Sprite DefaultSprite = null;
    [SerializeField] Sprite UnlockSprite = null;
    public SkillCreatePanel Pnl = null;
    bool isActive = false;
    bool isFilled = false;
    bool isLocked = true;
    int count = 0;
    int frame = 0;
    Color SeatSpriteColor;
    Color DefaultColor;
    [SerializeField] Image SeatAnim = null;
    [SerializeField] Image SelfImg = null;
    // Start is called before the first frame update
    void Start()
    {
        DefaultColor = SelfImg.color;
        SeatSpriteColor = SeatAnim.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            count++;
            if (count == 30)
            {
                SeatAnim.sprite = sprites[frame++];
                if (frame > 3)
                    frame = 0;
                count = 0;
            }
        }
    }

    public void SetFill()
    {
        isFilled = true;
        SeatAnim.sprite = DefaultSprite;
        SeatAnim.color = Pnl.pointerColor;
        SelfImg.sprite = Pnl.pointerSprite;
        SelfImg.color = Pnl.pointerColor;
        isActive = false;
    }

    public void SetFill(Sprite sprite, Color color)
    {
        isFilled = true;
        SeatAnim.sprite = DefaultSprite;
        SeatAnim.color = color;
        SelfImg.sprite = sprite;
        SelfImg.color = color;
        isActive = false;
    }

    public void ResetFill()
    {
        isFilled = false;
        SeatAnim.sprite = DefaultSprite;
        SeatAnim.color = SeatSpriteColor;
        SelfImg.sprite = UnlockSprite;
        SelfImg.color = DefaultColor;
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (isLocked)
            return;
        if (Pnl.isPointerObjActive)
        {
            Pnl.ActiveSeat = this;
            if (!isFilled)
            {
                isActive = true;
                SeatAnim.sprite = sprites[0];
                SeatAnim.color = Pnl.pointerColor;
                SelfImg.color = new Color(0, 0, 0, 0);
            }
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (isLocked)
            return;
        if (!isFilled)
        {
            isActive = false;
            SeatAnim.sprite = DefaultSprite;
            SeatAnim.color = SeatSpriteColor;
            SelfImg.color = DefaultColor;
        }
        Pnl.ActiveSeat = null;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (isFilled)
        {
            Pnl.OnClickSeatToClear(number);
        }
    }

    public void Unlock()
    {
        isLocked = false;
        SelfImg.sprite = UnlockSprite;
    }
}
