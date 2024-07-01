using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Data.Common;


public class Network : MonoBehaviour
 {
    static Network instance; 
    
    public static Network Instance { get { return instance; } }  


    string walletAddress = "";

    [DllImport("__Internal")]
    private static extern void WebSocketInit(string url);

    [DllImport("__Internal")]
    private static extern void Send(int index, string message);


    void ConnectCallBack(int index)
    {
        Debug.Log("Index connected" + index);

        switch (index)
        {
            case 0:
                break;

            case 1:
                Send(1, _id);
                break;

            case 2:
                break;

            case 3:
                string timerId = "(" + _tableid + ")";
                Send(3, timerId);
                break;

            case 4:
                StartCoroutine(SendWallet(walletAddress));
                break;

            case 5:
                break;

            case 6:
                break;

        }

    }


    void ReceiveMessage(string data)
    {
        string[] parts;
        string message;
        string index;

        parts = data.Split('@');
        message = parts[0];
        index = parts[1];

        if (message == "Ping")
            return;

        switch(int.Parse(index))
        {
            case 0:
                Actions.TableStatus(message);

                break;


            case 1:

                Debug.Log("Player message" + message);

                if (IsJsonString(message))
                    Actions.GetGameData(message);
                else if (message == "ResetAction")
                    ResetAction();
                else if (message.Contains("Delete"))
                {
                    message = message.Replace("Delete", "");
                    Debug.LogWarning("Player exited : " + message);

                    Actions.DeleteClient(message);
                }
                else
                    Actions.AddClient(message);

                break;


            case 2:
            
                if (!message.Contains("["))
                    Actions.BetData(int.Parse(message));
                else
                {
                    Debug.Log("its json");
                    //str = str.Replace("[", "{");
                    //str = str.Replace("]", "}");
                    string json = "{ \"numbers\": " + message + " }";
                    Debug.Log(json);

                    BetssData betssData = JsonUtility.FromJson<BetssData>(json);

                    Actions.ReadHistoryStat(betssData.numbers);
                }

                break;


            case 3:
           
                if (message != "Play")
                {
                    int timer = int.Parse(message);
                    Actions.timerIndex(timer);
                }
                else
                {
                    Actions.StartRoll();
                };

                break;


               case 4:

                float _balanceDollar = float.Parse(message);

                Debug.LogWarning("Wallet Balance :" + _balanceDollar);

                Actions.GetWalletBalance(_balanceDollar);

                break;


            case 5:
                Debug.Log("Credit_MATICS >>" + message);

                Actions.Credit_MAT(message);
                break;


            case 6:
                Debug.Log("Deduct_MATICS >>" + message);

                Actions.Deduct_MAT(message);
                break;
        }


    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        Actions.walletAddress += GetWalletAddress;

        Actions.GameLoaded += OnGameSceneLoaded;
    }

    private void OnGameSceneLoaded()
    {
        Debug.LogWarning("On Game Scene !!!");

        WebSocketInit("wss://unity.thecrypto360.com"); //1 GAME 8090
        WebSocketInit("wss://unity3.thecrypto360.com");//2 DATA 8200
        WebSocketInit("wss://unity2.thecrypto360.com");//3 TIMER 8100
        WebSocketInit("wss://unity4.thecrypto360.com");//4 WALLET 9010
        WebSocketInit("wss://unity5.thecrypto360.com");//5 CREDIT 9020
        WebSocketInit("wss://unity6.thecrypto360.com");//6 DEDUCT 9030


        //WebSocketInit("ws://localhost:8090"); //1 GAME 8090
        //WebSocketInit("ws://localhost:8200");//2 DATA 8200
        //WebSocketInit("ws://localhost:8100");//3 TIMER 8100
        //WebSocketInit("ws://localhost:9010");//4 WALLET 9010
        //WebSocketInit("ws://localhost:9020");//5 CREDIT 9020
        //WebSocketInit("ws://localhost:9030");//6 DEDUCT 9030


    }

    private void GetWalletAddress(string address)
    {
        walletAddress = address;
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


    public string GetTableId(string data, string web)
    {
        string tableId = GetStringBetween(data, "[#]", "[/#]");

        Debug.LogWarning("Table Id >>>" + tableId + " " +web);

        return tableId;
    }


     void Start()
    {
        if (instance == null)
            instance = this;

        //_id = Guid.NewGuid().ToString();


        string ip = "62.72.56.181";


        //websocket = new WebSocket(" ws://62.72.56.181:8090");//8090
        //webTimer = new WebSocket(" ws://62.72.56.181:8100");//8100
        //webData = new WebSocket(" ws://62.72.56.181:8200");//8200
        //webWallet = new WebSocket(" ws://62.72.56.181:9010");//9010
        //webCredit = new WebSocket(" ws://62.72.56.181:9020");//9020
        //webDeduct = new WebSocket(" ws://62.72.56.181:9030");//9030
        //webTable = new WebSocket(" ws://62.72.56.181:7070");//7070


        //websocket = new WebSocket("wss://unity.thecrypto360.com");//8090
        //webTimer = new WebSocket("wss://unity2.thecrypto360.com");//8100
        //webData = new WebSocket("wss://unity3.thecrypto360.com");//8200
        //webWallet = new WebSocket("wss://unity4.thecrypto360.com");//9010
        //webCredit = new WebSocket("wss://unity5.thecrypto360.com");//9020
        //webDeduct = new WebSocket("wss://unity6.thecrypto360.com");//9030
        //webTable = new WebSocket("wss://unity7.thecrypto360.com");//7070

        //websocket = new WebSocket(" ws://localhost:8090");//8090
        //webTimer = new WebSocket(" ws://localhost:8100");//8100
        //webData = new WebSocket(" ws://localhost:8200");//8200
        //webWallet = new WebSocket(" ws://localhost:9010");//9010
        //webCredit = new WebSocket(" ws://localhost:9020");//9020
        //webDeduct = new WebSocket(" ws://localhost:9030");//9030
        //webTable = new WebSocket(" ws://localhost:7070");//7070

        // WebSocketInit("ws://localhost:7070"); //0 TABLE 7070

        WebSocketInit("wss://unity7.thecrypto360.com"); //0 TABLE 7070
    }

    #region GAME

    void ResetAction()
    {
       // websocket.SendText(_id);
    }

    /// <summary>
    /// Sending the data 
    /// </summary>
    /// <param name="jsonString"></param>
    public IEnumerator SaveToNet(string jsonString)
    {
        yield return null;

        //websocket.SendText(jsonString);

        Debug.Log("Json value " + jsonString);

        Send(1, jsonString);
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

        //  webTimer.SendText("ResetTimer");

        Send(3, "ResetTimer");
    }

    #endregion

    #region WALLET

    public IEnumerator SendWallet(string message)
    {
        yield return null;
        Send(4, message);
    }

    public void CreditAmount(float amount)
    {
       // webCredit.SendText(amount.ToString());

        Send(5,amount.ToString());  
    }

    public void DeductAmount(float amount)
    {
        Debug.LogWarning("Deduct: " + amount);

       // webDeduct.SendText(amount.ToString());

       Send(6, amount.ToString());  
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

        Send(0,tableCreate);

        // webTable.SendText(tableCreate);
    }

    /// <summary>
    /// Search the id in the network
    /// </summary>
    /// <param name="searchId"></param>
    public void SearchID(string searchId) 
    {
        _tableid = searchId;

        string search = "[s]" + searchId + "[id]" + _id + "[/id]"; 
       // webTable.SendText(search);

        Send(0,search);
    }

    #endregion

    public  string GetStringBetween(string str, string firstString, string lastString)
    {
        int pos1 = str.IndexOf(firstString) + firstString.Length;
        int pos2 = str.Substring(pos1).IndexOf(lastString);
        return str.Substring(pos1, pos2);
    }

}

[System.Serializable]
public class BetssData
{
    public int[] numbers;
}
