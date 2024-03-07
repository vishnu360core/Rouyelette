#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.ScriptableObjects.Editor
{
    [CustomEditor(typeof(OpenAIKeysSO))]
    public class OpenAIKeysSOCustomInspector : UnityEditor.Editor
    {
        private bool showRunTime = false;

        #region SerializedProperties

        SerializedProperty openAIKey;

        SerializedProperty azureKeyVaultName;
        SerializedProperty azureSecretName;
        SerializedProperty azureClientId;
        SerializedProperty azureClientSecretValue;
        SerializedProperty azureTenantId;

        #endregion

        private void OnEnable()
        {
            openAIKey = serializedObject.FindProperty("openAIKey");

            azureKeyVaultName = serializedObject.FindProperty("azureKeyVaultName");
            azureSecretName = serializedObject.FindProperty("azureSecretName");
            azureClientId = serializedObject.FindProperty("azureClientId");
            azureClientSecretValue = serializedObject.FindProperty("azureClientSecretValue");
            azureTenantId = serializedObject.FindProperty("azureTenantId");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            OpenAIKeysSO keys = (OpenAIKeysSO)target;

            EditorGUILayout.PropertyField(openAIKey);

            showRunTime = EditorGUILayout.Foldout(showRunTime, "KeySecurity", true);
            if (showRunTime)
            {
                EditorGUILayout.PropertyField(azureKeyVaultName);
                EditorGUILayout.PropertyField(azureSecretName);
                EditorGUILayout.PropertyField(azureClientId);
                EditorGUILayout.PropertyField(azureClientSecretValue);
                EditorGUILayout.PropertyField(azureTenantId);
                
                if (GUILayout.Button("Update Vault"))
                {
                    keys.OnCreateVault();
                }
            }

            serializedObject.ApplyModifiedProperties();

        }
    }
}
#endif