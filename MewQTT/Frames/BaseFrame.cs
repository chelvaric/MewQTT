using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Frames
{
   public class BaseFrame
    {

        public BaseFrame()
        {
            Flags = new int[4];
        }

        public bool GetData(byte[] data)
        {
            byte[] remainder = new byte[data.Length-1];

            if ((data[0] & 128) == 128)
                ControlPacketType += 8;

            if ((data[0] & 64) == 64)
                ControlPacketType += 4;

            if ((data[0] & 32) == 32)
                ControlPacketType += 2;

            if ((data[0] & 16) == 16)
                ControlPacketType += 1;

            if ((data[0] & 8) == 8)
                Flags[3] = 1;

            if ((data[0] & 4) == 4)
                Flags[2] = 1;

            if ((data[0] & 2) == 2)
                Flags[1] = 1;

            if ((data[0] & 1) == 1)
                Flags[0] = 1;

            Array.Copy(data, 1, remainder, 0, data.Length - 1);

            Size = DecodeLenght(remainder, out int amount);

            if (Size > -1)
            {
                Payload = new byte[Size];
                Array.Copy(remainder, amount, Payload, 0, Size);

                if (remainder.Length > Size)
                    return true;
                else
                {
                    RemainingSize = Payload.Length - Size;
                    return false;
                }
            }
            else
                throw new Exception();
        }

       public int ControlPacketType { get; set; }

        public int[] Flags { get; set; }

        public int Size { get; set; }

        public int RemainingSize { get; set; } 

        public byte[] Payload { get; set; }


        public int DecodeLenght(byte[] lenght, out int amount)
        {
            amount = 0;
            int multiplier = 1;
            int value = 0;
            int i = 0;
            byte encodedbyte;
            do
            {
                encodedbyte = lenght[i];
                value += (encodedbyte & 127) * multiplier;
                multiplier *= 128;
                if (multiplier > 128 * 128 * 128)
                    return -1;

                i++;


            }
            while ((encodedbyte & 128) > 0);

            amount = i;
            return value;
        }
    }
}
