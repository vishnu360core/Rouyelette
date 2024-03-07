#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CallChatGPT))]
    public class CallChatGPTCustomInspector : UnityEditor.Editor
    {
        private bool showResponses = false;

        #region SerializedProperties

        private SerializedProperty whatTheAiIs;
        private SerializedProperty question;
        private SerializedProperty postPrompt;
        private SerializedProperty numberOfResponses;
        private SerializedProperty maxTokens;
        private SerializedProperty temperature;
        private SerializedProperty topP;
        private SerializedProperty frequencyPenalty;
        private SerializedProperty presencePenalty;
        private SerializedProperty logWebCallContent;
        private SerializedProperty model;
        private SerializedProperty moderation;

        #endregion

        private void OnEnable()
        {
            ((CallChatGPT)target).OnPartResponseRecived += () => Repaint();

            whatTheAiIs = serializedObject.FindProperty("whatTheAiIs");
            question = serializedObject.FindProperty("question");
            postPrompt = serializedObject.FindProperty("postPrompt");
            numberOfResponses = serializedObject.FindProperty("numberOfResponses");
            maxTokens = serializedObject.FindProperty("maxTokens");
            temperature = serializedObject.FindProperty("temperature");
            topP = serializedObject.FindProperty("topP");
            frequencyPenalty = serializedObject.FindProperty("frequencyPenalty");
            presencePenalty = serializedObject.FindProperty("presencePenalty");
            logWebCallContent = serializedObject.FindProperty("logWebCallContent");
            model = serializedObject.FindProperty("model");
            moderation = serializedObject.FindProperty("moderation");
        }

        private void OnDisable()
        {
            ((CallChatGPT)target).OnPartResponseRecived -= () => Repaint();
        }

        public override async void OnInspectorGUI()
        {
            serializedObject.Update();

            CallChatGPT chatGPTCall = (CallChatGPT)target;

            if (GUILayout.Button("Go to open AI"))
            {
                Application.OpenURL("https://platform.openai.com/overview");
            }

            EditorGUILayout.PropertyField(whatTheAiIs);
            EditorGUILayout.PropertyField(question);
            EditorGUILayout.PropertyField(postPrompt);
            EditorGUILayout.PropertyField(numberOfResponses);
            EditorGUILayout.PropertyField(maxTokens);
            EditorGUILayout.PropertyField(temperature);
            EditorGUILayout.PropertyField(topP);
            EditorGUILayout.PropertyField(frequencyPenalty);
            EditorGUILayout.PropertyField(presencePenalty);
            EditorGUILayout.PropertyField(logWebCallContent);
            EditorGUILayout.PropertyField(model);
            EditorGUILayout.PropertyField(moderation);

            if (GUILayout.Button("Generate Response"))
            {
                await chatGPTCall.MakeGTPCall();
                showResponses = true;
            }

            serializedObject.ApplyModifiedProperties();

            if (chatGPTCall.response == null) return;
            if (chatGPTCall.response.Length == 0) return;

            try
            {
                showResponses = EditorGUILayout.Foldout(showResponses, "Responses", true);
                if (showResponses == true)
                {
                    for (int responseNumber = 0; responseNumber < chatGPTCall.response.Length; responseNumber++)
                    {
                        EditorGUILayout.LabelField("Response " + (responseNumber + 1));
                        GUILayout.TextArea(chatGPTCall.response[responseNumber]);
                    }
                }
            }
            catch
            {

            }
        }

    }
}

#endif
