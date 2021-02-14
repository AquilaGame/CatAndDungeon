using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectVisualWithAnim : CharacterSelectVisrualObj
{
    public float RotateSpd = 10.0f;
    float rot = 0;
    // Update is called once per frame
    void Update()
    {
        rot += RotateSpd * Time.deltaTime;
        transform.localEulerAngles = new Vector3(90, rot, 0);
    }
}
