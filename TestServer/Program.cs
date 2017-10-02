using System;
using MewQTT;


namespace TestServer
{
    class Program
    {
        

        static  void Main(string[] args)
        {
            AsynchronousSocketListener listener = new AsynchronousSocketListener("127.0.0.1", 1883);
            int i =  listener.StartListening();

            Console.WriteLine(i);
            Console.ReadLine();
            return;
        }
    }
}
