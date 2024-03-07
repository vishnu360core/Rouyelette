using UnityEngine;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureKeyVault.Core;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Azure Speech Key", menuName = "Text and Voice Generation Tookit/Azure Speech/Keys")]
    public class AzureSpeechKeysSO : ScriptableObject
    {
        [SerializeField] public string subscriptionKey;

        [SerializeField] public string azureKeyVaultName;
        [SerializeField] public string azureSecretName;
        [SerializeField] public string azureClientId;
        [SerializeField] public string azureClientSecretValue;
        [SerializeField] public string azureTenantId;

        public KeyVault keyVault;


        public async void OnCreateVault()
        {
            keyVault.vaultName = azureKeyVaultName;
            keyVault.secretName = azureSecretName;
            keyVault.clientId = azureClientId;
            keyVault.clientSecret = azureClientSecretValue;
            keyVault.tenantId = azureTenantId;

            await keyVault.SetSecret(subscriptionKey);
        }
    }
}
