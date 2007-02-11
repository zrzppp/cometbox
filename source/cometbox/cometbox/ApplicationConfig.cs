using System;
using System.Collections.Generic;
using System.Text;

namespace cometbox
{
    public class AppConfig
    {
        public struct AuthConfig
        {
            public enum AuthType
            {
                None,
                Basic,
                Digest
            }

            public AuthType Type;
            public string Username;
            public string Password;
            public string Realm;
        }

        public class ServerInterfaceConfig
        {
            public bool LocalOnly = true;
            public string[] AcceptedIPs;
            public string BindTo = "127.0.0.1";
            public int Port = 1802;
        }

        public class ClientInterfaceConfig
        {
            public AuthConfig Authentication;
            public string BindTo = "127.0.0.1";
            public int Port = 1800;
        }

        public class WebInterfaceConfig
        {
            public AuthConfig Authentication;
            public string BindTo = "127.0.0.1";
            public int Port = 1801;
        }

        public ServerInterfaceConfig ServerInterface;
        public ClientInterfaceConfig ClientInterface;
        public WebInterfaceConfig WebInterface;
    }
}
