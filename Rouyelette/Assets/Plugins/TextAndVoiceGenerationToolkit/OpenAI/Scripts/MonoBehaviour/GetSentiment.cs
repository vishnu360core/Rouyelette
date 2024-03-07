using System;
using System.Threading.Tasks;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.ScriptableObjects;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureKeyVault.MonoBehaviours;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Core;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours
{
    [AddComponentMenu("Text & Voice Generation Toolkit/OpenAI/Get Sentiment")]
    public class GetSentiment : MonoBehaviour
    {


        [Tooltip(OpenAIDefinitions.question), TextArea(1, 10)]
        [SerializeField] public string question;

        [Range(-2f, 2f)]
        [SerializeField] public int sentimentValue;

        [Tooltip(OpenAIDefinitions.logWebCallContent)]
        [SerializeField] public bool logWebCallContent;



        public async Task<int> MakeGTPCall()
        {

            if (Core.CallChatGPT.OpenAiKey == "" || Core.CallChatGPT.OpenAiKey == null || Core.CallChatGPT.OpenAiKey == string.Empty)
            {
#if UNITY_EDITOR
                string[] strings = new string[1];
                strings = AssetDatabase.FindAssets("OpenAIKey");
                string path = AssetDatabase.GUIDToAssetPath(strings[0]);
                OpenAIKeysSO key = AssetDatabase.LoadAssetAtPath<OpenAIKeysSO>(path);
                Core.CallChatGPT.OpenAiKey = key.openAIKey;
#else
                Core.CallChatGPT.OpenAiKey = await GameObject.Find("OpenAI_Key_Vualt_Manager").GetComponent<KeyVualtManager>().keyVault.GetSecret();
#endif
            }

            sentimentValue = await Core.CallChatGPT.GetSentiment(question, logWebCallContent);

            return sentimentValue;
        }

    }
}
