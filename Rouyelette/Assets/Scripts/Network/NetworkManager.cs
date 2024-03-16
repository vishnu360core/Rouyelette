using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkRift.Client.Unity;
using DarkRift;
using Message = DarkRift.Message;

using MultiplayerModel;

public class NetworkManager : MonoBehaviour
{
    static NetworkManager instance; 
    public static NetworkManager Instance {  get { return instance; } }


    private void Awake()
    {
        if (instance == null)
         instance = this;
    }

    public UnityClient _client;

    public void TestNetworkAction()
    { 
        using (DarkRiftWriter writer = DarkRiftWriter.Create()) 
        {
            writer.Write("Write !!!!");
            using(Message message  = Message.Create((ushort)Tags.Tag.Text_Message,writer))
                _client.SendMessage(message,SendMode.Reliable);
        }
    }



}
