using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBase : MonoBehaviour
{
    [SerializeField] AudioSource audioSource = null;
    [HideInInspector] public object Using = null;
    int index = 0;
    bool isPlayEnable = false;
    public void Init(int i)
    {
        index = i;
    }
    public void Play(AudioClip clip)
    {
        if(audioSource.clip!=null)
            audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
        isPlayEnable = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isPlayEnable)
        {
            if (audioSource.isPlaying == false)
            {
                Sound.manager.Recycle(index);
                isPlayEnable = false;
            }
        }
    }
}
