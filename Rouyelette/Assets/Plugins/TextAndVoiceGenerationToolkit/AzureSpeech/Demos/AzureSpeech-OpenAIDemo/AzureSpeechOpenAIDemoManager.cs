using System;
using System.Collections;
using System.Collections.Generic;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.MonoBehaviours;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours;

using UnityEditor.Purchasing;

using UnityEngine;
using UnityEngine.UI;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Examples
{
    public class AzureSpeechOpenAIDemoManager : MonoBehaviour
    {

        public event EventHandler<OnGetResponseEventArgs> OnGetResponse;
        public class OnGetResponseEventArgs : EventArgs
        {
            public string message;
        }


        [Header("ChatBox")]

        [SerializeField] private int maxMessages;
        [SerializeField] private Transform chatPanle;
        [SerializeField] private GameObject aiTextObject;
        [SerializeField] private GameObject playerTextObject;
        [SerializeField] private List<Message> messageList = new List<Message>();
        [SerializeField] private Color aiMessageColor;
        [SerializeField] private Color playerMessageColor;


        [Header("References")]
        [SerializeField] private ChatGPT chatGPT;
        [SerializeField] private AzureSpeechToText azureSpeechToText;
        [SerializeField] private AzureTextToSpeech azureTextToSpeech;
        [SerializeField] private AudioSource audioSource;

        [Header("Recording Audio")]
        [SerializeField] private int micIndex;

        private AudioClip recordedAudio;
        private MicrophoneUtils microphone;

        [Header("Other")]
        private string userInput;
        private AudioClip aiAudioClip;


        private void Awake()
        {
            microphone = new MicrophoneUtils(micIndex);
        }

        private void Start()
        {
            OnGetResponse += AzureSpeechOpenAIDemoManager_OnGetResponse;
        }


        public void OnRecordAudioClicked()
        {
            microphone.StartRecording();
        }

        public void OnStopRecordingAudioClicked()
        {
            recordedAudio = microphone.StopRecording();
        }

        public async void OnSendClicked()
        {
            userInput = await azureSpeechToText.CallAzure(recordedAudio);

            SendMessageToChat(userInput, Message.MessageType.playerMessage);
            SendToGPT3();
        }

        private async void SendToGPT3()
        {
            chatGPT.question = userInput;
            OpenAIJsons.ChatResponse aiResponse = await chatGPT.MakeGTPCall();
            if (aiResponse != null)
            {
                SendMessageToChat(aiResponse.choices[0].message.content, Message.MessageType.aiMessage);
                OnGetResponse?.Invoke(this, new OnGetResponseEventArgs
                {
                    message = aiResponse.choices[0].message.content
                });
            }
        }

        private void SendMessageToChat(string text, Message.MessageType messageType)
        {
            if (messageList.Count > maxMessages)
            {
                Destroy(messageList[0].textObject.gameObject);
                messageList.Remove(messageList[0]);
            }

            Message newMessage = new Message();

            newMessage.text = text;

            GameObject newTextObject = Instantiate(MessageTextType(messageType), chatPanle);

            newMessage.textObject = newTextObject.GetComponent<Text>();

            newMessage.textObject.text = newMessage.text;

            newMessage.textObject.color = MessageColorType(messageType);

            messageList.Add(newMessage);
        }

        private Color MessageColorType(Message.MessageType messageType)
        {
            Color color = Color.white;

            switch (messageType)
            {
                case Message.MessageType.aiMessage:
                    color = aiMessageColor;
                    break;
                case Message.MessageType.playerMessage:
                    color = playerMessageColor;
                    break;
            }

            return color;
        }

        private GameObject MessageTextType(Message.MessageType messageType)
        {
            GameObject newGameObject = playerTextObject;

            switch (messageType)
            {
                case Message.MessageType.aiMessage:
                    newGameObject = aiTextObject;
                    break;
                case Message.MessageType.playerMessage:
                    newGameObject = playerTextObject;
                    break;
            }

            return newGameObject;
        }

        private async void AzureSpeechOpenAIDemoManager_OnGetResponse(object sender, OnGetResponseEventArgs e)
        {
            aiAudioClip = await azureTextToSpeech.CallAzure(e.message);
            audioSource.clip = aiAudioClip;
            audioSource.Play();
        }

        [Serializable]
        public class Message
        {
            public Text textObject;
            public string text;
            public MessageType messageType;

            public enum MessageType
            {
                playerMessage,
                aiMessage,
            }
        }

    }
}
