using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource footstep;
    public AudioClip[] footstepClips;
    private bool footstepPlaying = false;

    public void PlayFootsteps()
    {
        if (footstepPlaying == false)
        {
            footstep.clip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstep.Play();
            footstepPlaying = true;
            StartCoroutine(Wait());
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.3f);
        footstepPlaying = false;
    }
}
