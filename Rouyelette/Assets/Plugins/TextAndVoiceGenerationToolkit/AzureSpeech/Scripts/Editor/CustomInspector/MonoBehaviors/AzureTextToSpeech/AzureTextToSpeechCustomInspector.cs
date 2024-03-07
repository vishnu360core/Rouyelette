using System;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.ScriptableObjects;
using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.MonoBehaviours.Editor
{
    [CustomEditor(typeof(AzureTextToSpeech))]
    public class AzureTextToSpeechCustomInspector : UnityEditor.Editor
    {


        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;


        private PropertyField defualtValuesField;
        private TextField userInputField;
        private EnumField shortNameField;
        private EnumField localNameField;
        private EnumField displayNameField;
        private EnumField languageField;


        private AzureSpeechEnums.ShortNames shortName;
        private AzureSpeechEnums.LocalNames localName;
        private AzureSpeechEnums.DisplayNames displayName;
        private AzureSpeechEnums.languages language;



        private AzureTextToSpeech azureTextToSpeech;


        public override VisualElement CreateInspectorGUI()
        {
            azureTextToSpeech = (AzureTextToSpeech)target;
            VisualElement myInspector = new VisualElement();

            VisualTreeAsset visualTree = m_VisualTreeAsset;

            visualTree.CloneTree(myInspector);


            defualtValuesField = myInspector.Q<PropertyField>("DefualtValues");
            userInputField = myInspector.Q<TextField>("Input");
            shortNameField = myInspector.Q<EnumField>("ShortName");
            localNameField = myInspector.Q<EnumField>("LocalName");
            displayNameField = myInspector.Q<EnumField>("DisplayName");
            languageField = myInspector.Q<EnumField>("Language");



            defualtValuesField.BindProperty(serializedObject.FindProperty("defaultValues"));
            defualtValuesField.RegisterValueChangeCallback(new EventCallback<SerializedPropertyChangeEvent>(OnDefaultValuesFieldChanged));

            userInputField.value = azureTextToSpeech.input;
            userInputField.RegisterCallback<ChangeEvent<string>>(OnUserInputFieldChanged);

            shortNameField.Init(azureTextToSpeech.shortName);
            shortNameField.RegisterValueChangedCallback(i => OnNameDrdFielldChanged());
            shortName = azureTextToSpeech.shortName;

            localNameField.Init(azureTextToSpeech.localName);
            localNameField.RegisterValueChangedCallback(i => OnLocalNameDrdFieldChanged());
            localName = azureTextToSpeech.localName;

            displayNameField.Init(azureTextToSpeech.displayName);
            displayNameField.RegisterValueChangedCallback(i => OnDisplayNameDrdFieldChanged());
            displayName = azureTextToSpeech.displayName;


            languageField.Init(azureTextToSpeech.language);
            languageField.RegisterValueChangedCallback(i => OnLanguageOfSpeakerFieldChanged());
            language = azureTextToSpeech.language;


            return myInspector;
        }


        private void OnDefaultValuesFieldChanged(SerializedPropertyChangeEvent serializedProperty)
        {
            azureTextToSpeech.defaultValues = (AzureSpeechDefaultSO)serializedProperty.changedProperty.objectReferenceValue;
        }

        private void OnUserInputFieldChanged(ChangeEvent<string> eventArgs)
        {
            azureTextToSpeech.input = userInputField.value;
        }

        private void OnNameDrdFielldChanged()
        {
            shortName = (AzureSpeechEnums.ShortNames)(int)Enum.Parse(typeof(AzureSpeechEnums.ShortNames), shortNameField.value.ToString());
            localName = (AzureSpeechEnums.LocalNames)(int)Enum.Parse(typeof(AzureSpeechEnums.ShortNames), shortNameField.value.ToString());
            displayName = (AzureSpeechEnums.DisplayNames)(int)Enum.Parse(typeof(AzureSpeechEnums.ShortNames), shortNameField.value.ToString());

            localNameField.value = localName;
            displayNameField.value = displayName;

            azureTextToSpeech.shortName = shortName;
            azureTextToSpeech.localName = localName;
            azureTextToSpeech.displayName = displayName;
        }

        private void OnLocalNameDrdFieldChanged()
        {
            shortName = (AzureSpeechEnums.ShortNames)(int)Enum.Parse(typeof(AzureSpeechEnums.LocalNames), localNameField.value.ToString());
            localName = (AzureSpeechEnums.LocalNames)(int)Enum.Parse(typeof(AzureSpeechEnums.LocalNames), localNameField.value.ToString());
            displayName = (AzureSpeechEnums.DisplayNames)(int)Enum.Parse(typeof(AzureSpeechEnums.LocalNames), localNameField.value.ToString());


            shortNameField.value = shortName;
            displayNameField.value = displayName;

            azureTextToSpeech.shortName = shortName;
            azureTextToSpeech.localName = localName;
            azureTextToSpeech.displayName = displayName;
        }

        private void OnDisplayNameDrdFieldChanged()
        {
            shortName = (AzureSpeechEnums.ShortNames)(int)Enum.Parse(typeof(AzureSpeechEnums.DisplayNames), displayNameField.value.ToString());
            localName = (AzureSpeechEnums.LocalNames)(int)Enum.Parse(typeof(AzureSpeechEnums.DisplayNames), displayNameField.value.ToString());
            displayName = (AzureSpeechEnums.DisplayNames)(int)Enum.Parse(typeof(AzureSpeechEnums.DisplayNames), displayNameField.value.ToString());

            localNameField.value = localName;
            shortNameField.value = shortName;

            azureTextToSpeech.shortName = shortName;
            azureTextToSpeech.localName = localName;
            azureTextToSpeech.displayName = displayName;
        }

        private void OnLanguageOfSpeakerFieldChanged()
        {
            language = (AzureSpeechEnums.languages)Enum.Parse(typeof(AzureSpeechEnums.languages), languageField.value.ToString());

            azureTextToSpeech.language = language;
        }

    }
}
