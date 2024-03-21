using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using WebSocketSharp;

using NativeWebSocket;
using System;
using System.Text;
using Unity.VisualScripting;

public class Network : MonoBehaviour
{
    WebSocket websocket;

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8090");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");

            Console.WriteLine("Opened");

            websocket.SendText(Guid.NewGuid().ToString());
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

        };

        await websocket.Connect();
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
