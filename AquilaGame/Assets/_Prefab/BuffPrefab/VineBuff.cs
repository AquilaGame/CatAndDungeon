using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineBuff : MonoBehaviour
{
    [SerializeField] ParticleSystem[] PSs;
    [SerializeField] float delayTime = 1.0f;
    [SerializeField] AudioClip sound;
    public void Hold()
    {
        StartCoroutine(StopParticle());
        Sound.Play(sound);
    }
    IEnumerator StopParticle()
    {
        yield return new WaitForSeconds(delayTime);
        foreach (var v in PSs)
        {
            v.Pause();
        }
       
    }
}
