using System;
using System.Collections.Generic;
using System.Text;

namespace cometbox
{
    public class ApplicationConfig
    {
        public struct AuthenticationConfig
        {
            public enum AuthenticationType
            {
                None,
                Basic,
                Digest
            }

            public AuthenticationType Type;
            public string Username;
            public string Password;
            public string Realm;
        }

        public class ServerInterfaceConfig
        {
            public bool LocalOnly;
        }

        public class ClientInterfaceConfig
        {
            public AuthenticationConfig Authentication;
        }

        public class WebInterfaceConfig
        {
            public AuthenticationConfig Authentication;
        }

        public ServerInterfaceConfig ServerInterface;
        public ClientInterfaceConfig ClientInterface;
        public WebInterfaceConfig WebInterface;
    }
}
