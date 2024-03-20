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


    [Space]
    [SerializeField] bool _dealerStatus;


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
        Actions.DealerSet += DealerStatusAction;

        //AudioManager.Instance.SpeechAction(Speech.placeBet);

        //API Handling ...
        APIHandler.Instance.GetSlot("https://thecrypto360.com/roulette.php", SuccessAPI, ErrorAPI);
    }

    /// <summary>
    /// Dealer Status information
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void DealerStatusAction(bool obj)
    {
        _dealerStatus = obj;
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


        Actions.DealerSet(false);
        AudioManager.Instance.SpeechAction(Speech.placeBet);
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

        AudioManager.Instance.SpeechAction(Speech.NoMoreBet);
        SpinButtonAction();
    }

    public void BetProducedAction(int number, Slot.ColorType colorType)
    {
        Debug.LogWarning(number + ": " + colorType.ToString());

        string _iseven = number % 2 == 0 ? "even" : "odd";  

        AudioManager.Instance.SpeechAction(Speech.number,number);
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

        StartCoroutine(ResetAction());
    }

    IEnumerator ResetAction()
    {
        yield return null;
        yield return new WaitUntil(() => _dealerStatus);
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
        yield return null;
        CameraController.Instance.CameraSwitchAction(CameraController.CameraSwitch.wheel);
        //yield return new WaitUntil(() => CameraController.Instance.Reached());

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