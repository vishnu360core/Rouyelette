using UnityEngine;
using UnityEngine.UI;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.MonoBehaviours;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Examples
{
    public class SpeechToText_AzureSpeechManager : MonoBehaviour
    {


        [SerializeField] private Text Text;
        [SerializeField] private int microphoneIndex;


        private AudioClip audioClip;
        private MicrophoneUtils microphone;
        private AzureSpeechToText speechToText;


        private void Awake()
        {
            microphone = new MicrophoneUtils(microphoneIndex);
            speechToText = GetComponent<AzureSpeechToText>();
        }

        public void StartRecording()
        {
            microphone.StartRecording();
        }

        public void StopRecording()
        {
            audioClip = microphone.StopRecording();
            GetText();
        }

        private async void GetText()
        {
            Text.text = await speechToText.CallAzure(audioClip);
        }


    }
}