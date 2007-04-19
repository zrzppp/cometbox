using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace cometbox
{
    public class SIServer : HTTP.Server
    {
        Config.ServerInterfaceConfig config;

        public SIServer(Config.ServerInterfaceConfig c)
            : base(Dns.GetHostEntry(IPAddress.Parse(c.BindTo)).AddressList[0], c.Port, c.Authentication)
        {
            config = c;
        }

        public override HTTP.Response HandleRequest(cometbox.HTTP.Request request)
        {
            try {
                XmlSerializer s = new XmlSerializer(typeof(SIRequest));
                SIRequest data = (SIRequest)s.Deserialize(new XmlTextReader(request.Body));
                

                return HTTP.Response.Get500Response("Parsed.");
            } catch (Exception e) {
                return HTTP.Response.Get500Response("Error parsing xml: " + e.Message);
            }
        }

        public class SIRequest
        {
            public enum CommandType
            {
                Queue,
                Remove
            }

            public CommandType Command = CommandType.Queue;
            public string User = "";
            public string[] Messages;
        }
    }
}
