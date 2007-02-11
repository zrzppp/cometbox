using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace cometbox.HTTP
{
    public class Response
    {
        public Status Status = HTTP.Status.OK;
		public string Mime = "text/html";
		public string Body = "";
        private Dictionary<string, string> Headers = new Dictionary<string,string>();
       
        public string GetResponse()
        {
            StringBuilder r = new StringBuilder();

            r.Append("HTTP/1.1 " + GetStatusString(Status) + "\r\n");
            r.Append("Server: cometbox\r\n");
            
            foreach (KeyValuePair<string, string> header in Headers)
            {
                r.Append(header.Key + ": " + header.Value + "\r\n");
            }
            if (Body.Length > 0)
            {
                r.Append("Content-Type: " + Mime + "\r\n");
                r.Append("Content-Length: " + Body.Length + "\r\n");
                r.Append("\r\n");
                r.Append(Body);
            }
            else
            {
                r.Append("\r\n");
            }
            return r.ToString();
        }

        public void AddHeader(string key, string value)
        {
            Headers.Add(key, value);
        }

        private string GetStatusString(HTTP.Status status)
        {
            switch (status)
            {
                case HTTP.Status.OK:
                    return "404 Ok";
                case HTTP.Status.NotAuthorized:
                    return "401 Not Authorized";
                case HTTP.Status.NotFound:
                    return "404 Not Found";
                default:
                    throw new Exception("Invalid HTTP response.");
            }
        }
    }
}
