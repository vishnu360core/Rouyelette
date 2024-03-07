 #if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GetSentiment))]
    public class GetSentimentCustomInspector : UnityEditor.Editor
    {
        #region SerializedProperties

        private SerializedProperty question;
        private SerializedProperty sentimentValue;
        private SerializedProperty logWebCallContent;

        #endregion

        private void OnEnable()
        {
            question = serializedObject.FindProperty("question");
            sentimentValue = serializedObject.FindProperty("sentimentValue");
            logWebCallContent = serializedObject.FindProperty("logWebCallContent");
        }

        public override async void OnInspectorGUI()
        {
            serializedObject.Update();

            GetSentiment getSentiment = (GetSentiment)target;

            if (GUILayout.Button("Go to open AI"))
            {
                Application.OpenURL("https://platform.openai.com/overview");
            }

            EditorGUILayout.PropertyField(question);
            EditorGUILayout.PropertyField(sentimentValue);
            EditorGUILayout.PropertyField(logWebCallContent);

            if (GUILayout.Button("Generate Response"))
            {
                await getSentiment.MakeGTPCall();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
