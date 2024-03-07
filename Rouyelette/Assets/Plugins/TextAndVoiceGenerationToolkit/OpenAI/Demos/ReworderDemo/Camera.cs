using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheAshBot.Assets.TextAndVoiceGenerationToolkit.OpenAI.Examples
{
    public class Camera : MonoBehaviour
    {
        [SerializeField] private Transform fallowTransform;

        private void LateUpdate()
        {
            this.transform.position = new Vector3(fallowTransform.position.x, fallowTransform.position.y, transform.position.z);
        }
    }
}
