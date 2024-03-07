using System;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.ScriptableObjects;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureKeyVault.MonoBehaviours;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.MonoBehaviours
{
    public class AzureTextToSpeech : MonoBehaviour
    {


        #region variables

        [SerializeField] public AzureSpeechDefaultSO defaultValues;

        [Space(5)]

        [TextArea(1, 10)]
        [SerializeField] public string input = "enter what you want the AI to say here.";

        [SerializeField] public AzureSpeechEnums.languages language = AzureSpeechEnums.languages.en_US;

        public AzureSpeechEnums.ShortNames shortName = AzureSpeechEnums.ShortNames.en_US_AIGenerate1Neural;

        public AzureSpeechEnums.LocalNames localName = AzureSpeechEnums.LocalNames.AIGenerate1;

        public AzureSpeechEnums.DisplayNames displayName = AzureSpeechEnums.DisplayNames.AIGenerate1;


        [HideInInspector] public AudioClip audioClip;


        private void OnValidate()
        {
#if UNITY_EDITOR
            if (defaultValues == null)
            {
                string[] strings = AssetDatabase.FindAssets("Azure Speech Default Values t:AzureSpeechDefaultSO");
                string path = AssetDatabase.GUIDToAssetPath(strings[0]);
                defaultValues = AssetDatabase.LoadAssetAtPath<AzureSpeechDefaultSO>(path);
            }
#endif
            if (language == default && defaultValues != null)
            {
                language = defaultValues.language;
            }
        }

        #endregion

        public async Task<AudioClip> CallAzure()
        {

            string subscriptionKey;
#if UNITY_EDITOR
            string[] strings = AssetDatabase.FindAssets("Azure Speech Key t:AzureSpeechKeysSO");
            string path = AssetDatabase.GUIDToAssetPath(strings[0]);
            AzureSpeechKeysSO key = AssetDatabase.LoadAssetAtPath<AzureSpeechKeysSO>(path);
            subscriptionKey = key.subscriptionKey;
#else
            subscriptionKey = await GameObject.Find("AzureSpeech_Key_Vualt_Manager").GetComponent<KeyVualtManager>().keyVault.GetSecret();
#endif

            TextToSpeech textToSpeech = new TextToSpeech(language, defaultValues.region, subscriptionKey);
            audioClip = await textToSpeech.CallAzure(input, (AzureSpeechEnums.ShortNames)shortName);
            return audioClip;
        }
        public async Task<AudioClip> CallAzure(string text)
        {
            input = text;
            return await CallAzure();
        }

    }
}
