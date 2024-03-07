using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.MonoBehaviours;
using UnityEngine;
using UnityEngine.UI;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Examples
{
    public class King : MonoBehaviour
    {
        [SerializeField] private float timeTillCanResend = 30f;

        private bool responding;
        private float timeTillCanTalkAgain;
        private float timer = 0f;
        private string message1 = "I have lost my daughter, and I can't find her can you please find her?";
        private Text text;
        private Reworder reworder;

        void Start()
        {
            reworder = GetComponent<Reworder>();
            text = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        }

        private void Update()
        {
            timeTillCanTalkAgain -= Time.deltaTime;

            if (responding)
            {
                if (reworder.response[0] == null || reworder.response[0] == "" || reworder.response[0] == string.Empty)
                {
                    text.fontSize = 200;
                    timer += Time.deltaTime;
                    switch ((int)(timer * 3f) % 3)
                    {
                        case 0:
                            text.text = ".";
                            break;
                        case 1:
                            text.text = "..";
                            break;
                        case 2:
                            text.text = "...";
                            break;
                    }
                    return;
                }

                text.fontSize = 80;
                text.text = reworder.response[0];
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            StartTalking();
        }

        private async void StartTalking()
        {
            if (timeTillCanTalkAgain < 0)
            {
                responding = true;
                timeTillCanTalkAgain = timeTillCanResend; 
                reworder.rewordTheFallowing = message1;
                OpenAIJsons.ChatResponse chatResponse = await reworder.MakeGTPCall();
                responding = false;

                timeTillCanTalkAgain = timeTillCanResend; 
                text.text = chatResponse.choices[0].message.content;
            }
        }
    }
}
