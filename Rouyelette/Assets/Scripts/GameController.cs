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
    [SerializeField] WheelSlotManager _wheelSlotManager;

    [Header("UI Buttons:")]
    [SerializeField] Button _spinButton;


    string _hashCode;
    bool _isInitialized = false;


    private void Awake()
    {
        _boardManager.callback = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Actions.ballHit += BallGroundAction;
        Actions.ResetAction += RestAction;

        //API Handling ...
        APIHandler.Instance.GetSlot("https://hf-game-call-mainnet.vercel.app/api/roulette", SuccessAPI, ErrorAPI);
    }

    void SuccessAPI(string response)
    {
        Debug.Log("Response >>>" + response);

        ResponseData responseData = JsonUtility.FromJson<ResponseData>(response);

        Debug.Log(responseData.result + ">>>" + responseData.hash);
        
         _boardManager.SetGetSlot(_wheelSlotManager.GetWheelSlot(responseData.result));

        Actions.SetBallTarget(_wheelSlotManager.GetWheelSlot(responseData.result).transform);
    }

    void ErrorAPI(string response)
    {
        Debug.LogError("Response >>>" + response);
    }

    private void RestAction()
    {
        CameraController.Instance.CameraSwitchAction(CameraController.CameraSwitch.table);
        APIHandler.Instance.GetSlot("https://hf-game-call-mainnet.vercel.app/api/roulette", SuccessAPI, ErrorAPI);
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