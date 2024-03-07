#if UNITY_EDITOR

using UnityEditor;

using UnityEngine;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.ScriptableObjects;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Editor
{
    public class CodeGPTWindow : EditorWindow
    {
        public enum CodeGPTOptions
        {
            Fix = 0,
            Make  = 1
        }

        public CodeGPTOptions option;
        private CodeGPTOptions lastOption;

        private string message;
        private OpenAIJsons.ChatResponse response;
        private string openAIResponse = "";

        [MenuItem("Tools/Text and Voice Generation Toolkit/Open AI/CodeGPT")]
        public static void ShowWindow()
        {
            GetWindow<CodeGPTWindow>("Code GPT");
        }

        private async void OnGUI()
        {
            GUIStyle userStyle = new GUIStyle(EditorStyles.textField);
            userStyle.wordWrap = true;
            userStyle.fontSize = 14;

            GUIStyle botStyle = new GUIStyle(EditorStyles.textField);
            botStyle.wordWrap = true;
            botStyle.fontSize = 18;


            option = (CodeGPTOptions)EditorGUILayout.EnumPopup("CodeGPT Type:", option);

            if (option == CodeGPTOptions.Fix)
            {
                GUILayout.Label("Put in Code that you want help with.");

                if (openAIResponse == "")
                {
                    message = EditorGUILayout.TextArea(message, userStyle, GUILayout.ExpandHeight(true));
                }

                
                if (lastOption == option)
                {
                    if (openAIResponse != "")
                    {
                        message = EditorGUILayout.TextArea(message, userStyle, GUILayout.Height(100));
                        EditorGUILayout.TextArea(openAIResponse, botStyle, GUILayout.ExpandHeight(true));
                    }
                }
                
                if (GUILayout.Button("Start checking"))
                {
                    string[] strings;
                    strings = AssetDatabase.FindAssets("OpenAIKey t:OpenAIKeysSO");
                    string path = AssetDatabase.GUIDToAssetPath(strings[0]);
                    OpenAIKeysSO key = AssetDatabase.LoadAssetAtPath<OpenAIKeysSO>(path);
                    CallChatGPT.OpenAiKey = key.openAIKey;

                    lastOption = option;
                    response = await CallChatGPT.CallOpenAI(OpenAIEnums.ChatModels.gpt_35_turbo, "You are a Senior Unity Game Developer, that likes to Make unity scripts, and following optimized code principles.",
                        "Correct this code: " + message, "", OpenAIEnums.Moderation.NoModeration, 1, 1024, 0f, 1, 0, 0, "TheAshBotAssets", OnPartOfResponseReceived);

                }
            }
            else if (option == CodeGPTOptions.Make)
            {
                GUILayout.Label("tell the AI what you want it to make.");

                message = EditorGUILayout.TextArea(message, userStyle, GUILayout.Height(50));

                if (lastOption == option)
                {
                    if (openAIResponse != "")
                    {
                        EditorGUILayout.TextArea(openAIResponse, botStyle, GUILayout.ExpandHeight(true));
                    }
                }

                if (GUILayout.Button("Make Code"))
                {
                    string[] strings = AssetDatabase.FindAssets("OpenAIKey t:OpenAIKeysSO");
                    string path = AssetDatabase.GUIDToAssetPath(strings[0]);
                    OpenAIKeysSO key = AssetDatabase.LoadAssetAtPath<OpenAIKeysSO>(path);
                    CallChatGPT.OpenAiKey = key.openAIKey;


                    lastOption = option;
                    response = await CallChatGPT.CallOpenAI(OpenAIEnums.ChatModels.gpt_35_turbo, "You are a Senior Unity Game Developer,that makes others scripts, and follows optimized code principles.",
                        "Make a script that dose the following: " + message, "", OpenAIEnums.Moderation.NoModeration, 1, 1024, 0f, 1, 0, 0, "TheAshBotAssets", OnPartOfResponseReceived);
                }
            }
        }

        private void OnPartOfResponseReceived(string wholeResponse, OpenAIJsons.ChatStreamResponse newPartOfResponse)
        {
            Repaint();
            openAIResponse = wholeResponse;
        }
    }
}
#endif
