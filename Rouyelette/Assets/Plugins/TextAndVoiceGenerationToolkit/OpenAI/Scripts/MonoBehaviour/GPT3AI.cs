using System;
using System.Threading.Tasks;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.ScriptableObjects;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureKeyVault.MonoBehaviours;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours
{
    [Obsolete]
    [AddComponentMenu("Text & Voice Generation Toolkit/OpenAI/Depreciated/GPT-3 AI")]
    public class GPT3AI : MonoBehaviour
    {


        public event Action OnPartResponseReceived;




        #region Variables

        [Tooltip(OpenAIDefinitions.botName)]
        [SerializeField] public string botName;

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

        [Obsolete]
        [Tooltip(OpenAIDefinitions.model + "\n" + OpenAIDefinitions.Models)]
        [SerializeField] public OpenAIEnums.Models model = OpenAIEnums.Models.text_curie_001;

        [Tooltip(OpenAIDefinitions.moderation)]
        [SerializeField] public OpenAIEnums.Moderation moderation = OpenAIEnums.Moderation.Moderation;

        [Tooltip(OpenAIDefinitions.useMemory)]
        [SerializeField] public bool useMemory;

        [Tooltip(OpenAIDefinitions.useMemoryOptimizer)]
        [SerializeField] public bool useMemoryOptimizer;

        [Tooltip(OpenAIDefinitions.useSentiment)]
        [SerializeField] public bool useSentiment;

        [Range(-2, 2)]
        [SerializeField] public int sentimentValue;

        [HideInInspector] public string response;

        [Obsolete]
        public Core.GPT3AI Gpt3Ai
        {
            get; 
            private set;
        }

        #endregion


        [Obsolete]
        public async Task<OpenAIJsons.Response> MakeGTPCall()
        {
            if (Gpt3Ai == null)
            {
                ResetAI();
            }

            if (Gpt3Ai.OpenAiKey == "" || Gpt3Ai.OpenAiKey == null || Gpt3Ai.OpenAiKey == string.Empty)
            {
#if UNITY_EDITOR
                string[] strings = new string[1];
                strings = AssetDatabase.FindAssets("OpenAIKey t:OpenAIKeysSO");
                string path = AssetDatabase.GUIDToAssetPath(strings[0]);
                OpenAIKeysSO key = AssetDatabase.LoadAssetAtPath<OpenAIKeysSO>(path);
                Gpt3Ai.OpenAiKey = key.openAIKey;
                Core.CallGPT3.OpenAiKey = key.openAIKey;
#else
                Gpt3Ai.OpenAiKey = await GameObject.Find("OpenAI_Key_Vualt_Manager").GetComponent<KeyVualtManager>().keyVault.GetSecret();
                Core.CallGPT3.OpenAiKey = Gpt3Ai.OpenAiKey;
#endif
            }

            OpenAIJsons.Response response;
            response = await Gpt3Ai.CallOpenAI(model, whatTheAiIs, question, postPrompt, maxTokens, temperature, 
                topP, frequencyPenalty, presencePenalty, logWebCallContent, CallGPT3_OnPartResponseReceived);

            this.response = response.choices[0].text;
            
            if (useSentiment)
            {
                sentimentValue = await Core.CallGPT3.GetSentiment(question, logWebCallContent);
            }

            return response;
        }

        [Obsolete]
        public async Task<OpenAIJsons.Response> MakeGTPCall(string question)
        {
            this.question = question;
            return await MakeGTPCall();
        }

        [Obsolete]
        public void ResetAI()
        {
            Gpt3Ai = new Core.GPT3AI(botName, useMemory, useMemoryOptimizer, moderation, userName);
            response = "";
        }

        [Obsolete]
        private void CallGPT3_OnPartResponseReceived(string wholeResponse, OpenAIJsons.StreamResponse newPartOfResponse)
        {
            response = wholeResponse;
            OnPartResponseReceived?.Invoke();
        }


    }
}
