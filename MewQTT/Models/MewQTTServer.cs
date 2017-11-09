using MewQTT.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Models
{
    public class MewQTTServer
    {

        public static MewQTTServer instance {get;set;}


        public  MewQTTServer(string SettingsLocation)
        {
            ServerSettings = new Settings() {
                ip = "127.0.0.1",
                port = 1883,
                acounts = new List<Acount>() { new Acount() { userName = "test", password = "1234" } },
                useAuthentication = true
            };

            if(ServerSettings.useAuthentication)
            {
                AuthHandler.Acounts = ServerSettings.acounts;   
            }
            Clients = new Dictionary<string, List<IMQTTMessage>>();

            Listener = new AsynchronousSocketListener(ServerSettings.ip, ServerSettings.port,this);
            instance = this;

        }


        public void RunServer()
        {
            Listener.StartListening();
        }

       public Settings ServerSettings;

        AsynchronousSocketListener Listener;

       internal Security.SecurityHandler AuthHandler = new Security.SecurityHandler();

        Dictionary<string,List<IMQTTMessage>> Clients { get; set; }

        public void Dispose()
        {
           
        
        }


        public bool ContainsClientState(Client client)
        {
            return Clients.ContainsKey(client.ClientId);
        }

        public void AddClientState(Client client)
        {
            Clients.Add(client.ClientId, new List<IMQTTMessage>());
        }

    }
}
