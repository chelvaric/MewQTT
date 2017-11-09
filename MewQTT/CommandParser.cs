using MewQTT.Frames;
using MewQTT.HelperClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MewQTT
{
    public class CommandParser
    {

        public delegate void MessageHandler(IMQTTMessage message);
        public event MessageHandler OnMessageRecieved;
        public bool End
        {
            get;
            set;
        }

        Thread MessageThread,BufferThread; 

        public CommandParser()
        {
            MessageThread = new Thread(() => { ProccessMessage(); });
            BufferThread = new Thread(() => { ProcessQueue(); });

            MessageThread.Start();
            BufferThread.Start();
        }

        void ProcessQueue()
        {
            while(End != true)
            {
                if(BufferQueue.Count != 0)
                {
                    var buffer = BufferQueue.Dequeue();
                    while (ProcessBuffer(ref buffer) > 0) ;
                }
            }

        }

        void ProccessMessage()
        {
            while (End != true)
            {
                if (Messageque.Count != 0)
                {
                    IMQTTMessage message = null;
                    var buffer = Messageque.Dequeue();

                    switch(ReadMessageTypeFromHeader(buffer[0]))
                    {
                        case MessageType.Connect:
                            {
                                ConnectMessage mess = new ConnectMessage();
                                mess.MakeMessage(buffer);
                                message = mess;
                                Console.WriteLine("Message connected");
                                Console.WriteLine("username:" + mess.UserName);
                                break;
                            }
                    }

                    OnMessageRecieved?.Invoke(message);
                }
            }
        }

        FixedHeaderFrame header = new FixedHeaderFrame();
        List<byte> messagebuffer = new List<byte>();
        bool messageCompleted = true;
        public  int ProcessBuffer( ref byte[] buffer)
        {

            if (messageCompleted)
            {
                byte headerbyte;
                int recievedsize = buffer.Length;
                int i = 0;
                header = new FixedHeaderFrame();
                do
                {
                    headerbyte = buffer[i];
                    i++;
                }
                while (recievedsize > 0 && header.AppendByte(headerbyte));

                if (!header.HeaderComplete)
                {
                    return 0;
                }
                else
                {
                    messagebuffer = new List<byte>();
                    messageCompleted = false;
                }
            }

            header.CreateMessageBuffer(ref messagebuffer);

           for(int position = (header.HeaderSize); position < buffer.Length; position++)
            {
                messagebuffer.Add(buffer[position]);
                if(messagebuffer.Count == (header.HeaderSize + header.RemainingLenght))
                {
                    Messageque.Enqueue(messagebuffer);
                    messageCompleted = true;
                  
                    break;
                }
            }

           if(messagebuffer.Count < buffer.Length)
            {
                buffer.RemoveAt(0, messagebuffer.Count);


                return buffer.Length - messagebuffer.Count;
            }
           else
            {
                return 0;
            }


        }

        Queue<List<byte>> Messageque = new Queue<List<byte>>();

        Queue<byte[]> BufferQueue = new Queue<byte[]>();

       public void AddToQueue(byte[] buffer)
        {
            BufferQueue.Enqueue(buffer);
        }

        public static MessageType ReadMessageTypeFromHeader(byte header)
        {
            byte val = (byte)(header & 0xf0);
            return (MessageType)(val >> 4);
        }


        public static string DecodeString(byte[] src, ref int pos)
        {
            int oldpos = pos;
            try
            {

                var encoder = new UTF8Encoding();
                var sb = new StringBuilder();
                int length = DecodeInt16(src, ref pos);

                if (length > 0)
                {
                    int start = pos;
                    pos += length;

                    char[] chars = encoder.GetChars(src, start, length);
                    sb.Append(chars);
                }

                return sb.ToString();
            }
            catch (System.ArgumentOutOfRangeException ex)
            {
                pos = oldpos;
                return "";

            }
        }

        public static int DecodeInt16(byte[] src, ref int pos)
        {
            if (pos >= src.Length || pos + 1 >= src.Length)
            {
                return 0;
            }
            byte msb = src[pos++];
            byte lsb = src[pos++];
            return (msb << 8) | lsb;
        }


        public void Dispose()
        {
            End = true;
      

            MessageThread.Join();
            BufferThread.Join();
        }


       
    }
}
