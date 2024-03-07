using UnityEditor;

using UnityEngine;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.ScriptableObjects.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AzureSpeechKeysSO))]
    public class AzureSpeechKeysSOCustomInspector : UnityEditor.Editor
    {


        private bool showRunTime = false;


        #region SerializedProperties

        private SerializedProperty subscriptionKey;


        private SerializedProperty azureKeyVaultName;
        private SerializedProperty azureSecretName;
        private SerializedProperty azureClientId;
        private SerializedProperty azureClientSecretValue;
        private SerializedProperty azureTenantId;

        #endregion


        private void OnEnable()
        {
            subscriptionKey = serializedObject.FindProperty("subscriptionKey");
            azureKeyVaultName = serializedObject.FindProperty("azureKeyVaultName");
            azureSecretName = serializedObject.FindProperty("azureSecretName");
            azureClientId = serializedObject.FindProperty("azureClientId");
            azureClientSecretValue = serializedObject.FindProperty("azureClientSecretValue");
            azureTenantId = serializedObject.FindProperty("azureTenantId");
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            AzureSpeechKeysSO keys = (AzureSpeechKeysSO)target;

            EditorGUILayout.PropertyField(subscriptionKey);

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
