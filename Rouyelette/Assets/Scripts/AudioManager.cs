using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.MonoBehaviours;
using UnityEngine.UI;
using System.Threading.Tasks;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    AudioSource audioSource;

    public AudioSource Audio => audioSource;

    [Header("Audio Data")]
    [SerializeField] AudioData _audioData;

    float pitch;
    float volume;
    
    [Space]
    [Header("SpeechTextManager:")]
    [SerializeField] AzureTextToSpeech _speechBot;
    [SerializeField] AudioSource _speechAudioSource;

    public bool _isSpeechLoaded = false;

    public enum SFX { ballHit ,chip ,error,select,win,loss};

    public enum Clip { wheel }

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

    public async void SpeechAction(string speech)
    {
        while (_speechAudioSource.isPlaying)
        {

        }

        _speechAudioSource.clip = await _speechBot.CallAzure(speech).ContinueWith(task =>
        {
            _isSpeechLoaded = task.IsCompletedSuccessfully;
            return task.Result;

        });

        _speechAudioSource.Play();
        _isSpeechLoaded = false;
    }


   
   
    public void PlayClip(Clip clip)
    {
        switch(clip) 
        {
            case Clip.wheel:
               
                audioSource.clip = _audioData.GetClip(AudioType.CLIP, "Rouyellete");
                audioSource.Play();
                audioSource.loop = true;
                break;
        }
    }



    /// <summary>
    /// Play the SFX Sound
    /// </summary>
    /// <param name="sound"></param>
    public void  PlaySFX(SFX sound)
    {
        audioSource.loop = false;

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

             case SFX.win:
                audioSource.PlayOneShot(_audioData.GetClip(AudioType.SFX, "Win"));
                break;

            case SFX.loss:
                audioSource.PlayOneShot(_audioData.GetClip(AudioType.SFX, "Lost"));
                break;
        }
    }
}
