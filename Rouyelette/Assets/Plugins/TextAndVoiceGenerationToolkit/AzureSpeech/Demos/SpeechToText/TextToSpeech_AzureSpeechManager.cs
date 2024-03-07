using UnityEngine;
using UnityEngine.UI;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.MonoBehaviours;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Examples
{
    public class TextToSpeech_AzureSpeechManager : MonoBehaviour
    {


        [SerializeField] private InputField inputField;


        private AzureTextToSpeech azureTextToSpeech;

        private AudioClip audioClip;
        private AudioSource audioSource;


        private void Awake()
        {
            azureTextToSpeech = GetComponent<AzureTextToSpeech>();
            audioSource = GetComponent<AudioSource>();
        }

        public async void GetAudio()
        {
            audioClip = await azureTextToSpeech.CallAzure(inputField.text);
        }

        public void PlayAudio()
        {
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
            }
            audioSource.Play();
        }


    }
}
