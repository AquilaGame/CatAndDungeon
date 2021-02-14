using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBuff : MonoBehaviour
{
    [SerializeField] ParticleSystem[] PSs;
    [SerializeField] float delayTime = 1.0f;
    [SerializeField] AudioClip sound;
    public void Hold()
    {
        StartCoroutine(StopParticle());
        Sound.Play(sound);
    }
    public void Restart()
    {
        foreach (var v in PSs)
        {
            v.Play();
        }
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
