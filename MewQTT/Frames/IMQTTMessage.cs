using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Frames
{
   public interface IMQTTMessage
    {

        int id { get; set; }

        int MessageLenght { get; set; }

        MessageType Type { get; }

        List<byte> _messagebytes { get; set; }

        void MakeMessage(List<byte> messagebytes);

        int ReadVariableHeader();

        void ReadPayload();


        byte[] Serialize();



    }
}
