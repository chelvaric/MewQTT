using MewQTT.Frames;
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
    }

    public class AsynchronousSocketListener 
    {
        //thread signal
        public static ManualResetEvent AllDone = new ManualResetEvent(false);

        string _ip;
        int _port;
        Socket _mainSocket;

        public AsynchronousSocketListener (string ip, int port)
       {
            _ip = ip;
            _port = port;
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
                state.sb.Append(Encoding.UTF8.GetString(state.Buffer, 0, bytesread));

                content = state.sb.ToString();
                Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                    content.Length, content);

                BaseFrame frame = new BaseFrame();
                frame.GetData(state.Buffer);
            }
            else
            {
                handler.BeginReceive(state.Buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReadCallBack), state);
            }

        }
    }
}
