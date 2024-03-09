using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] TMP_Text _timerText;


    string _hashCode;
    bool _isInitialized = false;

    [Header("BoardTime Delay")]
    [Range(0, 100)]
    [SerializeField] int _delay; 


    private void Awake()
    {
        _boardManager.callback = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Actions.ballHit += BallGroundAction;
        Actions.ResetAction += RestAction;
        Actions.BoardSelectAction += BoardSelectAction;

        //API Handling ...
        APIHandler.Instance.GetSlot("https://thecrypto360.com/roulette.php", SuccessAPI, ErrorAPI);
    }

    #region API RESPONSE HANDLING
    void SuccessAPI(string response)
    {
        Debug.Log("Response >>>" + response);

        ResponseData responseData = JsonUtility.FromJson<ResponseData>(response);

        Debug.Log(responseData.result + ">>>" + responseData.hash);

        _boardManager.SetGetSlot(_wheelSlotManager.GetWheelSlot(responseData.result));

        Actions.SetBallTarget(_wheelSlotManager.GetWheelSlot(responseData.result).transform);

        // TMP_Text.text = responseData.result.ToString();

        AudioManager.Instance.SpeechAction("Place your bets please ");
        Actions.EnablePlay(true);

        StartCoroutine(Timer());
    }

    void ErrorAPI(string response)
    {
        Debug.LogError("Response >>>" + response);

        Actions.EnablePlay(false);

       // TMP_Text.text = "Error : " + response;
    }
    #endregion

    /// <summary>
    /// Timer
    /// </summary>
    /// <returns></returns>
    IEnumerator Timer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _delay)
        {

            elapsedTime += Time.deltaTime;

            float currentTimer =  _delay - elapsedTime;

            _timerText.text = "Timer :" +  Mathf.RoundToInt(currentTimer);

            yield return null;
        }

        AudioManager.Instance._isSpeechLoaded = false;
        AudioManager.Instance.SpeechAction("No more Bets");
        SpinButtonAction();
    }

    public void BetProducedAction(int number, Slot.ColorType colorType)
    {
        Debug.LogWarning(number + ": " + colorType.ToString());

        AudioManager.Instance._isSpeechLoaded = false;

        string _iseven = number % 2 == 0 ? "even" : "odd";  

        AudioManager.Instance.SpeechAction(number.ToString() + " " + colorType.ToString() + " " + _iseven);
    }

    /// <summary>
    /// Action implemented when user selects on board 
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void BoardSelectAction()
    {
        CameraController.Instance.CameraSwitchAction(CameraController.CameraSwitch.user);
    }

    private void RestAction()
    {
        CameraController.Instance.CameraSwitchAction(CameraController.CameraSwitch.table);

        APIHandler.Instance.GetSlot("https://thecrypto360.com/roulette.php", SuccessAPI, ErrorAPI);
    }

    void BallGroundAction()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.ballHit);
    }

    public void SpinButtonAction()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.select);
        StartCoroutine(SpinWheelAction());  

        EnableSpin(false);
    }

    IEnumerator SpinWheelAction()
    {
        yield return new WaitUntil(() => AudioManager.Instance._isSpeechLoaded);
        CameraController.Instance.CameraSwitchAction(CameraController.CameraSwitch.wheel);
        yield return new WaitUntil(() => CameraController.Instance.Reached());

        Debug.LogWarning("Camera switch ");
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