using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

using DataCollector;
using DG.Tweening;

public class GameController : MonoBehaviour, BoardControlInterface
{
    [Header("Managers:")]
    [SerializeField] BoardManager _boardManager;
    [SerializeField] SpinWheelManager _spinWheelManager;
    [SerializeField] WheelSlotManager _wheelSlotManager;
    [SerializeField] ClientManager _clientManager;
    [SerializeField] WalletConnector _walletConnector;

    [Header("UI Settings:")]
    [SerializeField] Button _spinButton;
    [SerializeField] TMP_Text _timerText;
    [SerializeField] GameObject _loadPanel;
    [SerializeField] Slider _timeslider;
    [SerializeField] TMP_Text _tableID;
    [SerializeField] TMP_Text _walletAddress;

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


   // List<Bet> _currentBets = new List<Bet>();

    Bet _currentBet = null;
    int _currentAmount;


    string gameJsonData = string.Empty;
    string playerJsonData = string.Empty;
    #endregion


    bool _onStart = true;

    private void Awake()
    {
        _boardManager.callback = this;
        _gameData = null;

        _onStart = true;
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
        Actions.AddClient += AddClientAction;
        Actions.timerIndex += TimerIndexAction;
        Actions.StartRoll += RoyelleteSpinAction;
        Actions.BetData += RoyellesBetData;

       // _walletConnector = GameObject.FindAnyObjectByType<WalletConnector>().GetComponent<WalletConnector>();

        _walletAddress.text = "Wallet address: " +  Network.Instance.Id;
        _tableID.text = "Table ID:" + Network.Instance.Tableid;

        // Test();

        //AudioManager.Instance.SpeechAction(Speech.placeBet);

        //API Handling ...
        //APIHandler.Instance.GET("https://thecrypto360.com/roulette.php", SuccessAPI, ErrorAPI);
    }

    /// <summary>
    /// Get the royelles bet data
    /// </summary>
    /// <param name="bet"></param>
    private void RoyellesBetData(int bet)
    {
        _boardManager.SetGetSlot(_wheelSlotManager.GetWheelSlot(bet));
        Actions.SetBallTarget(_wheelSlotManager.GetWheelSlot(bet).transform);

        StartCoroutine(Play());
    }

    private void RoyelleteSpinAction()
    {
        if (!_boardManager._setBet)
            _boardManager.ClearBets();

        _boardManager._setBet = false;

        _timerText.text = "No more Bets";

        Actions.EnablePlay(false);

        _clientManager.ResetAction(playerJsonData);

        SpinButtonAction();
    }

    private void TimerIndexAction(int time)
    {
        string timeBet = "";


        if (_timeslider.maxValue - (float)time == 45.0f)
        {
            PopMessage.Instance.PopUpMessage(PopMessage.MessageType.normal, "Last bet");

            timeBet = "Last bet";

            AudioManager.Instance.SpeechAction(Speech.lastBet);
        }


        if (_timeslider.maxValue - (float)time == 30.0f)
        {
            PopMessage.Instance.PopUpMessage(PopMessage.MessageType.normal, "No more Bets");

            _boardManager.EnableBet(false);
            Actions.EnablePlay(false);

            AudioManager.Instance.SpeechAction(Speech.NoMoreBet);
        }

        if (_timeslider.maxValue - (float)time <= 45.0f && _timeslider.maxValue - (float)time > 30.0f)
        {
            timeBet = "Last bet";
        }
        else if (_timeslider.maxValue - (float)time <= 30.0f)
        {
            timeBet = "No more Bets";
            SaveGameStatus(GameSwitch.lastbet);
        }
        else
            timeBet = "Place your bet";

        float timeShow = _timeslider.maxValue - (float) time;

        _timerText.text = timeShow.ToString() + " " + timeBet;
        _timeslider.DOValue(time, 0.5f);
    }

    private void AddClientAction(string id)
    {
        Debug.Log("Adding player >>" + id);

       _clientManager.AddClient(id,playerJsonData);
    }

    private void SetCurrentPlayerData(Bet bet)
    {
        Debug.Log("Saving bet !!!!!!!!!!" + bet);

        _clientManager.UpdateClient(Network.Instance.Id,bet,playerJsonData);
    }

    void Test()
    {
        ResponseData responseData = new ResponseData();

        responseData.hash = "dnckdnfdnv";
        responseData.result = 2;
        responseData.blockNumber = 0;

        _boardManager.SetGetSlot(_wheelSlotManager.GetWheelSlot(responseData.result));

        Actions.SetBallTarget(_wheelSlotManager.GetWheelSlot(responseData.result).transform);

        Actions.ReadStats(2, Slot.ColorType.black);

        // TMP_Text.text = responseData.result.ToString();

        StartCoroutine(Play());
    }

    #region GAME_STATUS
    /// <summary>
    /// Save the game data
    /// </summary>
    /// <param name="camera"></param>
    public void SaveGameStatus(GameSwitch _status)
    {
        string jsonString = JsonUtility.ToJson(new GameData { status = _status });

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
            if (obj.Contains("status"))
            {
                GameData gameData = JsonUtility.FromJson<GameData>(obj);
                Debug.Log("Game LIVE data " + gameData.status + "" + _onStart);

                Actions.EnablePlay(gameData.status == GameSwitch.on);

                _boardManager.EnableBet(gameData.status == GameSwitch.on);

                if (_onStart)
                {
                    _loadPanel.SetActive(gameData.status == GameSwitch.off);
                }
                else
                {
                    _loadPanel.SetActive(false);
                }
                

                gameJsonData = obj.ToString();
            }

            _onStart = false;
        } 
        catch 
        {
            gameJsonData = string.Empty;
            Debug.LogWarning("No GameData");
        }

        //PLAYER DATA
        try
        {
            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(obj);

            if (playerDataList != null)
            {
                Debug.Log("Player got >>>" + playerDataList.playerDatas.Count);

                if (!obj.Contains("status"))
                {
                    playerJsonData = obj.ToString();

                    _clientManager.ClientBetDetect(playerJsonData);
                }
            }
        }
        catch
        {
            //playerJsonData = string.Empty;
            Debug.LogWarning("No PlayerData");
        }


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
        Actions.EnablePlay(true);

        StartCoroutine(Play());
    }

   
    #endregion

    /// <summary>
    /// Timer
    /// </summary>
    /// <returns></returns>
    IEnumerator Play()
    {
        yield return null;
        yield return new WaitUntil(()=> !_loadPanel.activeInHierarchy);

        _boardManager.EnableBet(true);

        Actions.DealerSet(false);

        AudioManager.Instance.SpeechAction(Speech.placeBet);
        Actions.EnablePlay(true);

        //float elapsedTime = 0f;

        //while (elapsedTime < _delay)
        //{

        //    elapsedTime += Time.deltaTime;

        //    float currentTimer =  _delay - elapsedTime;

        //    //_timerText.text = "Timer :" +  Mathf.RoundToInt(currentTimer);

        //    yield return null;
        //}

        //AudioManager.Instance.SpeechAction(Speech.NoMoreBet);
        //SpinButtonAction();
    }

    public void BetProducedAction(int number = -1, Slot.ColorType colorType = Slot.ColorType.NULL)
    {
        if (number < 0)
            return;

        Debug.LogWarning(number + ": " + colorType.ToString());

        if (number == 0)
            Actions.ReadStats(0,Slot.ColorType.red);
        else 
            Actions.ReadStats(number, colorType);

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
        _timeslider.value = 0;
        _timerText.text = "";
        StartCoroutine(ResetAction());
    }

    IEnumerator ResetAction()
    {
        yield return null;
        yield return new WaitUntil(() => _dealerStatus);

        Network.Instance.ResetTimer();

        _clientManager.ResetAction(playerJsonData);

        SaveGameStatus(GameSwitch.on);
        //Test();

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

    public void TableViewAction()
    {
        CameraController.Instance.CameraSwitchAction(CameraController.CameraSwitch.table);
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

    public void CopyTableAction()
    {
        _walletConnector.CopyTableId();
    }
}