using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace cometbox
{
	public class WebInterfaceServer
	{
		private Thread t = null;
		private static TcpListener server = null;

		public WebInterfaceServer(IPAddress ip, Int32 port)
		{
			Console.WriteLine("WebInterfaceServer: Starting on {0}:{1}.", ip.ToString(), port);
			server = new TcpListener(ip, port);
			server.Start();
		
			t = new Thread(new ThreadStart(Loop));
			t.Start();
		}
				
		private static void Loop()
		{
			List<WebInterfaceClient> clients = new List<WebInterfaceClient>();
	
			while ( true ) {
				TcpClient client = server.AcceptTcpClient();

				WebInterfaceClient dc = new WebInterfaceClient(client);
				clients.Add(dc);
				
				int i = 0;
				while (i < clients.Count) {
					if ( !clients[i].IsLive ) {
						clients.RemoveAt(i);
					}
					i++;
				}
			}
			
			Console.WriteLine("WebInterfaceServer: Exiting thread.");
		}
		
		public bool IsRunning()
		{
			return t.IsAlive;
		}
	}
}
