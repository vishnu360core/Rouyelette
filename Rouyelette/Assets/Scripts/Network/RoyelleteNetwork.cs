using System.Collections;
using UnityEngine;
using System;
using System.Text;

using NativeWebSocket;

using Thirdweb;
using System.Threading.Tasks;
using Thirdweb.Examples;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading;

//using MetaMask.NativeWebSocket;


public class RoyelleteNetwork : MonoBehaviour
    {
       static RoyelleteNetwork instance;

    public static RoyelleteNetwork Instance { get { return instance; } }

    #region WEBSOCKET

     WebSocket websocket;
     WebSocket webTimer;

     string _id;
     public string Id => _id;

    public UnityEvent<string> GetBalanceAction { get; private set; }

    public async void NetworkStart()
        {
           if (instance == null)
               instance = this;

           _id = Guid.NewGuid().ToString();

            websocket = new WebSocket("ws://localhost:8090");
            webTimer = new WebSocket("ws://localhost:8100");

            webTimer.OnOpen += () =>
            {
                Console.WriteLine("timer opened");
            };

            webTimer.OnMessage += (bytes) =>
            {
                 string str = Encoding.UTF8.GetString(bytes);

                Debug.Log("Timer >>>" + str);

                if (str != "Play")
                {
                    int timer = int.Parse(str);
                    Actions.timerIndex(timer);
                }
                else
                    Actions.StartRoll();
            };


            websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");

                Console.WriteLine("Opened");

                websocket.SendText(_id);
            };

            websocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);

                Console.WriteLine("Opened error");
            };

            websocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");

                Console.WriteLine("closed");
            };

            websocket.OnMessage += (bytes) =>
            {
                string str = Encoding.UTF8.GetString(bytes);

                Debug.Log(str);

                if (IsJsonString(str))
                    Actions.GetGameData(str);
                else if(str == "ResetAction")
                   ResetAction();
                else 
                    Actions.AddClient(str);

            };

            await websocket.Connect();
            await webTimer.Connect();   
        }

    void ResetAction()
    {
        websocket.SendText(_id);
    }

        /// <summary>
       /// Sending the data 
       /// </summary>
       /// <param name="jsonString"></param>
        public IEnumerator SaveToNet(string jsonString)
        {
            Debug.Log("WebSocket State >>>> " + websocket.State);

            if (websocket.State == WebSocketState.Closed  || websocket.State == WebSocketState.Closing)
                yield return null;
            else
            {
              yield return new WaitUntil(() => websocket.State == WebSocketState.Open);
                websocket.SendText(jsonString);
            }
        }
        

        public  bool IsJsonString(string str)
        {
            try
            {
                // Attempt to deserialize the string
                JsonUtility.FromJson(str, typeof(object));
                return true;
            }
            catch (System.Exception)
            {
                // Parsing failed, indicating that the string is not valid JSON
                return false;
            }
        }
        
        /// <summary>
        /// Reset the timer
        /// </summary>
        public void ResetTimer()
        {
           webTimer.SendText("ResetTimer");
        }
    #endregion

    #region WALLET

    ThirdwebSDK thirdwebSDK;

    [Space]
    [SerializeField] GameObject _HasNFT;
    [SerializeField] GameObject _NoNFT;

    [Header("Wallet:")]
    [SerializeField]Prefab_ConnectWallet connectWallet;

    private void Start()
    {
        thirdwebSDK = ThirdwebManager.Instance.SDK;

        connectWallet.onConnected.AddListener(GetBalance);
    }


    public void GetBalance (string str) 
    {
        Debug.Log("Connection action called");

        var data = GetBalanceAsync(str);
    }

    public async Task GetBalanceAsync(string addresss)
    {
        Contract contract = thirdwebSDK.GetContract("0x75F52d54Eae4b91174d17C4281473505885CB6Ec");

       var data = await contract.ERC1155.BalanceOf(addresss, "0");


        var sendResult =  contract.ERC1155.Transfer("0x55B62C7f8a159Ef24f2AE4Eb9593e959e92066dc", "0", 1);

       Debug.Log("Send Result >>>" + sendResult);

        _HasNFT.SetActive(int.Parse(data.ToString()) > 0);
        _NoNFT.SetActive(!(int.Parse(data.ToString()) > 0));
    }



    #endregion
}
