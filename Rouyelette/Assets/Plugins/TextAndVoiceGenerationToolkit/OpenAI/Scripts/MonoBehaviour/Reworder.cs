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
    [AddComponentMenu("Text & Voice Generation Toolkit/OpenAI/Reworder")]
    public class Reworder : MonoBehaviour
    {


        public event Action OnPartResponseRecived;




        #region Variabes

        [Tooltip("Describe what the AI is. EX: You are a black smith from a fantisy world.")]
        [TextArea(1, 10), SerializeField] public string makeSpeachSoundLike;

        [Tooltip("This is the question the player asks"), TextArea(1, 10)]
        [SerializeField] public string rewordTheFallowing;

        [Tooltip("These are the words the AI will Keep. put a coma, and a space after every word but the last word")]
        [TextArea(1, 10), SerializeField] public string wordsToKeep;
        [SerializeField] public bool giveMultipleResponses;

        [Tooltip(OpenAIDefinitions.numberOfResponses), Min(1)]
        [SerializeField] public int numberOfResponses = 1;

        [Tooltip(OpenAIDefinitions.maxTokens), Min(1)]
        [SerializeField] public int maxTokens = 75;

        [Tooltip(OpenAIDefinitions.temperature), Range(0, 1)]
        [SerializeField] public float temperature = 0.2f;

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

        [Tooltip(OpenAIDefinitions.model + "\n" + OpenAIDefinitions.ChatModels)]
        [SerializeField] public OpenAIEnums.ChatModels model = OpenAIEnums.ChatModels.gpt_35_turbo;

        [Tooltip(OpenAIDefinitions.moderation)]
        [SerializeField] public OpenAIEnums.Moderation moderation = OpenAIEnums.Moderation.Moderation;

        [SerializeField] public string[] response;


        private float timeSinceLastCall = 0;
        private float timeSinceLastCallMax = 5;
        private bool canSend = true;


        #endregion



        private void Update()
        {
            if (!canSend)
            {
                timeSinceLastCall += Time.time;
            }

            if (timeSinceLastCall > timeSinceLastCallMax)
            {
                canSend = true;
                timeSinceLastCall = 0;
            }
        }

        public async Task<OpenAIJsons.ChatResponse> MakeGTPCall()
        {
            if (!canSend && Application.isPlaying)
            {
                Debug.Log("You can not send a request, because the time before another request can be sent has not elapsed yet. to learn more go to the documentation.", this);
            }

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

            OpenAIJsons.ChatResponse response = Core.CallChatGPT.defaultResponse;
            response = await Core.CallChatGPT.CallOpenAI(model, makeSpeachSoundLike + ". Reword this the fallowing to so sound more like you:",
                rewordTheFallowing, ".\nKeep these exact words: " + wordsToKeep, moderation, numberOfResponses, maxTokens, temperature,
                topP, frequencyPenalty, presencePenalty, userName, logWebCallContent, CallChatGPT_OnPartResponseRecived);

            for (int i = 0; i < this.response.Length; i++)
            {
                this.response[i] = response.choices[i].message.content;
            }

            if (Application.isPlaying)
            {
                canSend = false;
            }

            return response;
        }

        private void CallChatGPT_OnPartResponseRecived(string wholeResponse, OpenAIJsons.ChatStreamResponse newPartOfResponse)
        {
            response[newPartOfResponse.choices[0].index] = wholeResponse;
            OnPartResponseRecived?.Invoke();
        }


    }
}
