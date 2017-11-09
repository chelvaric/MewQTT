using MewQTT.Frames;
using MewQTT.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MewQTT
{


    public class StateObject
    {
        //cient socket
        public Socket workSocket = null;

        //size of buffer
        public const int bufferSize = 1024;

        //recieve buffer
        public byte[] Buffer = new byte[bufferSize];

        //get the text
        public StringBuilder sb = new StringBuilder();

        public CommandParser parser = new CommandParser();

        Client MqqtClient = new Client();

        public StateObject()
        {
            parser.OnMessageRecieved += Parser_OnMessageRecieved;
            MqqtClient.OnDisconect += MqqtClient_OnDisconect;
            MqqtClient.OnSend += MqqtClient_OnSend;
        }

        private void MqqtClient_OnSend(IMQTTMessage message)
        {
            AsynchronousSocketListener.Send(workSocket, message);
        }

        private void MqqtClient_OnDisconect(object sender, EventArgs e)
        {
            parser.Dispose();
            workSocket.Disconnect(false);
            workSocket.Dispose();
            
        }

        private void Parser_OnMessageRecieved(IMQTTMessage message)
        {
            MqqtClient.MessageRecieved(message);
        }
    }

    public class AsynchronousSocketListener 
    {
        //thread signal
        public static ManualResetEvent AllDone = new ManualResetEvent(false);

        string _ip;
        int _port;
        Socket _mainSocket;

        MewQTTServer _server;

        public AsynchronousSocketListener (string ip, int port,MewQTTServer server)
       {
            _ip = ip;
            _port = port;
            _server = server;
         }


        public  int StartListening()
        {
            //buffer incoming data
            byte[] buffer = new byte[1024];

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);

            _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //bind and listen
                _mainSocket.Bind(iPEndPoint);
                _mainSocket.Listen(100);

                Console.WriteLine("Socket Made");

                while(true)
                {
                    //set the event to non signaled
                    AllDone.Reset();

                    //start the async socket to listen for connection
                    Console.WriteLine("listening for connections");

                    _mainSocket.BeginAccept(new AsyncCallback(AcceptCallBack), _mainSocket);

                    //wait until accept is handled
                    AllDone.WaitOne();

                }


            }
            catch(Exception ex)
            {
                return -1;
            }
        }

        public void AcceptCallBack(IAsyncResult r)
        {
            //signal the main thread to continue
            AllDone.Set();

            //gets the socket to handle the connection
            Socket Listener = (Socket)r.AsyncState;
            Socket handler = Listener.EndAccept(r);

            //create the state object
            StateObject state = new StateObject
            {
                workSocket = handler
            };
            //start recieving
            handler.BeginReceive(state.Buffer,0, StateObject.bufferSize,0,new AsyncCallback(ReadCallBack),state);
            Console.WriteLine("Started recieving on: " + state.workSocket.RemoteEndPoint.ToString());
        }

        public void ReadCallBack(IAsyncResult r)
        {
            String content = String.Empty;

            //reetrieve the state object and the buffer
            //from the async state obj
            StateObject state = (StateObject)r.AsyncState;
            Socket handler = state.workSocket;


            //read data from the client socket
            int bytesread = handler.EndReceive(r);

            if(bytesread > 0)
            {

                byte[] recievedbytes = new byte[bytesread];
                Array.Copy(state.Buffer, recievedbytes, bytesread);

                state.parser.AddToQueue(recievedbytes);
            }
            else
            {
                handler.BeginReceive(state.Buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCallBack), state);
            }

        }

        public static void Send(Socket handler, IMQTTMessage message)
        {
            byte[] data = message.Serialize();
            handler.BeginSend(data,0,data.Length,0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
