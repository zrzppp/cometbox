using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace cometbox
{
    class SIServer : HTTP.Server
    {
        Config.ServerInterfaceConfig config;

        public SIServer(Config.ServerInterfaceConfig c)
            : base(Dns.GetHostEntry(IPAddress.Parse(c.BindTo)).AddressList[0], c.Port, c.Authentication)
        {
            config = c;
        }

        public override HTTP.Response HandleRequest(cometbox.HTTP.Request request)
        {


            return HTTP.Response.Get500Response("Not implemented");
        }
    }
}
