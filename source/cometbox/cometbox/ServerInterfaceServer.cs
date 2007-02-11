using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace cometbox
{
	public class ServerInterfaceServer
	{
		private Thread t = null;
		private static TcpListener server = null;
	
		public ServerInterfaceServer(IPAddress ip, Int32 port)
		{
			Console.WriteLine("DataServer: Starting on {0}:{1}.", ip.ToString(), port);
			server = new TcpListener(ip, port);
			server.Start();
		
			t = new Thread(new ThreadStart(Loop));
			t.Start();
		}
		
		private static void Loop()
		{
			while ( true ) {
				Console.WriteLine("DataServer: Waiting for new client.");
				
				TcpClient client = server.AcceptTcpClient();

				ServerInterfaceClient dc = new ServerInterfaceClient(client);
			}
		}
		
		public bool IsRunning()
		{
			return t.IsAlive;
		}
	}
}
