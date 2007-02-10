using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace cometbox
{
	public class WebInterfaceClient
	{
		private struct HTTPResponse 
		{
			public string status;
			public string mime;
			public string body;
			
			public HTTPResponse(string s, string m, string b) 
			{
				status = s;
				mime = m;
				body = b;
			}
		}
	
		private TcpClient client = null;
		private NetworkStream stream = null;
		private WebInterfaceServer parent = null;
		byte[] read_buffer = new byte[1024];
		string buffer = "";
		int bufferpos = 0;
		string AuthPassword = "cometbox123";
		
		public bool IsLive = true;

		public WebInterfaceClient(TcpClient c)
		{
			client = c;
					
			stream = client.GetStream();
			
			stream.BeginRead(read_buffer, 0, 1024, new AsyncCallback(callbackRead), this);
		}
		
		public static void callbackRead(IAsyncResult ar) 
		{
			WebInterfaceClient dc = (WebInterfaceClient)ar.AsyncState;
			int bytes = 0;
			
			try {
				bytes = dc.stream.EndRead(ar);
				if ( bytes <= 0 ) {
					Console.WriteLine("WebInterfaceClient: Remote disconnect {0}.", dc.client.Client.RemoteEndPoint.ToString());
					dc.CleanUp();
					return;
				}
				
				dc.buffer += Encoding.ASCII.GetString(dc.read_buffer, 0, bytes);
				dc.Handle();
				
				if ( dc.stream != null ) {
					dc.stream.BeginRead(dc.read_buffer, 0, 1024, new AsyncCallback(WebInterfaceClient.callbackRead), dc);
				}
			} catch (Exception e) {
				Console.WriteLine("WebInterfaceClient: Error in callbackRead() - {0}", e.Message);
			}
		}
		
		private void Handle()
		{
			string[] datas = null;
			string[] pair = null;
			bool authGiven = false;
			
			int pos;
			if ((pos = buffer.IndexOf("\r\n\r\n", bufferpos)) >= 0) {
				string[] splitters = {"\r\n"};
				datas = buffer.Split(splitters, StringSplitOptions.None);
			} else if ((pos = buffer.IndexOf("\n\n", bufferpos)) >= 0) {
				string[] splitters = {"\n"};
				datas = buffer.Substring(0, pos+1).Split(splitters, StringSplitOptions.None);
			}
			
			for ( int i=1; i<datas.Length; i++ ) {
				pair = datas[i].Split(':');
				if ( pair[0] == "Authorization" ) {
					string[] splitters = {" "};
					pair = pair[1].Split(splitters, StringSplitOptions.RemoveEmptyEntries);
					if ( pair[0] == "Basic" && DecodeBase64(pair[1]) == ":" + AuthPassword ) {
						authGiven = true;
					}
					break;
				}
			}
			
			
			if ( datas != null && authGiven ) {
				HTTPResponse res = GetResponse(datas[0].Split(' '));
				
				string r = "HTTP/1.0 " + res.status + "\r\n";
				r += "Server: cometbox Web Interface\r\n";
				r += "Content-Type: " + res.mime + "\r\n";
				r += "Content-Length: " + res.body.Length + "\r\n";
				r += "\r\n";
				r += res.body;
				
				Send(r);

				Console.WriteLine("WebInterfaceClient: Handled {0}. Status {1}. Length {2}.", datas[0], res.status, res.body.Length);
				
				CleanUp();
			} else if ( datas != null && !authGiven ) {
				//no auth, send headers
				
				string r = "HTTP/1.0 401 Unauthorized\r\n";
				r += "Server: cometbox Web Interface\r\n";
				r += "WWW-Authenticate: Basic realm=\"cometbox web interface\"\r\n";
				r += "\r\n";
				
				Send(r);
				
				Console.WriteLine("WebInterfaceClient: Auth challenge sent.");
			
				CleanUp();
			}
		}
		
		private void Send(string data) 
		{
			try {
				byte[] bytes = Encoding.ASCII.GetBytes(data);
				stream.Write(bytes, 0, bytes.Length);
			} catch (Exception e) {
				Console.WriteLine("WebInterfaceClient: Error in Send() - {0}", e.Message);
				CleanUp();
			}
		}
		
		private HTTPResponse GetResponse(string[] info) 
		{
			if (info[1] == "/") {
				return GetPage_Root();
			} else if (info[1] == "/other") {
				return GetPage_Other();
			}
			return new HTTPResponse("404 Not Found", "text/html", "<html><body>Page not found.</body></html>");
		}
		
		private HTTPResponse GetPage_Root()
		{
			return new HTTPResponse("200 OK", "text/html", @"<html>
<head>
	<title>Test Root</title>
</head>
<body>
	Test root page...<br>
	<a href=""other"">Other</a>
</body>
</html>");
		}
		
		private HTTPResponse GetPage_Other()
		{
			return new HTTPResponse("200 OK", "text/html", "<html><head><title>Test Root</title></head><body>Other Page</body></html>");
		}
		
		public void CleanUp() 
		{
			try {
				stream.Close();
			} catch {}
			try {
				stream.Dispose();
			} catch {}
			try {
				client.Close();
			} catch {}
			
			stream = null;
			client = null;
			
			IsLive = false;
		}
		
		
		public string DecodeBase64(string str)
		{
			try {
				return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(str));
			} catch {
				return "";
			}
		}
	}
}


