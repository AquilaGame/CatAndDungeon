using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillUnitPointer : MonoBehaviour
{
    RectTransform trans;
    [SerializeField] Image SeatImg = null;
    [SerializeField] Image ChildImg = null;
    [SerializeField] float RotateSpd = 5.0f;
    public void SetUp(Color color , Sprite sprite)
    {
        trans = SeatImg.GetComponent<RectTransform>();
        ChildImg.sprite = sprite;
        SeatImg.color = color;
        ChildImg.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        trans.localEulerAngles = new Vector3(0, 0, trans.localEulerAngles.z + RotateSpd * Time.deltaTime);
    }
}
