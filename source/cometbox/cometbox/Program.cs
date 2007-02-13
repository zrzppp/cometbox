using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace cometbox
{
    class Program
    {
        public static Config.AppConfig Configuration;

        static void Main(string[] args)
        {
            string configfile = "";

            if (args.Length == 0)
            {
                configfile = "cometbox.conf";
            }
            else
            {
                configfile = args[0];
            }

            //Config.AppConfig.BuildNewConfigFile(configfile);

            if (new FileInfo(configfile).Exists)
            {
                if ((Configuration = Config.AppConfig.LoadConfig(configfile)) == null)
                {
                    Console.WriteLine("Error loading configuration.");
                    GracefullyChokeAndDie();
                    return;
                }
            }
            else
            {
                Console.WriteLine("Configuration not found: " + configfile);
                GracefullyChokeAndDie();
                return;
            }

            WIServer wiserver = new WIServer(Configuration.WebInterface);

            //ServerInterfaceServer data = new ServerInterfaceServer(Dns.GetHostEntry(IPAddress.Parse(Config.ServerInterface.BindTo)).AddressList[0], Config.ServerInterface.Port);
            //WebInterfaceServer webint = new WebInterfaceServer();

            //while (webint.IsRunning())
            //{
            //    Thread.Sleep(1000);
            //}
        }

        static void GracefullyChokeAndDie()
        {
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }
    }
}
