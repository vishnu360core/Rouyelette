#if UNITY_EDITOR

using UnityEditor;

using UnityEngine;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.ScriptableObjects;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Editor
{
    public class SpellCheckerWindow : EditorWindow
    {
        private string message;
        private OpenAIJsons.ChatResponse response;

        [MenuItem("Tools/Text and Voice Generation Toolkit/Open AI/SpellChecker")]
        public static void ShowWindow()
        {
            GetWindow<SpellCheckerWindow>("AI Spell Checker");
        }

        private async void OnGUI()
        {
            GUIStyle textFieldStyle = new GUIStyle(EditorStyles.textField);
            textFieldStyle.wordWrap = true;

            GUILayout.Label("Spell Check the fallowing: ");

            message = EditorGUILayout.TextField(message, textFieldStyle, GUILayout.ExpandHeight(true));

            string[] strings;
            strings = AssetDatabase.FindAssets("OpenAIKey t:OpenAIKeysSO");
            string path = AssetDatabase.GUIDToAssetPath(strings[0]);
            OpenAIKeysSO key = AssetDatabase.LoadAssetAtPath<OpenAIKeysSO>(path);
            CallChatGPT.OpenAiKey = key.openAIKey;

            if (CallChatGPT.Response != null)
            {
                if (CallChatGPT.Response.choices != null)
                {
                    if (CallChatGPT.Response.choices[0] != null)
                    {
                        if (response.choices[0].message.content != null)
                        {
                            EditorGUILayout.TextField(response.choices[0].message.content, textFieldStyle, GUILayout.ExpandHeight(true));
                        }
                    }
                }
            }

            if (GUILayout.Button("Start cheking"))
            {
                response = await CallChatGPT.CallOpenAI(OpenAIEnums.ChatModels.gpt_35_turbo, "", "You are a reliable spell, and grammer checker. " +
                    "You check the spelling, and grammer of all text you see and correct it. \n\n" + message, "", OpenAIEnums.Moderation.NoModeration, 1, 1024, 0f, 1, 0, 0, "TheAshBotAssets");
            }


        }

    }
}
#endif
