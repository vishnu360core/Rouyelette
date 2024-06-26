using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;

public class Loader : MonoBehaviour
{

    [SerializeField] Slider slider;

    [SerializeField] RectTransform wheel;

    public void SliderValueAction(float value)
    {
        slider.DOValue(value, 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        wheel.DORotate(new Vector3(0, 0,360),5.0f,RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);

        SceneController.Instance.LoadGameScene(this);
    }

}
