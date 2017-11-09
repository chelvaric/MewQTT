using MewQTT.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Models
{
    public class Client
    {
        public string ClientId { get; set; }

        public string WillTopic { get; set; }

        public string WillMessage { get; set; }

        public int WillQoS{get;set;}

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool Retain { get; set; }




        public delegate void SendMessageHandler(IMQTTMessage message);


        public event SendMessageHandler OnSend;
        public event EventHandler OnDisconect;

        object lockMessageRecv = new object();
        public void MessageRecieved(IMQTTMessage message)
        {

            lock(lockMessageRecv)
            {

                MessageHandler.HandleMessage(this, message);

            }

        }


        public void Send(IMQTTMessage message)
        => OnSend?.Invoke(message);

        public void Disconect()
            => OnDisconect?.Invoke(this, null);

    }
}
