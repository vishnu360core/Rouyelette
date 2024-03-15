using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Storage/Audio")] 
public class AudioData : ScriptableObject
{
    [Header("AudioClips:")]
    [SerializeField] List<AudioClipData> clipDatas = new List<AudioClipData>(); 

    /// <summary>
    /// Get the clip
    /// </summary>
    /// <param name="audioType"></param>
    /// <param name="clipName"></param>
    /// <returns></returns>
    public AudioClip GetClip(AudioType audioType ,string clipName)
    {
        return clipDatas.Find(element => element._clip.name == clipName)._clip;
    }

    /// <summary>
    /// Get the speech
    /// </summary>
    /// <param name="speech"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public AudioClip GetSpeech(Speech speech, int number =-1)
    {
        AudioClip clip = null;

        switch(speech)
        {
            case Speech.placeBet:
                clip = GetClip(AudioType.CLIP, "PlaceYourBet");
                break;

            case Speech.NoMoreBet:
                clip = GetClip(AudioType.CLIP, "NoMoreBets");
                break;

            case Speech.number:
                clip = GetClip(AudioType.CLIP,number.ToString());
                break;
        }


        return clip;
    }
}


public enum AudioType { SFX, CLIP };

public enum Speech {placeBet,NoMoreBet,number}

[System.Serializable]
public class AudioClipData
{
    public AudioType _type;

    public AudioClip _clip;
}

