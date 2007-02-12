using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Globalization;

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
            r.Append("Date: " + DateTime.Now.ToUniversalTime().ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'") + "\r\n"); 
            r.Append("Accept-Ranges: none\r\n");

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
            Console.WriteLine(r.ToString());
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
                    return "200 OK";
                case HTTP.Status.NotAuthorized:
                    return "401 Not Authorized";
                case HTTP.Status.NotFound:
                    return "404 Not Found";
                default:
                    throw new Exception("Invalid HTTP response.");
            }
        }

        public static Response Get404Response(string filename)
        {
            Response res = new Response();
            res.Status = Status.NotFound;
            res.Mime = "text/html";
            res.Body = @"<html>
<head>
    <title>404 Not found.</title>
</head>
<body>
    404: The file '" + filename + @"' cannot be found.
</body>
</html>";
            return res;
        }

        public static Response Get500Response(string errormsg)
        {
            Response res = new Response();
            res.Status = Status.NotFound;
            res.Mime = "text/html";
            res.Body = @"<html>
<head>
    <title>500 Internal server error.</title>
</head>
<body>
    500: There was an internal server error with the message: " + errormsg + @" 
</body>
</html>";
            return res;
        }

        public static Response GetFileResponse(FileInfo file, string Url)
        {
            if (file.Exists) {
                try {
                    string data = "";
                    using (FileStream s = file.OpenRead()) {
                        byte[] bytes = new byte[s.Length];
                        int bytesread = s.Read(bytes, 0, (int)s.Length);
                        data = Encoding.ASCII.GetString(bytes, 0, bytesread);
                    }
                    Response res = new Response();
                    res.Mime = GetMimeType(file.Extension);
                    res.Body = data;
                    return res;
                }
                catch (Exception e) {
                    return Get500Response("Error reading file: " + e);
                }
            } else {
                return Get404Response(file.Name + " (" + Url + ")");
            }
        }

        public static string GetMimeType(string e)
        {
            if (e == ".jpg") {
                return "image/jpeg";
            }
            return "text/html";
        }
    }
}
