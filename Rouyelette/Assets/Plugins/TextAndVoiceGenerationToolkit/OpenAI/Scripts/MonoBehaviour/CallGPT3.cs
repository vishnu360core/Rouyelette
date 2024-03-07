using System;
using System.Threading.Tasks;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.ScriptableObjects;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours
{

    [Obsolete]
    [AddComponentMenu("Text & Voice Generation Toolkit/OpenAI/Depreciated/Call GPT-3")]
    public class CallGPT3 : MonoBehaviour
    {


        public event Action OnPartResponseReceived;



        #region Variables
        [Tooltip(OpenAIDefinitions.whatTheAiIs), TextArea(1, 10)]
        [SerializeField] public string whatTheAiIs;

        [Tooltip(OpenAIDefinitions.question), TextArea(1, 10)]
        [SerializeField] public string question;

        [Tooltip(OpenAIDefinitions.postPrompt), TextArea(1, 10)]
        [SerializeField] public string postPrompt;

        [Tooltip(OpenAIDefinitions.numberOfResponses), Min(1)]
        [SerializeField] public int numberOfResponses = 1;

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

        [Tooltip(OpenAIDefinitions.user)]
        [SerializeField] public string userName;

        [Tooltip(OpenAIDefinitions.model + "\n" + OpenAIDefinitions.Models)]
        [SerializeField] public OpenAIEnums.Models model = OpenAIEnums.Models.text_curie_001;

        [Tooltip(OpenAIDefinitions.moderation)]
        [SerializeField] public OpenAIEnums.Moderation moderation = OpenAIEnums.Moderation.Moderation;

        [HideInInspector] public string[] response;

        #endregion



        [Obsolete]
        public async Task<OpenAIJsons.Response> MakeGTPCall()
        {
            if (Core.CallGPT3.OpenAiKey == "" || Core.CallGPT3.OpenAiKey == null || Core.CallGPT3.OpenAiKey == string.Empty)
            {
#if UNITY_EDITOR
                string[] strings = new string[1];
                strings = AssetDatabase.FindAssets("OpenAIKey t:OpenAIKeysSO");
                string path = AssetDatabase.GUIDToAssetPath(strings[0]);
                OpenAIKeysSO key = AssetDatabase.LoadAssetAtPath<OpenAIKeysSO>(path);
                Core.CallGPT3.OpenAiKey = key.openAIKey;
#else
                Core.CallGPT3.OpenAiKey = await GameObject.Find("OpenAI_Key_Vualt_Manager").GetComponent<KeyVualtManager>().keyVault.GetSecret();
#endif
            }

            this.response = new string[numberOfResponses];

            OpenAIJsons.Response response;
            response = await Core.CallGPT3.CallOpenAI(model, whatTheAiIs, question, postPrompt, moderation, numberOfResponses, maxTokens, 
                temperature, topP, frequencyPenalty, presencePenalty, userName, logWebCallContent, CallGPT3_OnPartResponseReceived);

            for (int i = 0; i < response.choices.Length; i++)
            {
                this.response[i] = response.choices[i].text;
            }

            return response;
        }

        [Obsolete]
        public async Task<OpenAIJsons.Response> MakeGTPCall(string question)
        {
            this.question = question;
            return await MakeGTPCall();
        }

        private void CallGPT3_OnPartResponseReceived(string wholeResponse, OpenAIJsons.StreamResponse newPartOfResponse)
        {
            response[newPartOfResponse.choices[0].index] = wholeResponse;
            OnPartResponseReceived?.Invoke();
        }


    }

}
