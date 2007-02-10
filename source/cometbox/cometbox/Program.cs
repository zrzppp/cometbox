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

        static void BuildNewConfig()
        {
            ApplicationConfig c = new ApplicationConfig();

            c.ClientInterface = new ApplicationConfig.ClientInterfaceConfig();
            c.ClientInterface.Authentication = new ApplicationConfig.AuthenticationConfig();
            c.ClientInterface.Authentication.Type = ApplicationConfig.AuthenticationConfig.AuthenticationType.None;
            c.ClientInterface.Authentication.Realm = "cometbox Client Interface";
            c.ClientInterface.Authentication.Username = "none";
            c.ClientInterface.Authentication.Password = "none";

            c.WebInterface = new ApplicationConfig.WebInterfaceConfig();
            c.WebInterface.Authentication = new ApplicationConfig.AuthenticationConfig();
            c.WebInterface.Authentication.Type = ApplicationConfig.AuthenticationConfig.AuthenticationType.Basic;
            c.WebInterface.Authentication.Realm = "cometbox Web Interface";
            c.WebInterface.Authentication.Username = "cometbox";
            c.WebInterface.Authentication.Password = "cometbox123";

            c.ServerInterface = new ApplicationConfig.ServerInterfaceConfig();
            c.ServerInterface.LocalOnly = true;


            using (FileStream fs = new FileStream(@"cometbox.conf", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ApplicationConfig));
                serializer.Serialize(fs, c);
            }

        }
    }
}
