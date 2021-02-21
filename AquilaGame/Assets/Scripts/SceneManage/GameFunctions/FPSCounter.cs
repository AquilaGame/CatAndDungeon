using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FPSCounter : MonoBehaviour
{
    Text Txt;
    int count = 0;
    private void Start()
    {
        Txt = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        count++;
        if (count >= 10)
        {
            Txt.text = "FPS:" + 1.0f / Time.deltaTime;
            count = 0;
        }
    }
}
