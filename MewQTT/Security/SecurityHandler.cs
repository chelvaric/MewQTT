using MewQTT.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MewQTT.Security
{
    class SecurityHandler
    {

        public List<Acount> Acounts { get; set; }

        public bool CheckAuth(string user,string pass)
        {
            if (Acounts.Exists(x => x.userName == user && x.password == pass))
                return true;
            else
                return false;
        }


    }
}
