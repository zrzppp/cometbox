using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace cometbox
{
    class WIServer : HTTP.Server
    {
        Config.WebInterfaceConfig config;

        public WIServer(Config.WebInterfaceConfig c)
            : base(Dns.GetHostEntry(IPAddress.Parse(c.BindTo)).AddressList[0], c.Port)
        {
            config = c;
        }

        public override cometbox.HTTP.Response HandleRequest(cometbox.HTTP.Request request)
        {
           
        }
    }
}
