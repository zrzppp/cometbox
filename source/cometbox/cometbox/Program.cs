using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace cometbox
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress dataip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            Int32 dataport = 1800;

            IPAddress webintip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            Int32 webintport = 1801;

            DataServer data = new DataServer(dataip, dataport);
            WebInterfaceServer webint = new WebInterfaceServer(webintip, webintport);

            while (data.IsRunning())
            {
                Thread.Sleep(0);
            }
        }
    }
}
