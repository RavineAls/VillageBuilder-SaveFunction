using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip placementSound;
    public AudioClip destroySound;
    public AudioClip backgroundMusic;
    public AudioSource audioSource;

    public static AudioPlayer instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);
    }

    public void PlayPlacementSound()
    {
        if(placementSound != null)
        {
            audioSource.PlayOneShot(placementSound);
        }
    }

    public void PlayDestroySound()
    {
        if(destroySound != null)
        {
            audioSource.PlayOneShot(destroySound);
        }
    }
}