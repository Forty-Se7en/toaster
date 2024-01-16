using Notification.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Windows.UI.Notifications;

namespace Notification
{
    internal class CommandBuilder
    {
        public static Windows.Foundation.TypedEventHandler<ToastNotification, object> BuildCommand(CommandModel commandModel)
        {            
            switch (commandModel.Type)
            {
                case "openLink": return (s, e) => {
                    OpenLink(commandModel.Data);
                };

                default: return (s, e) => { Console.WriteLine("Unknown command type"); };
            }

        }

        public static void OpenLink(string link) {
            try
            {
                var uri = link;
                var psi = new System.Diagnostics.ProcessStartInfo();
                psi.UseShellExecute = true;
                psi.FileName = uri;
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        public static void RunApp(string path, string[] args = null)
        {
            // Prepare the process to run
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            if (args != null && args.Length>0) { start.Arguments = String.Join(" ", args); }
            // Enter the executable to run, including the complete path
            start.FileName = path;
            // Do you want to show a console window?
            //start.WindowStyle = ProcessWindowStyle.Hidden;
            //start.CreateNoWindow = true;
            //int exitCode;
            Process.Start(start);

            //// Run the external process & wait for it to finish
            //using (Process proc = Process.Start(start))
            //{
            //    proc.WaitForExit();

            //    // Retrieve the app's exit code
            //    exitCode = proc.ExitCode;
            //}
        }

        public async static Task<HttpResponseMessage> MakeRequest(string baseUrl, string type, string data = null, TimeSpan? timeSpan = null)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = timeSpan ?? TimeSpan.FromSeconds(5);
            switch (type)
            {
                case "get":
                    return await client.GetAsync(baseUrl);
                case "post":
                    if (data == null)
                    {
                        Console.WriteLine("MakeRequest: bad post request, data is null");
                    }
                    var content = new StringContent(data, Encoding.UTF8);
                    return await client.PostAsync(baseUrl, content);
                case "put": Console.WriteLine("put request is not implemented"); return null;
                case "delete": Console.WriteLine("delete request is not implemented"); return null;
                default: Console.WriteLine($"unknown type of request: \"{type}\""); return null;
            }
        }

        public static void SendViaUDP(string ip, int port, byte[] data)
        {
            var client = new UdpClient(ip, port);
            client.Send(data, data.Length);
        }

        public static void SendViaTCP(string ip, int port, byte[] data)
        {
            //---create a TCPClient object at the IP and port no.---
            TcpClient client = new TcpClient(ip, port);
            NetworkStream nwStream = client.GetStream();

            nwStream.Write(data, 0, data.Length);

            ////---read back the text---
            //byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            //int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            //Console.WriteLine("Received : " + Encoding.ASCII.GetString(bytesToRead, 0, bytesRead));
            //Console.ReadLine();
            client.Close();
        }

        public static void ParseOpenLinkCommand(string arguments, IDictionary<string, object> inputs)
        {
            var link = HttpUtility.ParseQueryString(arguments).Get("link");
            var sourceId = HttpUtility.ParseQueryString(arguments).Get("sourceId[]")?.Split(',');

            if (string.IsNullOrEmpty(link)) { Console.WriteLine("Open link error: link invalid value"); return; }
            if (sourceId != null && sourceId.Length > 0)
            {
                foreach (var id in sourceId)
                {
                    dynamic value;
                    var result = inputs.TryGetValue(id, out value);
                    if (result)
                    {
                        if (!link.EndsWith("/"))
                        {
                            link += "/";
                        }
                        link += $"{id}/{value}";
                    }
                    else
                    {
                        Console.WriteLine($"Open link warning: failed get value from input id \"{id}\"");
                    }
                }
            }
            OpenLink(link);
        }

        public static void ParseRunAppCommand(string arguments, IDictionary<string, object> inputs)
        {
            var appPath = HttpUtility.ParseQueryString(arguments).Get("appPath");
            if (string.IsNullOrEmpty(appPath)) { Console.WriteLine("Run app error: app path invalid value"); return; }
            var sourceId = HttpUtility.ParseQueryString(arguments).Get("sourceId[]")?.Split(',');
            if (sourceId != null && sourceId.Length > 0)
            {
                List<string> values = new List<string>();
                foreach (var id in sourceId)
                {
                    dynamic value;
                    var result = inputs.TryGetValue(id, out value);
                    if (result)
                    {
                        values.Add(value);
                    }
                    else
                    {
                        Console.WriteLine($"Run app warning: failed get value from input id \"{id}\"");
                    }
                }
                RunApp(appPath);
            }
            else { RunApp(appPath); }
        }

        public static void ParseSendPackageCommand(string arguments, IDictionary<string, object> inputs)
        {
            var protocol = HttpUtility.ParseQueryString(arguments).Get("protocol");
            if (string.IsNullOrEmpty(protocol))
            {
                Console.WriteLine("send package error: protocol invalid value");
                return;
            }
            var ip = HttpUtility.ParseQueryString(arguments).Get("ip");
            if (string.IsNullOrEmpty(ip))
            {
                Console.WriteLine("send package error: ip invalid value");
                return;
            }
            var port = HttpUtility.ParseQueryString(arguments).Get("port");
            if (string.IsNullOrEmpty(port))
            {
                Console.WriteLine("send package error: port invalid value");
                return;
            }
            var data = HttpUtility.ParseQueryString(arguments).Get("data");

            if (data == null)
            {
                var sourceId = HttpUtility.ParseQueryString(arguments).Get("sourceId[]")?.Split(',');
                if (sourceId == null && sourceId.Length > 0)
                {
                    string json = "{\n";
                    foreach (var id in sourceId)
                    {
                        dynamic value;
                        var result = inputs.TryGetValue(id, out value);
                        if (result)
                        {

                            json += $"\"{id}\": \"{value}\"\n";
                        }
                        else
                        {
                            Console.WriteLine($"send package warning: failed get value from input id \"{id}\"");
                        }
                    }
                    json += "}";
                    data = json;
                }
                else
                {
                    Console.WriteLine("send package error: bad request: data is null, source id is null or empty");
                    return;
                }
            }

            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(data);
            int portInt;
            if (!int.TryParse(port, out portInt))
            {
                Console.WriteLine($"send package error: failed to convert port \"{port}\" to int");
            }
            switch (protocol)
            {
                case "tcp": SendViaTCP(ip, portInt, bytes); break;
                case "udp": SendViaUDP(ip, portInt, bytes); break;                
            }

        }

        public static void ParseRestCommand(string arguments, IDictionary<string, object> inputs)
        {
            var baseUrl = HttpUtility.ParseQueryString(arguments).Get("baseUrl");
            if (string.IsNullOrEmpty(baseUrl))
            {
                Console.WriteLine("rest command error: baseUrl invalid value");
                return;
            }
            var request = HttpUtility.ParseQueryString(arguments).Get("request");
            if (string.IsNullOrEmpty(request))
            {
                Console.WriteLine("rest command error: request invalid value");
                return;
            }            

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(5);

            switch (request)
            {
                case "get":
                    //client.GetAsync(baseUrl).ContinueWith((a) => { Console.WriteLine($"get result: {a.Result.StatusCode}"); });
                    MakeRequest(baseUrl, request).ContinueWith((a) => { Console.WriteLine($"post result: {a.Result.StatusCode}"); });
                    break;
                case "post":
                    var data = HttpUtility.ParseQueryString(arguments).Get("data");
                    if (data == null)
                    {
                        var sourceId = HttpUtility.ParseQueryString(arguments).Get("sourceId[]")?.Split(',');
                        if (sourceId == null && sourceId.Length > 0)
                        {
                            string json = "{\n";
                            foreach (var id in sourceId)
                            {
                                dynamic value;
                                var result = inputs.TryGetValue(id, out value);
                                if (result)
                                {

                                    json += $"\"{id}\": \"{value}\"\n";
                                }
                                else
                                {
                                    Console.WriteLine($"rest command warning: failed get value from input id \"{id}\"");
                                }
                            }
                            json += "}";
                            data = json;
                        }
                        else
                        {
                            Console.WriteLine("rest command error: bad request: data is null, source id is null or empty");
                            return;
                        }
                    }
                    //if (data == null)
                    //{
                    //    Console.WriteLine($"rest command error: content for post request is null");
                    //    return;
                    //}
                    MakeRequest(baseUrl, request, data).ContinueWith((a) => { Console.WriteLine($"post result: {a.Result.StatusCode}"); });


                    //if (data != null)
                    //{
                    //    content = new StringContent(data, Encoding.UTF8);                        
                    //}
                    //else if (sourceId != null && sourceId.Length > 0)
                    //{
                    //    string json = "{\n";
                    //    foreach (var id in sourceId)
                    //    {
                    //        dynamic value;
                    //        var result = inputs.TryGetValue(id, out value);
                    //        if (result)
                    //        {

                    //            json += $"\"{id}\": \"{value}\"\n";
                    //        }
                    //        else
                    //        {
                    //            Console.WriteLine($"rest command warning: failed get value from input id \"{id}\"");
                    //        }
                    //    }
                    //    json += "}";
                    //    content = new StringContent(json, Encoding.UTF8);
                    //}
                    //if (content == null)
                    //{
                    //    Console.WriteLine($"rest command error: content for post request is null");
                    //    return;
                    //}
                    //client.PostAsync(baseUrl, content).ContinueWith((a) => { Console.WriteLine($"post result: {a.Result.StatusCode}"); });

                    //HttpRequestMessage requestMessage = new HttpRequestMessage()
                    //{
                    //    Content = content,
                    //    Method = HttpMethod.Post,
                    //    RequestUri = new Uri(baseUrl)
                    //};

                    //client.SendAsync(requestMessage);

                    break;
                case "put": Console.WriteLine("put request is not implemented"); break;
                case "delete": Console.WriteLine("delete request is not implemented"); break;
                default: Console.WriteLine($"request command error: unknown type of request: \"{request}\""); break;
            }
        }
    }
}
