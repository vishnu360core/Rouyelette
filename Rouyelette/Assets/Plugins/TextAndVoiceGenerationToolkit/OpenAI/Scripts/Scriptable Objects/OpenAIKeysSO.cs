using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureKeyVault.Core;
using UnityEngine;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Open AI Key", menuName = "Text and Voice Generation Tookit/Open AI/Keys")]
    public class OpenAIKeysSO : ScriptableObject
    {
        [SerializeField] public string openAIKey;

        [SerializeField] public string azureKeyVaultName;
        [SerializeField] public string azureSecretName;
        [SerializeField] public string azureClientId;
        [SerializeField] public string azureClientSecretValue;
        [SerializeField] public string azureTenantId;

        private KeyVault keyVault;

        public async void OnCreateVault()
        {
            keyVault = new KeyVault();

            keyVault.vaultName = azureKeyVaultName;
            keyVault.secretName = azureSecretName;
            keyVault.clientId = azureClientId;
            keyVault.clientSecret = azureClientSecretValue;
            keyVault.tenantId = azureTenantId;

            await keyVault.SetSecret(openAIKey);
        }
    }
}
