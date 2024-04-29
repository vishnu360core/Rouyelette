using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using WebSocketSharp;

using NativeWebSocket;
using System;
using System.Text;


public class Network : MonoBehaviour
 {
    static Network instance; 
    
    public static Network Instance { get { return instance; } }  

    WebSocket websocket;
    WebSocket webTimer;
    WebSocket webWallet;
    WebSocket webData;
    WebSocket webCredit;
    WebSocket webDeduct;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnApplicationQuit()
    {
        webData.SendText(_id);
    }

    #region GAME
    string _id;
    public string Id => _id;

    async void Start()
    {
        if (instance == null)
            instance = this;

        _id = Guid.NewGuid().ToString();

        websocket = new WebSocket("ws://localhost:8090");
        webTimer = new WebSocket("ws://localhost:8100");
        webData = new WebSocket("ws://localhost:8200");
        webWallet = new WebSocket("ws://localhost:9010");
        webCredit = new WebSocket("ws://localhost:9020");
        webDeduct = new WebSocket("ws://localhost:9030");

        #region WEB_TIMER
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
            {
                Actions.StartRoll();
            };
        };
        #endregion

        #region WEB_DATA

        webData.OnOpen += () =>
        {
            Debug.Log("Connection open! >> DATA ");

        };

        webData.OnError += (e) =>
        {
            Debug.Log("Error! data socket" + e);

        };

        webData.OnClose += (e) =>
        {
            Debug.Log("Connection closed! data");
        };

        webData.OnMessage += (bytes) =>
        {
            string str = Encoding.UTF8.GetString(bytes);

            Debug.Log("Data >>" + str);

            if (!str.Contains("["))
                Actions.BetData(int.Parse(str));
            else
            {
                Debug.Log("its json");
                //str = str.Replace("[", "{");
                //str = str.Replace("]", "}");
                string json = "{ \"numbers\": " + str + " }";
                Debug.Log(json);

                BetssData betssData = JsonUtility.FromJson<BetssData>(json);

                Actions.ReadHistoryStat(betssData.numbers);
            }    
        };
        #endregion

        #region WEB_GAME

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

            Debug.Log("GAME: " + str);

            if (IsJsonString(str))
                Actions.GetGameData(str);
            else if (str == "ResetAction")
                ResetAction();
            else if(str.Contains("Delete"))
                  {
                     str = str.Replace("Delete", "");
                     Debug.LogWarning("Player exited : " +  str);

                     Actions.DeleteClient(str);
                  }
            else
                Actions.AddClient(str);

        };

        #endregion

        #region WEB_WALLET

        webWallet.OnOpen += () =>
        {
            Debug.Log("Wallet Connection open!");
        };

        webWallet.OnError += (e) =>
        {
            Debug.Log("WebWallet_Error! " + e);
        };

        webWallet.OnClose += (e) =>
        {
            Debug.Log("WebWallet Connection closed!");
        };

        webWallet.OnMessage += (bytes) =>
        {
            string str = Encoding.UTF8.GetString(bytes);

            float _balanceDollar = float.Parse(str);

            Debug.LogWarning("Wallet Balance :" +  _balanceDollar);

            Actions.GetWalletBalance(_balanceDollar);

        };

        #endregion

        #region WEB_CREDIT

        webCredit.OnOpen += () =>
        {
            Debug.Log("Credit Connection open!");
        };

        webCredit.OnError += (e) =>
        {
            Debug.Log("Credit_Error! " + e);
        };

        webCredit.OnClose += (e) =>
        {
            Debug.Log("Credit Connection closed!");
        };

        webCredit.OnMessage += (bytes) =>
        {
            string str = Encoding.UTF8.GetString(bytes);

            Debug.Log("Credit_MATICS >>" + str);

            Actions.Credit_MAT(str);
        };

        #endregion

        #region WEB_DEDUCT
        webDeduct.OnOpen += () =>
        {
            Debug.Log("Deduct Connection open!");
        };

        webDeduct.OnError += (e) =>
        {
            Debug.Log("Deduct_Error! " + e);
        };

        webDeduct.OnClose += (e) =>
        {
            Debug.Log("Deduct Connection closed!");
        };

        webDeduct.OnMessage += (bytes) =>
        {
            string str = Encoding.UTF8.GetString(bytes);

            Debug.Log("Deduct_MATICS >>" + str);

            Actions.Deduct_MAT(str);
        };

        #endregion

        if (webTimer.State == WebSocketState.Connecting || webTimer.State == WebSocketState.Open)
        {
            Debug.Log("Still connecting !!! and closing it");
            await webTimer.Close();
        }

        await webData.Connect();
        await websocket.Connect();
        await webTimer.Connect();
        await webWallet.Connect();
        await webDeduct.Connect();
        await webCredit.Connect();
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
           Debug.Log("Resetting timer !!!!!");
         
           webTimer.SendText("ResetTimer");
        }
    #endregion

    #region WALLET

    public IEnumerator SendWallet(string message)
    {
        if (webWallet.State == WebSocketState.Closed || webWallet.State == WebSocketState.Closing)
            yield return null;
        else
        {
            yield return new WaitUntil(() => webWallet.State == WebSocketState.Open);
            webWallet.SendText(message);
        }
    }

    public void CreditAmount(float amount)
    {
        webCredit.SendText(amount.ToString());
    }

    public void DeductAmount(float amount)
    {
        Debug.LogWarning("Deduct: " + amount);

        webDeduct.SendText(amount.ToString());
    }

    #endregion
}

[System.Serializable]
public class BetssData
{
    public int[] numbers;
}
