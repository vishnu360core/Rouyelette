using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

using DataCollector;
using UnityEngine.Playables;

public class GameController : MonoBehaviour, BoardControlInterface
{
    [Header("Managers:")]
    [SerializeField] BoardManager _boardManager;
    [SerializeField] SpinWheelManager _spinWheelManager;
    [SerializeField] WheelSlotManager _wheelSlotManager;

    [Header("UI Buttons:")]
    [SerializeField] Button _spinButton;
    [SerializeField] TMP_Text _timerText;
    [SerializeField] GameObject _loadPanel;

    string _hashCode;
    bool _isInitialized = false;

    [Header("BoardTime Delay")]
    [Range(0, 100)]
    [SerializeField] int _delay;


    [Space]
    [SerializeField] bool _dealerStatus;

    #region CURRENT_DATAS
    GameData _gameData = null;
    PlayerData _playerData = null;
    List<Bet> _currentBets = new List<Bet>();
    int _currentAmount;
    #endregion

    private void Awake()
    {
        _boardManager.callback = this;
        _gameData = null;

    }

    // Start is called before the first frame update
    void Start()
    {

        Actions.ballHit += BallGroundAction;
        Actions.ResetAction += RestAction;
        Actions.BoardSelectAction += BoardSelectAction;
        Actions.DealerSet += DealerStatusAction;
        Actions.GetGameData += GetData;
        Actions.PlayerBets += SetCurrentPlayerData;

        //Test();

        //AudioManager.Instance.SpeechAction(Speech.placeBet);

        //API Handling ...
        // APIHandler.Instance.GetSlot("https://thecrypto360.com/roulette.php", SuccessAPI, ErrorAPI);
    }

    private void SetCurrentPlayerData(List<Bet> list,int amount)
    {
        Debug.Log("Saving bet !!!!!!!!!!");

        _currentBets = list;
        _currentAmount = amount;

        SendBetData(_currentBets, _currentAmount);
    }

    void Test()
    {
        ResponseData responseData = new ResponseData();

        responseData.hash = "dnckdnfdnv";
        responseData.result = 0;
        responseData.blockNumber = 0;

        _boardManager.SetGetSlot(_wheelSlotManager.GetWheelSlot(responseData.result));

        Actions.SetBallTarget(_wheelSlotManager.GetWheelSlot(responseData.result).transform);

        // TMP_Text.text = responseData.result.ToString();

       // SaveGameStatus(CameraController.CameraSwitch.table);

        StartCoroutine(Timer());
    }

    #region GAME_STATUS
    /// <summary>
    /// Save the game data
    /// </summary>
    /// <param name="camera"></param>
    public void SaveGameStatus(GameSwitch _status)
    {
        _gameData = new GameData { status = _status};
        string jsonString = JsonUtility.ToJson(_gameData);

        StartCoroutine(Network.Instance.SaveToNet(jsonString));
    }

    /// <summary>
    /// Get the game data
    /// </summary>
    /// <param name="obj"></param>
    private void GetData(string obj)
    {
        Debug.Log("Data got >>>> " + obj);

        // GAME DATA
        try
        {
            _gameData = JsonUtility.FromJson<GameData>(obj);
            Debug.Log("Game LIVE data " + _gameData.status);


            Actions.EnablePlay(_gameData.status !=  GameSwitch.off);
            _loadPanel.SetActive(_gameData.status == GameSwitch.off);
        } 
        catch 
        {
            Debug.LogWarning("No GameData");
        }
        
        //PLAYER DATA
        try
        {
           PlayerData playerData = JsonUtility.FromJson<PlayerData>(obj);
           Debug.Log("Player got >>>" + playerData.id);
        }
        catch 
        {
            Debug.LogWarning("No PlayerData");
        }

    }
    #endregion

    #region CLIENT_STATUS

    void SendBetData(List<Bet> bets,int amount)
    {
        PlayerData playerData = new PlayerData
        {
            id = Network.Instance.Id,
            bets = bets,
            amount = amount
        };

        string jsonString = JsonUtility.ToJson(playerData);
        Debug.Log("Player json :  " + jsonString);

       StartCoroutine(Network.Instance.SaveToNet(jsonString));
    }

    #endregion

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
        yield return new WaitForSeconds(3.0f);
        yield return new WaitUntil(()=> !_loadPanel.activeInHierarchy);

        Actions.DealerSet(false);
        AudioManager.Instance.SpeechAction(Speech.placeBet);
        Actions.EnablePlay(true);

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

        SaveGameStatus(GameSwitch.on);
        Test();
        //APIHandler.Instance.GetSlot("https://thecrypto360.com/roulette.php", SuccessAPI, ErrorAPI);
    }

    void BallGroundAction()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.ballHit);
    }

    public void SpinButtonAction()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFX.select);
        SpinWheelAction();  
    }

    void SpinWheelAction()
    {
        CameraController.Instance.CameraSwitchAction(CameraController.CameraSwitch.wheel);

        SaveGameStatus(GameSwitch.off);

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