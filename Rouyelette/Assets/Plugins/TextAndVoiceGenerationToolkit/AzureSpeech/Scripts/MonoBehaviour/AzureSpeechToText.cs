using System.Threading.Tasks;

using UnityEngine;

using UnityEditor;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.ScriptableObjects;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureKeyVault.MonoBehaviours;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.MonoBehaviours
{
    public class AzureSpeechToText : MonoBehaviour
    {


        [SerializeField] public AudioClip audioClip;
        [SerializeField, TextArea(1, 10)] public string text;
        [SerializeField] public AzureSpeechEnums.languages languageOfSpeech = default;


        [SerializeField] public AzureSpeechDefaultSO defaultValues;

        private void OnValidate()
        {
            if (defaultValues == null)
            {
#if UNITY_EDITOR
                string[] strings = AssetDatabase.FindAssets("Azure Speech Default Values t:AzureSpeechDefaultSO");
                string path = AssetDatabase.GUIDToAssetPath(strings[0]);
                defaultValues = AssetDatabase.LoadAssetAtPath<AzureSpeechDefaultSO>(path);
#endif
            }

            if (languageOfSpeech == default)
            {
                languageOfSpeech = defaultValues.language;
            }
        }



        public async Task<string> CallAzure()
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

            SpeechToText speechToText = new SpeechToText(languageOfSpeech, defaultValues.region, subscriptionKey);


            AzureSpeechJsons.SpeechToTextResponse response = await speechToText.GetTextFromSpeech(audioClip);

            text = response.DisplayText;
            return text;
        }

        public async Task<string> CallAzure(AudioClip audioClip)
        {
            this.audioClip = audioClip;
            return await CallAzure();
        }


    }
}
