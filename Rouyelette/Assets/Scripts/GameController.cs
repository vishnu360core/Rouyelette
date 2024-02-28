using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameController : MonoBehaviour, BoardControlInterface
{
    [Header("Managers:")]
    [SerializeField] BoardManager _boardManager;
    [SerializeField] SpinWheelManager _spinWheelManager;

    [Header("UI Buttons:")]
    [SerializeField] Button _spinButton;


    private void Awake()
    {
        _boardManager.callback = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Actions.ballHit += BallGroundAction;
        Actions.ResetAction += RestAction;
    }

    private void RestAction()
    {
        CameraController.Instance.CameraSwitchAction(CameraController.CameraSwitch.table);
    }

    void BallGroundAction()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.ballHit);
    }


    public void SpinButtonAction()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.select);
        CameraController.Instance.CameraSwitchAction(CameraController.CameraSwitch.wheel);

        StartCoroutine(SpinWheelAction());  

        EnableSpin(false);
    }

    IEnumerator SpinWheelAction()
    {
        yield return new WaitUntil(() => CameraController.Instance.Reached());
        _spinWheelManager.SpinAction();
    }

    /// <summary>
    /// Enable the Spin Button
    /// </summary>
    /// <param name="enable"></param>
    public void EnableSpin(bool enable)
    {
        Color32 color = _spinButton.GetComponent<Image>().color;

        _spinButton.enabled = enable;

        _spinButton.GetComponent<Image>().color = enable ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 100);

    }
}