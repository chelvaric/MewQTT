using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Frames
{
    sealed class FixedHeaderFrame
    {
        private readonly byte[] _buffer = new byte[5];
        private int _bufferPos;


        public bool HeaderComplete { get; internal set; }

        public int HeaderSize
        {
            get
            {
                if (!HeaderComplete)
                {
                    throw new InvalidOperationException("Header size cannot be determined until the fixed header is complete.");
                }
                return _bufferPos;
            }
        }


        public MessageType Type
        {
            get
            {
                return CommandParser.ReadMessageTypeFromHeader(_buffer[0]);
            }
        }

        public byte[] Buffer
        {
            get
            {
                return _buffer;
            }
        }


        public void CreateMessageBuffer(ref List<byte> messagebuffer)
        {
            if (!HeaderComplete)
            {
                throw new InvalidOperationException("Cannot prepend until the fixed header is complete.");
            }

            
            AddHeader(ref messagebuffer);
         

        }


        public void AddHeader(ref List<byte> msgbuffer)
        {
            for (int i = 0; i < HeaderSize; i++)
            {
                msgbuffer.Add(_buffer[i]);
            }

        }

        public bool AppendByte(byte b)
        {
            if (HeaderComplete)
            {
                throw new InvalidOperationException("The fixed header is complete. Cannot append more data to it.");
            }

            if (_bufferPos > 4)
            {
                throw new InvalidOperationException("Unexpected fixed header data.");
            }

            _buffer[_bufferPos] = b;
            _bufferPos++;

            HeaderComplete = ((_bufferPos != 1) && (b & 128) == 0);
            return !HeaderComplete;
        }

        public int RemainingLenght
        {
            get{
                int pos = 1;            // Remaining Length byte starts here
                int multiplier = 1;
                int value = 0;
                int encodedByte = 0;

                do
                {
                    encodedByte = _buffer[pos++];
                    value += (encodedByte & 127) * multiplier;
                    multiplier *= 128;
                } while ((encodedByte & 128) != 0);

                return value;

            }
        }
    }
}
