using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Core;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Examples
{
    public class ChatBot : MonoBehaviour
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
        [SerializeField] private InputField chatBox;
        [SerializeField] private List<Message> messageList = new List<Message>();
        [SerializeField] private Color aiMessageColor;
        [SerializeField] private Color playerMessageColor;


        [Header("ChatGPT")]
        private ChatGPT chatGPT;

        private void Start()
        {
            if (!TryGetComponent(out chatGPT)) Destroy(gameObject);

            chatGPT.OnPartResponseReceived += () =>
            {
                SetLastMessage(chatGPT.response);
            };

            chatBox.ActivateInputField();
        }

        private void Update()
        {
            if (chatBox.text != "")
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SendMessageToChat(chatBox.text, Message.MessageType.playerMessage);
                    SendToOpenAI();
                    chatBox.text = "";
                    chatBox.ActivateInputField();
                }
            }
            else
            {
                if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
                {
                    chatBox.ActivateInputField();
                }
            }
        }

        private async void SendToOpenAI()
        {
            chatGPT.question = chatBox.text;
            SendMessageToChat("", Message.MessageType.aiMessage);
            OpenAIJsons.ChatResponse aiResponse = await chatGPT.MakeGTPCall();

            if (aiResponse != null)
            {
                SetLastMessage(aiResponse.choices[0].message.content);

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

        private void SetLastMessage(string text)
        {
            messageList[messageList.Count - 1].textObject.text = text;
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
