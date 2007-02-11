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
        public static AppConfig Config;

        static void Main(string[] args)
        {
            BuildNewConfig();

            string configfile = "";

            if (args.Length == 0)
            {
                configfile = "cometbox.conf";
            }
            else
            {
                configfile = args[0];
            }

            if (new FileInfo(configfile).Exists)
            {
                if ((Config = LoadConfig(configfile)) == null)
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

            //ServerInterfaceServer data = new ServerInterfaceServer(Dns.GetHostEntry(IPAddress.Parse(Config.ServerInterface.BindTo)).AddressList[0], Config.ServerInterface.Port);
            WebInterfaceServer webint = new WebInterfaceServer(Dns.GetHostEntry(IPAddress.Parse(Config.WebInterface.BindTo)).AddressList[0], Config.WebInterface.Port);

            while (webint.IsRunning())
            {
                Thread.Sleep(1000);
            }
        }

        static void GracefullyChokeAndDie()
        {
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        static AppConfig LoadConfig(string configfile)
        {
            AppConfig c = null;
            try
            {
                using (FileStream fs = new FileStream(configfile, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
                    c = (AppConfig)serializer.Deserialize(fs);
                }
            }
            catch { }

            return c;
        }

        static void BuildNewConfig()
        {
            AppConfig c = new AppConfig();

            c.ClientInterface = new AppConfig.ClientInterfaceConfig();
            c.ClientInterface.Authentication = new AppConfig.AuthConfig();
            c.ClientInterface.Authentication.Type = AppConfig.AuthConfig.AuthType.None;
            c.ClientInterface.Authentication.Realm = "2cometbox Client Interface";
            c.ClientInterface.Authentication.Username = "none";
            c.ClientInterface.Authentication.Password = "none";

            c.WebInterface = new AppConfig.WebInterfaceConfig();
            c.WebInterface.Authentication = new AppConfig.AuthConfig();
            c.WebInterface.Authentication.Type = AppConfig.AuthConfig.AuthType.Basic;
            c.WebInterface.Authentication.Realm = "Cometbox Web Interface";
            c.WebInterface.Authentication.Username = "cometbox";
            c.WebInterface.Authentication.Password = "cometbox123";

            c.ServerInterface = new AppConfig.ServerInterfaceConfig();
            c.ServerInterface.LocalOnly = true;


            using (FileStream fs = new FileStream(@"cometbox.conf", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AppConfig));
                serializer.Serialize(fs, c);
                fs.Close();
            }

        }
    }
}
