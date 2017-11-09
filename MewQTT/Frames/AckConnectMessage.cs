using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Frames
{
    class AckConnectMessage : IMQTTMessage
    {
        int _id;
        public int id { get => _id; set => _id = value; }

        int _messageLenght;
        public int MessageLenght { get => _messageLenght; set => _messageLenght = value; }
        public MessageType Type { get => MessageType.ConnAck; }
        public List<byte> _messagebytes { get; set; }

        //message specific

        public int SessionId { get; set; }

        public int ReturnCode { get; set; }

        public void MakeMessage(List<byte> messagebytes)
        {
            MessageLenght = messagebytes.Count;
            _messagebytes = messagebytes;
            ReadPayload();
        }

        public void ReadPayload()
        {
            int pos = ReadVariableHeader();
            SessionId = _messagebytes[pos++];
            ReturnCode = _messagebytes[pos++];
        }

        public int ReadVariableHeader()
        {
            int pos;
             ReadRemainingLength(out pos);
            return pos;
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
            packet[0] = 32;
            packet[1] = 2;
            packet[2] = (byte)SessionId;
            packet[3] = (byte)ReturnCode;

            return packet;
        }
    }
}
