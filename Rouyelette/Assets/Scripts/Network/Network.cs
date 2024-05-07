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

    WebSocket webTable;

    string walletAddress = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        Actions.walletAddress += GetWalletAddress;
    }

    private void GetWalletAddress(string address)
    {
        walletAddress = address;
    }

    private void OnApplicationQuit()
    {
        webData.SendText(_id);
    }

    string _id;
    public string Id => _id;

    string _tableid;
    public string Tableid => _tableid;

    public void SetId(string id)
    {
        _id = id;
       // websocket.SendText(_id);
    }


    async void Start()
    {
        if (instance == null)
            instance = this;

        //_id = Guid.NewGuid().ToString();


        string ip = "62.72.56.181";

        //websocket = new WebSocket(" wss://62.72.56.181:8090");//8090
        //webTimer = new WebSocket(" wss://62.72.56.181:8100");//8100
        //webData = new WebSocket(" wss://62.72.56.181:8200");//8200
        //webWallet = new WebSocket(" wss://62.72.56.181:9010");//9010
        //webCredit = new WebSocket(" wss://62.72.56.181:9020");//9020
        //webDeduct = new WebSocket(" wss://62.72.56.181:9030");//9030


        //websocket = new WebSocket("wss://unity.thecrypto360.com");//8090
        //webTimer = new WebSocket("wss://unity2.thecrypto360.com");//8100
        //webData = new WebSocket("wss://unity3.thecrypto360.com");//8200
        //webWallet = new WebSocket("wss://unity4.thecrypto360.com");//9010
        //webCredit = new WebSocket("wss://unity5.thecrypto360.com");//9020
        //webDeduct = new WebSocket("wss://unity6.thecrypto360.com");//9030

        websocket = new WebSocket(" ws://localhost:8090");//8090
        webTimer  = new WebSocket(" ws://localhost:8100");//8100
        webData   = new WebSocket(" ws://localhost:8200");//8200
        webWallet = new WebSocket(" ws://localhost:9010");//9010
        webCredit = new WebSocket(" ws://localhost:9020");//9020
        webDeduct = new WebSocket(" ws://localhost:9030");//9030
        webTable  = new WebSocket(" ws://localhost:7070");//7070


        #region WEB_TABLE

        webTable.OnOpen += () =>
        {
            Debug.Log("Connection open! >> TABLE ");

        };

        webTable.OnError += (e) =>
        {
            Debug.Log("Error! TABLE socket" + e);

        };

        webTable.OnClose += (e) =>
        {
            Debug.Log("Connection closed! TABLE");
        };

        webTable.OnMessage += (bytes) =>
        {
            string str = Encoding.UTF8.GetString(bytes);

            Debug.Log("Table status :" + str);

            Actions.TableStatus(str);
                
        };

        #endregion

        #region WEB_TIMER
        webTimer.OnOpen += () =>
            {
                Console.WriteLine("timer opened");
            };

        webTimer.OnMessage += (bytes) =>
        {
            string str = Encoding.UTF8.GetString(bytes);

            Debug.Log("Timer Information >>" + str);

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

           // websocket.SendText(_id);
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

        webWallet.OnClose += async (e) =>
        {
            Debug.Log("WebWallet Connection closed!");

          // await  webWallet.Connect();
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

        webCredit.OnClose += async (e) =>
        {
            Debug.Log("Credit Connection closed!");

          // await webCredit.Connect();
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

        webDeduct.OnClose += async (e) =>
        {
            Debug.Log("Deduct Connection closed!");

           // await webDeduct.Connect();
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

        await webTable.Connect();
        //await webData.Connect();
        //await websocket.Connect();
        //await webTimer.Connect();
        //await webWallet.Connect();
        //await webDeduct.Connect();
        //await webCredit.Connect();
    }

    #region GAME

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

    #endregion

    #region TIMER
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
        Debug.Log("Sending wallet address to server 1");
        if (webWallet.State == WebSocketState.Closed || webWallet.State == WebSocketState.Closing)
            yield return null;
        else
        {
            yield return new WaitUntil(() => webWallet.State == WebSocketState.Open);

            Debug.Log("Sending wallet address to server 2");
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

    #region TABLE

    /// <summary>
    /// Push the id to server
    /// </summary>
    /// <param name="tableId"></param>
    public void PushTableId(string tableId)
    {
        _tableid = tableId;

        string tableCreate = tableId + "[id]" + _id + "[/id]";

        webTable.SendText(tableCreate);
    }

    /// <summary>
    /// Search the id in the network
    /// </summary>
    /// <param name="searchId"></param>
    public void SearchID(string searchId) 
    {
        string search = "[s]" + searchId + "[id]" + _id + "[/id]"; 
        webTable.SendText(search);
    }

    #endregion
}

[System.Serializable]
public class BetssData
{
    public int[] numbers;
}
