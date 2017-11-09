using MewQTT.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Models
{
    class ClientSession
    {

        public List<string> Subscriptions { get; set; }
        public List<IMQTTMessage> NotComfirmedByClient { get; set; }
        public List<IMQTTMessage> NotComfirmedByServer { get; set; }
        public List<IMQTTMessage> NewMessages { get; set; }

    }
}
