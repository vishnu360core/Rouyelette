using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;

public class Loader : MonoBehaviour
{

    [SerializeField] Slider slider;

    public void SliderValueAction(float value)
    {
        slider.DOValue(value, 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneController.Instance.LoadGameScene(this);
    }

}
