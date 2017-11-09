using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Models
{
   public class Settings
    {

        public int port { get; set; }

        public string ip { get; set; }

        public List<Acount> acounts { get; set; }

        public bool useAuthentication { get; set; }

    }
}
