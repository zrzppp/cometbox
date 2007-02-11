using System;
using System.Collections.Generic;
using System.Text;

namespace cometbox.Config
{
    public class ServerInterfaceConfig
    {
        public bool LocalOnly = true;
        public string[] AcceptedIPs;
        public string BindTo = "127.0.0.1";
        public int Port = 1802;


    }
}
