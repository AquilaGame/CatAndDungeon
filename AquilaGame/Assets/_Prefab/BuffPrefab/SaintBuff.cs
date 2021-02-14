using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaintBuff : MonoBehaviour
{
    [SerializeField] Transform RangeTrans;
    [SerializeField] AudioClip audioClip;
    public void SetUp(float size)
    {
        RangeTrans.localScale = new Vector3(size, 1, size);
        Sound.Play(audioClip);
    }
    public void PlaySound()
    {
        Sound.Play(audioClip);
    }
}
