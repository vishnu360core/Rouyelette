#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.MonoBehaviours.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AzureSpeechToText))]
    public class AzureSpeechToTextCustomInspector : UnityEditor.Editor
    {
        
        #region SerializedProperties

        private SerializedProperty audioClip;
        private SerializedProperty text;


        private SerializedProperty defaultValues;
        private SerializedProperty languageOfSpeech;

        #endregion

        private void OnEnable()
        {
            audioClip = serializedObject.FindProperty("audioClip");
            text = serializedObject.FindProperty("text");

            defaultValues = serializedObject.FindProperty("defaultValues");
            languageOfSpeech = serializedObject.FindProperty("languageOfSpeech");
            
        }

        public override async void OnInspectorGUI()
        {
            try
            {
                serializedObject.Update();

                AzureSpeechToText azureSpeechToText = (AzureSpeechToText)target;

                EditorGUILayout.PropertyField(defaultValues);
                EditorGUILayout.PropertyField(languageOfSpeech);
                
                EditorGUILayout.Space(5f);

                EditorGUILayout.PropertyField(audioClip);

                if (GUILayout.Button("Get Text"))
                {
                    string text = await azureSpeechToText.CallAzure();
                }


                if (azureSpeechToText.text != null && azureSpeechToText.text != string.Empty)
                {
                    EditorGUILayout.PropertyField(text);
                }

                serializedObject.ApplyModifiedProperties();
            }
            catch { }
        }

    }
}
#endif