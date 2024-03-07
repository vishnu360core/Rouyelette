using UnityEngine;
using UnityEngine.UI;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Examples
{
    public class AI : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Sprite[] sprites;
        
        private int SentimentValue;
        private SpriteRenderer spriteRenderer;
        private GetSentiment getSentiment;

        private void Start()
        {
            getSentiment = GetComponent<GetSentiment>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public async void OnSendToAI()
        {
            getSentiment.question = inputField.text;
            SentimentValue = await getSentiment.MakeGTPCall();

            spriteRenderer.sprite = sprites[SentimentValue + 2];
        }
    }
}
