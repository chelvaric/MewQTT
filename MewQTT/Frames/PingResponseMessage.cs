using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Frames
{
    class PingResponseMessage:IMQTTMessage
    {
        int _id;
        public int id { get => _id; set => _id = value; }

        int _messageLenght;
        public int MessageLenght { get => _messageLenght; set => _messageLenght = value; }
        public MessageType Type { get => MessageType.PingResp; }
        public List<byte> _messagebytes { get; set; }

        //message specific



        public void MakeMessage(List<byte> messagebytes)
        {
            MessageLenght = messagebytes.Count;
            _messagebytes = messagebytes;

        }

        public void ReadPayload()
        {


        }

        public int ReadVariableHeader()
        {
            return 0;
        }

        public int ReadRemainingLength(out int pos)
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


        public byte[] Serialize()
        {
            byte[] packet = new byte[4];
            packet[0] = 208;
            packet[1] = 0;


            return packet;
        }
    }
}
