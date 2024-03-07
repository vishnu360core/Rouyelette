#if UNITY_EDITOR

using TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Core;

using UnityEditor;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.AzureSpeech.Editor
{
    public class DebugAllMicNames : UnityEditor.EditorWindow
    {
        [MenuItem("Tools /Text and Voice Generation Toolkit/Azure Speech/Log All Mic Names")]
        public static void ShowWindow()
        {
            MicrophoneUtils.LogAllMicrophoneNames();
        }
    }
}
#endif