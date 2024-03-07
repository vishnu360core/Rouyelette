using UnityEngine;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureKeyVault.Core;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureKeyVault.MonoBehaviours
{
    public class KeyVualtManager : MonoBehaviour
    {


        [SerializeField] private string azureKeyVaultName;
        [SerializeField] private string azureSecretName;
        [SerializeField] private string azureClientId;
        [SerializeField] private string azureClientSecretValue;
        [SerializeField] private string azureTenantId;


        public KeyVault keyVault;


        private void Start()
        {
            keyVault.vaultName = azureKeyVaultName;
            keyVault.secretName = azureSecretName;
            keyVault.clientId = azureClientId;
            keyVault.clientSecret = azureClientSecretValue;
            keyVault.tenantId = azureTenantId;
        }
    }
}
