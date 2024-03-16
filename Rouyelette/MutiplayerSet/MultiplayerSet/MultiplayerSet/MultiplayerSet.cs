using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
using DarkRift.Server;
using MultiplayerModel;

namespace MultiplayerSet
{
    public class MultiplayerSet : Plugin
    {
        public override bool ThreadSafe => false;

        public override Version Version => new Version(1,1,0);

        public MultiplayerSet(PluginLoadData pluginLoadData) : base(pluginLoadData) 
        {
            //Console.WriteLine("Multiplayer set");

            ClientManager.ClientConnected += OnClientConnected;
            ClientManager.ClientDisconnected += OnClientDisConnected;
        }

        private void OnClientDisConnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Player DisConnected :" + e.Client.ID);
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("Player Connected :" + e.Client.ID);

            e.Client.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
           using (Message message = e.GetMessage()) 
            {
                if(message.Tag == (ushort) Tags.Tag.Text_Message)
                {
                    using (DarkRiftReader reader = message.GetReader()) 
                    {
                        string _message  = reader.ReadString();

                        Console.WriteLine("Message: " + _message);
                    }
                }
            }
        }
    }
}
