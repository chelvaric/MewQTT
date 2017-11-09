using System;
using MewQTT;



namespace TestServer
{
    class Program
    {
        

        static  void Main(string[] args)
        {

            MewQTT.Models.MewQTTServer server = new MewQTT.Models.MewQTTServer("");
            server.RunServer();
            Console.ReadLine();
            return;



        }
    }
}
