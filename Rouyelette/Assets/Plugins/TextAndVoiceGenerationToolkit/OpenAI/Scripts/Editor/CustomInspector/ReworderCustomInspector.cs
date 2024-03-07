using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Reworder))]
    public class ReworderCustomInspector : UnityEditor.Editor
    {
        private bool showResponses = true;

        #region SerializedProperties


        private SerializedProperty makeSpeachSoundLike;
        private SerializedProperty rewordTheFallowing;
        private SerializedProperty wordsToKeep;
        private SerializedProperty giveMultipleResponses;
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
            ((Reworder)target).OnPartResponseRecived += () => Repaint();

            makeSpeachSoundLike = serializedObject.FindProperty("makeSpeachSoundLike");
            rewordTheFallowing = serializedObject.FindProperty("rewordTheFallowing");
            wordsToKeep = serializedObject.FindProperty("wordsToKeep");
            giveMultipleResponses = serializedObject.FindProperty("giveMultipleResponses");
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
            ((Reworder)target).OnPartResponseRecived -= () => Repaint();
        }


        public override async void OnInspectorGUI()
        {
            try
            {
                serializedObject.Update();

                Reworder reworder = (Reworder)target;

                if (GUILayout.Button("Go to open AI"))
                {
                    Application.OpenURL("https://platform.openai.com/overview");
                }

                EditorGUILayout.PropertyField(makeSpeachSoundLike);
                EditorGUILayout.PropertyField(rewordTheFallowing);
                EditorGUILayout.PropertyField(wordsToKeep);
                EditorGUILayout.PropertyField(giveMultipleResponses);
                if (giveMultipleResponses.boolValue)
                {
                    EditorGUILayout.PropertyField(numberOfResponses);
                }
                else
                {
                    numberOfResponses.intValue = 1;
                }
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
                    await reworder.MakeGTPCall();
                    showResponses = true;
                }

                serializedObject.ApplyModifiedProperties();

                showResponses = EditorGUILayout.Foldout(showResponses, "Result", true);
                if (showResponses == true)
                {
                    if (reworder.response != null)
                    {
                        if (giveMultipleResponses.boolValue)
                        {
                            for (int i = 0; i < numberOfResponses.intValue; i++)
                            {
                                GUILayout.Space(10);
                                GUILayout.TextArea(reworder.response[i]);
                            }
                        }
                        else
                        {
                            GUILayout.TextArea(reworder.response[0]);
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}
