using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    AudioSource audioSource;

    [Header("Audio Data")]
    [SerializeField] AudioData _audioData;

    float pitch;
    float volume;

    public enum SFX { ballHit ,chip ,error,select};

    private void Awake()
    {
        if (instance == null)
            instance = this;

        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        volume = audioSource.volume;
        pitch = audioSource.pitch;
    }


    // Start is called before the first frame update
    void Start()
    {
     
    }

    /// <summary>
    /// Play the SFX Sound
    /// </summary>
    /// <param name="sound"></param>
    public void  PlaySFX(SFX sound)
    {
        switch (sound) 
        {
            case SFX.ballHit:

                if (audioSource.isPlaying && audioSource)
                    return;

                audioSource.PlayOneShot(_audioData.GetClip(AudioType.SFX, "BallHit"));
                break;

            case SFX.chip:
                audioSource.PlayOneShot(_audioData.GetClip(AudioType.SFX, "Chip"));
                break;

            case SFX.error:
                audioSource.PlayOneShot(_audioData.GetClip(AudioType.SFX, "Chip"));
                break;

            case SFX.select:
                audioSource.PlayOneShot(_audioData.GetClip(AudioType.SFX, "Select"));
                break;
        }
    }
}
