using System;
using System.IO;
using System.Net;

namespace BuildSystem
{
    internal static class Utils
    {
        public static string ExecuteHttpRequest(string request)
        {
            WebRequest webRequest = WebRequest.Create(request);
            WebResponse response = webRequest.GetResponse();
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            return reader.ReadToEnd();
        }
    }
}
