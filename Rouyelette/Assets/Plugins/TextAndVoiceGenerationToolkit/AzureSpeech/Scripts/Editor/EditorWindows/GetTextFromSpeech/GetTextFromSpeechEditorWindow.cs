using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.ScriptableObjects;
using UnityEditor.UIElements;
using System;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Editor
{
    public class GetTextFromSpeechEditorWindow : UnityEditor.EditorWindow
    {


        [SerializeField] private VisualTreeAsset mainWindow;


        private float BaseFontSize
        {
            get
            {
                return position.height < position.width ? position.height : position.width;
            }
        }


        [Header("UI Toolkit")]
        private Label languageLabel;
        private EnumField languageEnum;
        private Button recordAudio;
        private Button playAudioBtn;
        private Button getTextBtn;
        private TextField textFld;
        private TextField consoleFld;


        [Header("Not UI Toolkit")]
        private bool isRecording;
        private PopupField<string> microphoneNamesPopup;
        private AudioClip audioFromMicrophone;
        private MicrophoneUtils microphone;
        private AzureSpeechEnums.languages language;

        [Header("ScriptableObjects")]
        private AzureSpeechDefaultSO defaultValues;
        private AzureSpeechKeysSO key;



        [MenuItem("Tools /Text and Voice Generation Toolkit/Azure Speech/Get Text From Speech")]
        public static void ShowWindow()
        {
            GetTextFromSpeechEditorWindow window = (GetTextFromSpeechEditorWindow)GetWindow(typeof(GetTextFromSpeechEditorWindow), false, "Get Text From Speech");
            window.minSize = new Vector2(250, 150);
        }


        private void CreateGUI()
        {
            microphoneNamesPopup = new PopupField<string>(MicrophoneUtils.micNames.ToList(), 0);
            rootVisualElement.Add(microphoneNamesPopup);

            VisualElement mainVisualElement = mainWindow.CloneTree().Q<VisualElement>("Content");
            rootVisualElement.Add(mainVisualElement);

            InitFields();

            ResizeText();
        }

        private void OnGUI()
        {
            ResizeText();
        }

        private void InitFields()
        {

            #region ScriptableObjects

            string[] strings = AssetDatabase.FindAssets("Azure Speech Default Values t:AzureSpeechDefaultSO");
            string path = AssetDatabase.GUIDToAssetPath(strings[0]);
            defaultValues = AssetDatabase.LoadAssetAtPath<AzureSpeechDefaultSO>(path);

            strings = AssetDatabase.FindAssets("Azure Speech Key t:AzureSpeechKeysSO");
            path = AssetDatabase.GUIDToAssetPath(strings[0]);
            key = AssetDatabase.LoadAssetAtPath<AzureSpeechKeysSO>(path);

            #endregion


            #region UI Toolkit

            languageLabel = rootVisualElement.Q<Label>("Language");
            languageEnum = rootVisualElement.Q<EnumField>("LanguageEnum");
            recordAudio = rootVisualElement.Q<Button>("StartRecording");
            playAudioBtn = rootVisualElement.Q<Button>("PlayAudio");
            getTextBtn = rootVisualElement.Q<Button>("GetText");
            textFld = rootVisualElement.Q<TextField>("Text");
            consoleFld = rootVisualElement.Q<TextField>("Console");

            languageEnum.Init(defaultValues.language);
            languageEnum.RegisterValueChangedCallback(i =>
            {
                language = (AzureSpeechEnums.languages)(int)Enum.Parse(typeof(AzureSpeechEnums.languages), languageEnum.value.ToString());
            });
            language = defaultValues.language;

            recordAudio.clicked += RecordAudio_clicked;
            playAudioBtn.clicked += PlayAudioBtn_clicked;
            getTextBtn.clicked += GetTextBtn_clicked;

            #endregion

        }

        private void ResizeText()
        {
            try
            {
                microphoneNamesPopup.style.fontSize = BaseFontSize / 15;
                languageLabel.style.fontSize = BaseFontSize / 15;
                languageEnum.style.fontSize = BaseFontSize / 15;
                playAudioBtn.style.fontSize = BaseFontSize / 15;
                getTextBtn.style.fontSize = BaseFontSize / 15;

                textFld.style.fontSize = BaseFontSize / 15;
                consoleFld.style.fontSize = BaseFontSize / 15;
            }
            catch
            {

            }
        }

        private void RecordAudio_clicked()
        {
            if (isRecording)
            {
                // Is Already Recording
                recordAudio.style.unityBackgroundImageTintColor = Color.black;

                audioFromMicrophone = microphone.StopRecording();

                UpdateConsole("Recording has been stopped");
            }
            else
            {
                int micIndex = 0;

                for (int i = 0; i < MicrophoneUtils.micNames.Length; i++)
                {
                    if (MicrophoneUtils.micNames[i] == microphoneNamesPopup.value)
                    {
                        micIndex = i;
                        break;
                    }
                }

                // Is Not yet recording
                recordAudio.style.unityBackgroundImageTintColor = Color.red;

                microphone = new MicrophoneUtils(micIndex);
                microphone.StartRecording();

                UpdateConsole("Recording has started");
            }

            isRecording = !isRecording;
        }

        private async void PlayAudioBtn_clicked()
        {
            if (audioFromMicrophone != null)
            {
                UpdateConsole("Audio has started to play.");

                // Making new Temp gamobject
                GameObject audioObject = new GameObject("Temp Audio Object")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };

                // Adding the audio source
                AudioSource audioSource = audioObject.AddComponent<AudioSource>();
                audioSource.clip = audioFromMicrophone;
                audioSource.Play();

                // Waiting time before destorying the audio clip
                await Task.Delay(Mathf.RoundToInt(1000 * audioFromMicrophone.length));
                DestroyImmediate(audioObject);

                UpdateConsole("Audio has finished playing");

                return;
            }
            UpdateConsole("Error: you have not recorded any audio yet.");
        }

        private void GetTextBtn_clicked()
        {
            CallAzure();
        }

        private async void CallAzure()
        {

            UpdateConsole("Please wait for azure to give a response");

            SpeechToText speechToText = new SpeechToText(language, defaultValues.region, key.subscriptionKey);

            AzureSpeechJsons.SpeechToTextResponse response = await speechToText.GetTextFromSpeech(audioFromMicrophone);

            textFld.value = response.DisplayText;

            UpdateConsole("Azure has responded");

        }

        private void UpdateConsole(string text)
        {
            consoleFld.value = text;
        }

    }
}
