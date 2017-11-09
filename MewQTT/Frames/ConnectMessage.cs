using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Frames
{
    public class ConnectMessage : IMQTTMessage
    {
        int _id;
        public int id { get => _id; set => _id = value; }

        int _messageLenght;
        public int MessageLenght { get => _messageLenght; set => _messageLenght = value; }
        public MessageType Type { get => MessageType.Connect; }
        public List<byte> _messagebytes { get; set; }




        //specific message
        public string ProtocalName { get; internal set; }
        public bool CorrectProtocol { get; internal set; }

        public int KeepAliveTime { get; internal set; }

        byte _connectionflags { get; set; }

        public string ClientId { get; internal set; }

        public bool UserNameFlag {
            get
            {
                return (_connectionflags & 128) == 128 ? true : false;

            }
        }

        public bool PasswordFlag
        {
            get
            {
                return (_connectionflags & 64) == 64 ? true : false;

            }
        }

        public bool WillRetainFlag
        {
            get
            {
                return (_connectionflags & 32) == 32 ? true : false;

            }
        }

        public int WillQosFlag
        {

            get
            {
                return 0;
            }
        }

        public bool WillFlag
        {
            get
            {
                return (_connectionflags & 4) == 4 ? true : false;
            }
        }

        public bool CleanSession
        {
            get
            {
                return (_connectionflags & 2) == 2 ? true : false;
            }
        }

        public bool Reserved
        {
            get
            {
                return (_connectionflags & 1) == 1 ? true : false;
            }
        }

        //properties 

        public string  WillTopic {get;set;}

        public string WillMessage { get; set; }


        public string UserName { get; set; }

        public string Password { get; set; }

        public void MakeMessage(List<byte> messagebytes)
        {
            MessageLenght = messagebytes.Count;
            _messagebytes = messagebytes;
            ReadPayload();
            
        }

        public int ReadVariableHeader()
        {
            int pos;
            ReadRemainingLength(out pos);
            ProtocalName = CommandParser.DecodeString(_messagebytes.ToArray(), ref pos);
           
            if(pos+1 < _messagebytes.Count)
            {
                CorrectProtocol = (_messagebytes[pos++] & 0x04) == 4? true : false;
                _connectionflags = _messagebytes[pos++];
            }

            KeepAliveTime = CommandParser.DecodeInt16(_messagebytes.ToArray(), ref pos);

            return pos;
            
        }


       

        public  int ReadRemainingLength(out int pos)
        {
            pos = 1;            // Remaining Length byte starts here
            int multiplier = 1;
            int value = 0;
            int digit = 0;

            do
            {
                digit = _messagebytes[pos++];
                value += (digit & 127) * multiplier;
                multiplier *= 128;
            } while ((digit & 128) != 0);

            return value;
        }

        public  void ReadPayload()
        {
            int pos = ReadVariableHeader();
            ClientId = CommandParser.DecodeString(_messagebytes.ToArray(), ref pos);
            if(WillFlag)
            {
                WillTopic = CommandParser.DecodeString(_messagebytes.ToArray(), ref pos);
                WillMessage = CommandParser.DecodeString(_messagebytes.ToArray(), ref pos);
            }

            if(UserNameFlag)
            {
                UserName = CommandParser.DecodeString(_messagebytes.ToArray(), ref pos); 
            }

            if(PasswordFlag)
            {
                Password = CommandParser.DecodeString(_messagebytes.ToArray(), ref pos);
            }



        }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
