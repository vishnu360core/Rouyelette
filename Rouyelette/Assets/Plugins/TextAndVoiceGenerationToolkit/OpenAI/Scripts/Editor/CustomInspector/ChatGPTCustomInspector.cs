#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ChatGPT))]
    public class ChatGPTCustomInspector : UnityEditor.Editor
    {
        private bool showResponses = true;

        #region SerializedProperties

        private SerializedProperty whatTheAiIs;
        private SerializedProperty question;
        private SerializedProperty postPrompt;
        private SerializedProperty maxTokens;
        private SerializedProperty temperature;
        private SerializedProperty topP;
        private SerializedProperty frequencyPenalty;
        private SerializedProperty presencePenalty;
        private SerializedProperty userName;
        private SerializedProperty logWebCallContent;
        private SerializedProperty model;
        private SerializedProperty moderation;
        private SerializedProperty useMemory;
        private SerializedProperty useSentiment;
        private SerializedProperty sentimentValue;

        #endregion

        private void OnEnable()
        {
            ((ChatGPT)target).OnPartResponseReceived += () => Repaint();

            whatTheAiIs = serializedObject.FindProperty("whatTheAiIs");
            question = serializedObject.FindProperty("question");
            postPrompt = serializedObject.FindProperty("postPrompt");
            maxTokens = serializedObject.FindProperty("maxTokens");
            temperature = serializedObject.FindProperty("temperature");
            topP = serializedObject.FindProperty("topP");
            frequencyPenalty = serializedObject.FindProperty("frequencyPenalty");
            presencePenalty = serializedObject.FindProperty("presencePenalty");
            userName = serializedObject.FindProperty("userName");
            logWebCallContent = serializedObject.FindProperty("logWebCallContent");
            model = serializedObject.FindProperty("model");
            moderation = serializedObject.FindProperty("moderation");
            useMemory = serializedObject.FindProperty("useMemory");
            useSentiment = serializedObject.FindProperty("useSentiment");
            sentimentValue = serializedObject.FindProperty("sentimentValue");
        }

        private void OnDisable()
        {
            ((ChatGPT)target).OnPartResponseReceived -= () => Repaint();
        }


        public override async void OnInspectorGUI()
        {
            try
            {
                serializedObject.Update();

                ChatGPT chatGPT = (ChatGPT)target;

                if (GUILayout.Button("Go to open AI"))
                {
                    Application.OpenURL("https://platform.openai.com/overview");
                }
                if (GUILayout.Button("Reset AI"))
                {
                    chatGPT.ResetAI();
                }

                EditorGUILayout.PropertyField(whatTheAiIs);
                EditorGUILayout.PropertyField(question);
                EditorGUILayout.PropertyField(postPrompt);
                EditorGUILayout.PropertyField(maxTokens);
                EditorGUILayout.PropertyField(temperature);
                EditorGUILayout.PropertyField(topP);
                EditorGUILayout.PropertyField(frequencyPenalty);
                EditorGUILayout.PropertyField(presencePenalty);
                EditorGUILayout.PropertyField(userName);
                EditorGUILayout.PropertyField(logWebCallContent);
                EditorGUILayout.PropertyField(model);
                EditorGUILayout.PropertyField(moderation);
                EditorGUILayout.PropertyField(useMemory);
                EditorGUILayout.PropertyField(useSentiment);

                if (chatGPT.useSentiment)
                {
                    EditorGUILayout.PropertyField(sentimentValue);
                }

                if (GUILayout.Button("Generate Response"))
                {
                    await chatGPT.MakeGTPCall();
                    showResponses = true;
                }

                serializedObject.ApplyModifiedProperties();

                showResponses = EditorGUILayout.Foldout(showResponses, "Conversation", true);
                if (showResponses == true)
                {
                    if (chatGPT.response == null) return;

                    if (chatGPT.chatGPTAI.UseMemory)
                    {
                        int i = 0;
                        for (int responseNumber = 0; responseNumber < chatGPT.chatGPTAI.ConversationList.Count; responseNumber++)
                        {
                            GUILayout.Space(10);
                            if (i == 0)
                            {
                                GUILayout.TextArea("Player: " + chatGPT.chatGPTAI.ConversationList[responseNumber]);
                                i++;
                            }
                            else
                            {
                                GUILayout.TextArea("AI: " + chatGPT.chatGPTAI.ConversationList[responseNumber]);
                                i = 0;
                            }
                        }
                    }
                    else
                    {
                        GUILayout.TextArea(chatGPT.response);
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
