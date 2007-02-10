using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace cometbox
{
	public class DataClient
	{
		private TcpClient client = null;
		private NetworkStream stream = null;
		byte[] read_buffer = new byte[1024];
		
		public DataClient(TcpClient c)
		{
			client = c;
					
			Console.WriteLine("DataClient: Initializing {0}", client.Client.RemoteEndPoint.ToString());
				
			stream = client.GetStream();
			
			stream.BeginRead(read_buffer, 0, 1024, new AsyncCallback(callbackRead), this);
		}
		
		public static void callbackRead(IAsyncResult ar) 
		{
			DataClient dc = (DataClient)ar.AsyncState;
			String data = "";
			int bytes = 0;
			
			bytes = dc.stream.EndRead(ar);
			if ( bytes <= 0 ) {
				Console.WriteLine("DataClient: Disconnected {0}", dc.client.Client.RemoteEndPoint.ToString());
				return;
			}
			data = String.Concat(data, Encoding.ASCII.GetString(dc.read_buffer, 0, bytes));
			
			Console.WriteLine(data);
			
			dc.stream.BeginRead(dc.read_buffer, 0, 1024, new AsyncCallback(DataClient.callbackRead), dc);
		}
	}
}
