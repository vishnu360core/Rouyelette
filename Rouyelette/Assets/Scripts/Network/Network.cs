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

    private void Awake()
    {
        if (instance == null)
            instance = this;
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

            Actions.BetData(int.Parse(str));
        };
        #endregion

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
            else if (str == "ResetAction")
                ResetAction();
            else
                Actions.AddClient(str);

        };

        if (webTimer.State == WebSocketState.Connecting || webTimer.State == WebSocketState.Open)
        {
            Debug.Log("Still connecting !!! and closing it");
            await webTimer.Close();
        }

        await webData.Connect();
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
           Debug.Log("Resetting timer !!!!!");
         
           webTimer.SendText("ResetTimer");
        }
    #endregion

    #region WALLET

    


    #endregion

    #region BET_DATA




    #endregion

}
