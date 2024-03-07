#if UNITY_EDITOR
using System;
using System.IO;
using System.Threading.Tasks;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.ScriptableObjects;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Editor
{
    public class GenerateAudioClipEditorWindow : EditorWindow
    {


        #region Variables
        [Header("UI Toolkit")]
        private const string USER_INPUT_NAME = "UserInput";
        private const string NAME_NAME = "Name";
        private const string LOCAL_NAME_NAME = "LocalName";
        private const string DISPLAY_NAME_NAME = "DisplayName";
        private const string LANGUAGE_OF_SPEAKER_NAME = "Language";
        private const string GET_AUDIO_NAME = "GetAudio";
        private const string PLAY_AUDIO_NAME = "PlayAudio";
        private const string SAVE_AUDIO_NAME = "SaveAudio";
        private const string FILE_NAME = "FileName";
        private const string MESSAGE_NAME = "Message";


        [SerializeField] private VisualTreeAsset mainWindow;


        private TextField userInputTxtFld;
        private EnumField nameFld;
        private EnumField localNameFld;
        private EnumField DispayNameFld;
        private EnumField languageOfSpeakerFld;
        private Button getAudioBtn;
        private Button playAudioBtn;
        private Button saveAudioBtn;
        private TextField fileName;
        private TextField messageTxtFld;

        [Header("Not UI Toolkit")]
        private AudioClip audioClip;
        private AzureSpeechEnums.DisplayNames displayName;
        private AzureSpeechEnums.LocalNames localName;
        private AzureSpeechEnums.ShortNames shortName;
        private AzureSpeechEnums.languages language;
        private AzureSpeechDefaultSO defaultValues;

        #endregion


        [MenuItem("Tools/Text and Voice Generation Toolkit/Azure Speech/Generate Audio Clip")]
        public static void ShowWindow()
        {
            GenerateAudioClipEditorWindow window = (GenerateAudioClipEditorWindow)GetWindow(typeof(GenerateAudioClipEditorWindow), false, "Generate Audio Clip");
            window.minSize = new Vector2(480, 270);
        }

        private void CreateGUI()
        {

            VisualElement labelFromUXML = mainWindow.CloneTree().Q<VisualElement>("Content");
            rootVisualElement.Add(labelFromUXML);


            InitFields();

        }

        private void InitFields()
        {
            #region Getting

            userInputTxtFld = rootVisualElement.Q<TextField>(USER_INPUT_NAME);
            nameFld = rootVisualElement.Q<EnumField>(NAME_NAME);
            localNameFld = rootVisualElement.Q<EnumField>(LOCAL_NAME_NAME);
            DispayNameFld = rootVisualElement.Q<EnumField>(DISPLAY_NAME_NAME);
            languageOfSpeakerFld = rootVisualElement.Q<EnumField>(LANGUAGE_OF_SPEAKER_NAME);
            getAudioBtn = rootVisualElement.Q<Button>(GET_AUDIO_NAME);
            playAudioBtn = rootVisualElement.Q<Button>(PLAY_AUDIO_NAME);
            saveAudioBtn = rootVisualElement.Q<Button>(SAVE_AUDIO_NAME);
            fileName = rootVisualElement.Q<TextField>(FILE_NAME);
            messageTxtFld = rootVisualElement.Q<TextField>(MESSAGE_NAME);

            #endregion

            #region Enums

            string[] strings = AssetDatabase.FindAssets("Azure Speech Default Values t:AzureSpeechDefaultSO");
            string path = AssetDatabase.GUIDToAssetPath(strings[0]);
            defaultValues = AssetDatabase.LoadAssetAtPath<AzureSpeechDefaultSO>(path);

            nameFld.Init(AzureSpeechEnums.ShortNames.en_US_AIGenerate1Neural);
            nameFld.RegisterValueChangedCallback(i => OnNameDrdFldChanged());
            shortName = AzureSpeechEnums.ShortNames.en_US_AIGenerate1Neural;

            localNameFld.Init(AzureSpeechEnums.LocalNames.AIGenerate1);
            localNameFld.RegisterValueChangedCallback(i => OnLocalNameDrdFldChanged());
            localName = AzureSpeechEnums.LocalNames.AIGenerate1;

            DispayNameFld.Init(AzureSpeechEnums.DisplayNames.AIGenerate1);
            DispayNameFld.RegisterValueChangedCallback(i => OnDisplayNameDrdFldChanged());
            displayName = AzureSpeechEnums.DisplayNames.AIGenerate1;

            languageOfSpeakerFld.Init(defaultValues.language);
            languageOfSpeakerFld.RegisterValueChangedCallback(i => OnLanguageOfSpeakerFldChanged());
            language = defaultValues.language;

            #endregion

            #region Buttons

            getAudioBtn.clicked += GetAudio_Click;

            playAudioBtn.clicked += PlayAudioBtn_clicked;

            saveAudioBtn.clicked += SaveAudioBtn_clicked;

            #endregion

        }

        private void SaveAudioBtn_clicked()
        {
            messageTxtFld.value = "Saving";
            if (Directory.Exists(Application.dataPath + "\\Audio\\"))
            {
                if (Directory.Exists(Application.dataPath + "\\Audio\\" + fileName.value + ".wav"))
                {
                    messageTxtFld.value = "Feald becouse there is already a file with this name.";
                    return;
                }
                AudioConverter.ConvertAudioClipToWavFile(Application.dataPath + "\\Audio\\" + fileName.value + ".wav", audioClip);
            }
            else
            {
                messageTxtFld.value = "Creating directory";
                Directory.CreateDirectory(Application.dataPath + "\\Audio\\");
                AudioConverter.ConvertAudioClipToWavFile(Application.dataPath + "\\Audio\\" + fileName.value + ".wav", audioClip);
            }
            AssetDatabase.Refresh();
        }

        private async void PlayAudioBtn_clicked()
        {
            // Making new Temp gamobject
            GameObject audioObject = new GameObject("Temp Audio Object")
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            // Adding the audio source
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();

            // Waiting time before destorying the audio clip
            await Task.Delay(Mathf.RoundToInt(1000 * audioClip.length));
            DestroyImmediate(audioObject);
            
        }

        private void GetAudio_Click()
        {
            CallAzureTextToSpeech();
        }

        private void OnNameDrdFldChanged()
        {
            shortName = (AzureSpeechEnums.ShortNames)(int)Enum.Parse(typeof(AzureSpeechEnums.ShortNames), nameFld.value.ToString());
            localName = (AzureSpeechEnums.LocalNames)(int)Enum.Parse(typeof(AzureSpeechEnums.ShortNames), nameFld.value.ToString());
            displayName = (AzureSpeechEnums.DisplayNames)(int)Enum.Parse(typeof(AzureSpeechEnums.ShortNames), nameFld.value.ToString());

            localNameFld.value = localName;
            DispayNameFld.value = displayName;
        }

        private void OnLocalNameDrdFldChanged()
        {
            shortName = (AzureSpeechEnums.ShortNames)(int)Enum.Parse(typeof(AzureSpeechEnums.LocalNames), localNameFld.value.ToString());
            localName = (AzureSpeechEnums.LocalNames)(int)Enum.Parse(typeof(AzureSpeechEnums.LocalNames), localNameFld.value.ToString());
            displayName = (AzureSpeechEnums.DisplayNames)(int)Enum.Parse(typeof(AzureSpeechEnums.LocalNames), localNameFld.value.ToString());


            nameFld.value = shortName;
            DispayNameFld.value = displayName;
        }

        private void OnDisplayNameDrdFldChanged()
        {
            shortName = (AzureSpeechEnums.ShortNames)(int)Enum.Parse(typeof(AzureSpeechEnums.DisplayNames), DispayNameFld.value.ToString());
            localName = (AzureSpeechEnums.LocalNames)(int)Enum.Parse(typeof(AzureSpeechEnums.DisplayNames), DispayNameFld.value.ToString());
            displayName = (AzureSpeechEnums.DisplayNames)(int)Enum.Parse(typeof(AzureSpeechEnums.DisplayNames), DispayNameFld.value.ToString());

            localNameFld.value = localName;
            nameFld.value = shortName;
        }

        private void OnLanguageOfSpeakerFldChanged()
        {
            language = (AzureSpeechEnums.languages)Enum.Parse(typeof(AzureSpeechEnums.languages), languageOfSpeakerFld.value.ToString());
        }

        private async void CallAzureTextToSpeech()
        {
            messageTxtFld.value = "Waiting for audio to be generated.";

            string[] pathArray  = AssetDatabase.FindAssets("Azure Speech Key t:AzureSpeechKeysSO");
            string path = AssetDatabase.GUIDToAssetPath(pathArray[0]);
            AzureSpeechKeysSO key = AssetDatabase.LoadAssetAtPath<AzureSpeechKeysSO>(path);

            TextToSpeech textToSpeech = new TextToSpeech(language, defaultValues.region, key.subscriptionKey);
            audioClip = await textToSpeech.CallAzure(userInputTxtFld.value, localName);

            messageTxtFld.value = "The audio has sucsesfully been generated.";
        }


    }
}
#endif
