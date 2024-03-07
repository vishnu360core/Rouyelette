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
    [AddComponentMenu("Text & Voice Generation Toolkit/OpenAI/Call Chat-GPT")]
    public class CallChatGPT : MonoBehaviour
    {


        public event Action OnPartResponseRecived;



        #region Variables

        [Tooltip(OpenAIDefinitions.whatTheAiIs)]
        [TextArea(1, 10)]
        [SerializeField] public string whatTheAiIs;
        
        [Tooltip(OpenAIDefinitions.question)]
        [TextArea(1, 10)]
        [SerializeField] public string question;
        
        [Tooltip(OpenAIDefinitions.postPrompt)]
        [TextArea(1, 10)]
        [SerializeField] public string postPrompt;
        
        [Tooltip(OpenAIDefinitions.numberOfResponses)]
        [Min(1)]
        [SerializeField] int numberOfResponses = 1;
        
        [Tooltip(OpenAIDefinitions.maxTokens)]
        [Min(1)]
        [SerializeField] public int maxTokens = 75;
        
        [Tooltip(OpenAIDefinitions.temperature)]
        [Range(0, 1)]
        [SerializeField] public float temperature = 0.7f;
        
        [Tooltip(OpenAIDefinitions.topP)]
        [Range(0, 1)] 
        [SerializeField] public float topP = 1;
        
        [Tooltip(OpenAIDefinitions.frequencyPenalty)]
        [Range(-2, 2)] 
        [SerializeField] public float frequencyPenalty = 0;
        
        [Tooltip(OpenAIDefinitions.presencePenalty)]
        [Range(-2, 2)]
        [SerializeField] public float presencePenalty = 0;

        [Tooltip(OpenAIDefinitions.logWebCallContent)]
        [SerializeField] public bool logWebCallContent;


        [Tooltip(OpenAIDefinitions.model + "\n" + OpenAIDefinitions.ChatModels)]
        [SerializeField] public OpenAIEnums.ChatModels model = OpenAIEnums.ChatModels.gpt_35_turbo;
        
        [Tooltip(OpenAIDefinitions.moderation)]
        [SerializeField] public OpenAIEnums.Moderation moderation = OpenAIEnums.Moderation.Moderation;

        [HideInInspector] public string[] response;
        #endregion




        public async Task<OpenAIJsons.ChatResponse> MakeGTPCall()
        {
            if (Core.CallChatGPT.OpenAiKey == "" || Core.CallChatGPT.OpenAiKey == null || Core.CallChatGPT.OpenAiKey == string.Empty)
            {
#if UNITY_EDITOR
                string[] strings = new string[1];
                strings = AssetDatabase.FindAssets("OpenAIKey t:OpenAIKeysSO");
                string path = AssetDatabase.GUIDToAssetPath(strings[0]);
                OpenAIKeysSO key = AssetDatabase.LoadAssetAtPath<OpenAIKeysSO>(path);
                Core.CallChatGPT.OpenAiKey = key.openAIKey;
#else
                Core.CallChatGPT.OpenAiKey = await GameObject.Find("OpenAI_Key_Vualt_Manager").GetComponent<KeyVualtManager>().keyVault.GetSecret();
#endif
            }

            this.response = new string[numberOfResponses];

            OpenAIJsons.ChatResponse response;
            response = await Core.CallChatGPT.CallOpenAI(model, whatTheAiIs, question, postPrompt, moderation, numberOfResponses, 
                maxTokens, temperature, topP, frequencyPenalty, presencePenalty, "", logWebCallContent, CallChatGPT_OnPartResponseRecived);

            for (int i = 0; i < response.choices.Length; i++)
            {
                this.response[i] = response.choices[i].message.content;
            }

            return response;
        }

        public async Task<OpenAIJsons.ChatResponse> MakeGTPCall(string question)
        {
            this.question = question;
            return await MakeGTPCall();
        }

        private void CallChatGPT_OnPartResponseRecived(string wholeResponse, OpenAIJsons.ChatStreamResponse newPartOfResponse)
        {
            response[newPartOfResponse.choices[0].index] = wholeResponse;
            OnPartResponseRecived?.Invoke();
        }


    }
}
