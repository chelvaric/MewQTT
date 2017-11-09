using MewQTT.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT
{
    class SubscriptionHandler
    {

      


        object Lock = new object();
        public void Unscribe(Client client,string topic)
        {
            if (client != null)
            {
                lock (Lock)
                {

                   
                }
            }


        }

        public void Subscribe(Client client, List<Subscription> subs)
        {
            if(client != null && subs != null)
            {
                lock (Lock)
                {

                }
            }
        }

    }
}
