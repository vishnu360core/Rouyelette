using System;
using System.Threading.Tasks;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.ScriptableObjects;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureKeyVault.MonoBehaviours;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours
{
    [AddComponentMenu("Text & Voice Generation Toolkit/OpenAI/Chat-GPT AI")]
    public class ChatGPT : MonoBehaviour
    {


        public event Action OnPartResponseReceived;




        #region Variables

        [Tooltip(OpenAIDefinitions.whatTheAiIs), TextArea(1, 10)]
        [SerializeField] public string whatTheAiIs;

        [Tooltip(OpenAIDefinitions.question), TextArea(1, 10)]
        [SerializeField] public string question;

        [Tooltip(OpenAIDefinitions.postPrompt), TextArea(1, 10)]
        [SerializeField] public string postPrompt;

        [Tooltip(OpenAIDefinitions.maxTokens), Min(1)]
        [SerializeField] public int maxTokens = 75;

        [Tooltip(OpenAIDefinitions.temperature), Range(0, 1)]
        [SerializeField] public float temperature = 0.7f;

        [Tooltip(OpenAIDefinitions.topP), Range(0, 1)]
        [SerializeField] public float topP = 1;

        [Tooltip(OpenAIDefinitions.frequencyPenalty), Range(-2, 2)]
        [SerializeField] public float frequencyPenalty = 0;

        [Tooltip(OpenAIDefinitions.presencePenalty), Range(-2, 2)]
        [SerializeField] public float presencePenalty = 0;

        [Tooltip(OpenAIDefinitions.logWebCallContent)]
        [SerializeField] public bool logWebCallContent;


        [Header("Need to Generate new AI to Change")]

        [Tooltip(OpenAIDefinitions.user)]
        [SerializeField] public string userName;

        [Tooltip(OpenAIDefinitions.model + "\n" + OpenAIDefinitions.ChatModels)]
        [SerializeField] public OpenAIEnums.ChatModels model = OpenAIEnums.ChatModels.gpt_35_turbo;

        [Tooltip(OpenAIDefinitions.moderation)]
        [SerializeField] public OpenAIEnums.Moderation moderation = OpenAIEnums.Moderation.Moderation;

        [Tooltip(OpenAIDefinitions.useMemory)]
        [SerializeField] public bool useMemory;

        [Tooltip(OpenAIDefinitions.useSentiment)]
        [SerializeField] public bool useSentiment;

        [Range(-2, 2)]
        [SerializeField] public int sentimentValue;

        public ChatGPTAI chatGPTAI
        {
            get;
            private set;
        }

        [HideInInspector] public string response;
        
        #endregion



        public async Task<OpenAIJsons.ChatResponse> MakeGTPCall()
        {
            if (chatGPTAI == null)
            {
                ResetAI();
            }

            if (chatGPTAI.OpenAiKey == "" || chatGPTAI.OpenAiKey == null || chatGPTAI.OpenAiKey == string.Empty)
            {
#if UNITY_EDITOR
                string[] strings = new string[1];
                strings = AssetDatabase.FindAssets("OpenAIKey t:OpenAIKeysSO");
                string path = AssetDatabase.GUIDToAssetPath(strings[0]);
                OpenAIKeysSO key = AssetDatabase.LoadAssetAtPath<OpenAIKeysSO>(path);
                chatGPTAI.OpenAiKey = key.openAIKey;
#else
                chatGPTAI.OpenAiKey = await GameObject.Find("OpenAI_Key_Vualt_Manager").GetComponent<KeyVualtManager>().keyVault.GetSecret();
#endif
            }

            OpenAIJsons.ChatResponse response;
            response = await chatGPTAI.CallOpenAI(model, whatTheAiIs, question, postPrompt, maxTokens, temperature, 
                topP, frequencyPenalty, presencePenalty, logWebCallContent, CallChatGPT_OnPartResponseReceived);

            this.response = response.choices[0].message.content;

            if (useSentiment)
            {
                sentimentValue = await Core.CallChatGPT.GetSentiment(question, logWebCallContent);
            }

            return response;

        }
        
        public async Task<OpenAIJsons.ChatResponse> MakeGTPCall(string question)
        {
            this.question = question;
            return await MakeGTPCall();
        }

        public void ResetAI()
        {
            chatGPTAI = new ChatGPTAI(useMemory, moderation, userName);
            response = "";
        }

        private void CallChatGPT_OnPartResponseReceived(string wholeResponse, OpenAIJsons.ChatStreamResponse newPartOfResponse)
        {
            response = wholeResponse;
            OnPartResponseReceived?.Invoke();
        }

    }
}
