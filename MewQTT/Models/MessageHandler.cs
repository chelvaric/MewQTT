using MewQTT.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Models
{
  static class MessageHandler
    {

      public  static void HandleMessage(Client client, IMQTTMessage message)
        {
            switch(message.Type)
            {
                case MessageType.Connect: { HandleConnect(client, (ConnectMessage)message); break; }
                case MessageType.PingReq: { HandlePingReq(client); break; }
              
            }
        }


        static void HandlePingReq(Client client)
        {
            client.Send(new PingResponseMessage());
        }

       static void HandleConnect(Client client, ConnectMessage message)
        {
            AckConnectMessage Ackmessage = new AckConnectMessage();
           
            if(!message.CorrectProtocol)
            {
                Ackmessage.SessionId = 0;
                Ackmessage.ReturnCode = 0x01;
                client.Send(Ackmessage);
                client.Disconect();
                return;
            }

            if(message.Reserved)
            {
                client.Disconect();
            }

            if(client.ClientId == "")
            {
                Ackmessage.SessionId = 0;
                Ackmessage.ReturnCode = 0x02;
                client.Send(Ackmessage);
                client.Disconect();
                return;
            }

            if( MewQTTServer.instance.ServerSettings.useAuthentication)
            {
                if(message.UserName == null || message.Password == null)
                {
                    Ackmessage.SessionId = 0;
                    Ackmessage.ReturnCode = 0x04;
                    client.Send(Ackmessage);
                    client.Disconect();
                    return;
                }

                if(!MewQTTServer.instance.AuthHandler.CheckAuth(message.UserName, message.Password))
                {
                    Ackmessage.SessionId = 0;
                    Ackmessage.ReturnCode = 0x05;
                    client.Send(Ackmessage);
                    client.Disconect();
                    return;
                }
            }

            client.WillTopic = message.WillTopic;
            client.Retain = message.WillRetainFlag;
            client.UserName = message.UserName;
            client.Password = message.Password;
            client.ClientId = message.ClientId;
           

            if(message.CleanSession)
            {
                Ackmessage.SessionId = 0;
                Ackmessage.ReturnCode = 0;
                client.Send(Ackmessage);
                return;
            }
            else
            {
                if(MewQTTServer.instance.ContainsClientState(client))
                {
                    Ackmessage.SessionId = 1;
                }
                else
                {
                    Ackmessage.SessionId = 0;
                    MewQTTServer.instance.AddClientState(client);
                }

                Ackmessage.ReturnCode = 0;
                client.Send(Ackmessage);
                return;

            }



        }

    }
}
