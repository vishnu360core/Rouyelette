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

        string _id;
        public string Id => _id; 

        async void Start()
        {
           if (instance == null)
               instance = this;

          _id = Guid.NewGuid().ToString();

            websocket = new WebSocket("ws://localhost:8090");

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
                else
                    Actions.AddClient(str);

            };

            await websocket.Connect();
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



    //private void Start()
    //{
    //    WebSocket web = new WebSocket("ws://localhost:8090");
    //    web.Connect();

    //    web.OnMessage += (sender, e) =>
    //    {
    //        Debug.Log(e.Data);
    //    };

    //    web.Send(Guid.NewGuid().ToString());
    //}

}
