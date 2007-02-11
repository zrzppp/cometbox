using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace cometbox.HTTP
{
    public class Client
    {
        private TcpClient client = null;
        private NetworkStream stream = null;

        byte[] read_buffer = new byte[1024];
        string buffer = "";
        int bufferpos = 0;
        ParseState state = ParseState.Start;
        HTTP.Request request;

        public bool IsLive = true;

        public Client(TcpClient c, Server server)
        {
            client = c;

            stream = client.GetStream();

            stream.BeginRead(read_buffer, 0, 1024, new AsyncCallback(callbackRead), this);
        }

        public static void callbackRead(IAsyncResult ar)
        {
            Client dc = (Client)ar.AsyncState;
            int bytes = 0;

            //try {
            bytes = dc.stream.EndRead(ar);
            if (bytes <= 0) {
                dc.CleanUp();
                return;
            }

            dc.buffer += Encoding.ASCII.GetString(dc.read_buffer, 0, bytes);

            Console.WriteLine(dc.buffer);

            dc.ParseInput();

            if (dc.stream != null) {
                dc.stream.BeginRead(dc.read_buffer, 0, 1024, new AsyncCallback(Client.callbackRead), dc);
            }
            //} catch (Exception e) {
            //	Console.WriteLine("Client: Error in callbackRead() - {0}", e.Message);
            //}
        }

        private enum ParseState
        {
            Start,
            Headers,
            Content,
            Done
        }

        private void ParseInput()
        {
            int pos;
            string temp;
            bool skip = false;
            while (bufferpos < buffer.Length - 1 && state != ParseState.Done && !skip) {
                switch (state) {
                    case ParseState.Start:
                        if ((pos = buffer.IndexOf("\r\n", bufferpos)) >= 0) {
                            temp = buffer.Substring(bufferpos, pos - bufferpos);
                            bufferpos = pos + 2;

                            string[] parts = temp.Split(' ');
                            if (parts.Length == 3) {
                                request = new HTTP.Request();

                                request.Method = parts[0];
                                request.Url = parts[1];
                                request.Version = parts[2];

                                state = ParseState.Headers;
                            } else {
                                CleanUp();
                            }
                        }
                        break;
                    case ParseState.Headers:
                        if ((pos = buffer.IndexOf("\r\n", bufferpos)) >= 0) {
                            temp = buffer.Substring(bufferpos, pos - bufferpos);
                            bufferpos = pos + 2;

                            if (temp.Length > 0) {
                                string[] parts = temp.Split(new string[1] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 2) {
                                    request.Headers.Add(parts[0], parts[1]);
                                }
                            } else {
                                if (request.HasContent()) {
                                    state = ParseState.Content;
                                } else {
                                    state = ParseState.Done;
                                }
                            }
                        }
                        break;
                    case ParseState.Content:
                        if (bufferpos + request.ContentLength <= buffer.Length) {
                            int t = bufferpos + request.ContentLength - 1;
                            request.Body = buffer.Substring(bufferpos, request.ContentLength);
                            bufferpos += request.ContentLength;

                            state = ParseState.Done;
                        } else {
                            skip = true;
                        }
                        break;
                }
            }

            if (state == ParseState.Done) {
                HandleRequest();
                state = ParseState.Start;
                request = null;
            }
        }

        private void HandleRequest()
        {
            HTTP.Response response;

            if (!request.VerifyAuth(Program.Configuration.WebInterface.Authentication)) {
                response = GetResponse_401();
            } else {
                if (request.Url == "/") {
                    response = GetResponse_Root();
                } else if (request.Url == "/form") {
                    response = GetResponse_Form();
                } else {
                    response = GetResponse_404();
                }
            }

            string res = response.GetResponse();
            Send(res);
        }

        private HTTP.Response GetResponse_Form()
        {
            HTTP.Response response = new HTTP.Response();
            response.Body = @"<html><head><title>Test Root</title></head><body>";
            response.Body += request.Body;
            response.Body += "</body></html>";
            return response;
        }

        private HTTP.Response GetResponse_Root()
        {
            HTTP.Response response = new HTTP.Response();
            response.Body = @"<html><head><title>Test Root</title></head><body>
<form action=""/form"" method=""post"">
<input type=""text"" name=""testbox"" /><br />
<input type=""text"" name=""otherbox"" /><br />
<input type=""submit"" value=""   Submit   "" />
</form>
</body></html>";
            return response;
        }

        private HTTP.Response GetResponse_401()
        {
            HTTP.Response response = new HTTP.Response();
            response.Status = HTTP.Status.NotAuthorized;
            response.AddHeader("WWW-Authenticate", "Basic realm=\"" + Program.Configuration.WebInterface.Authentication.Realm + "\"");
            response.Body = @"<html><head><title>Not authorized.</title></head><body>You are not authorized to view document.</body></html>";
            return response;
        }

        private HTTP.Response GetResponse_404()
        {
            HTTP.Response response = new HTTP.Response();
            response.Status = HTTP.Status.NotFound;
            response.Body = @"<html><head><title>Document not found.</title></head><body>Sorry that is not a valid cometbox document.</body></html>";
            return response;
        }

        private void Send(string data)
        {
            try {
                byte[] bytes = Encoding.ASCII.GetBytes(data);
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e) {
                Console.WriteLine("Client: Error in Send() - {0}", e.Message);
                CleanUp();
            }
        }

        public void CleanUp()
        {
            try {
                stream.Close();
            }
            catch { }
            try {
                stream.Dispose();
            }
            catch { }
            try {
                client.Close();
            }
            catch { }

            stream = null;
            client = null;

            IsLive = false;
        }
    }
}


