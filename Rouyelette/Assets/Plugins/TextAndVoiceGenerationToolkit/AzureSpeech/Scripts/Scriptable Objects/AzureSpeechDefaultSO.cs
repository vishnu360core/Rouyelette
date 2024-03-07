using UnityEngine;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Azure Speech Default Values", menuName = "Text and Voice Generation Tookit/Azure Speech/Default Values")]
    public class AzureSpeechDefaultSO : ScriptableObject
    {

        
        [SerializeField] public AzureSpeechEnums.Regions region;
        [SerializeField] public AzureSpeechEnums.languages language = AzureSpeechEnums.languages.af_ZA;


    }
}
