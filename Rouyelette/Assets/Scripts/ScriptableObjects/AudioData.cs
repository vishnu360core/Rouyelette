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
}


public enum AudioType { SFX, CLIP };

[System.Serializable]
public class AudioClipData
{
    public AudioType _type;

    public AudioClip _clip;
}

