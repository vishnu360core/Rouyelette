using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using System;

public class Network : MonoBehaviour
{
    WebSocket websocket;

    async void Start()
    {
        websocket = new WebSocket("wss://socketsbay.com/wss/v2/1/demo/");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");

            Console.WriteLine("Opened");

            websocket.SendText("Player entered") ;
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
            //Debug.Log("Message !!!" + bytes);

        };

        await websocket.Connect();
    }
}
