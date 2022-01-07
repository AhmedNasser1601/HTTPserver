using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog .
            serverSocket.Listen(int.MaxValue);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //Accept a client Socket (will block until a client connects)
                Socket client = serverSocket.Accept();

                //RemoteEndPoint Gets the IP address and Port number of the client
                Console.WriteLine("New client accepted: {0}", client.RemoteEndPoint);

                //Create a thread that works on ClientConnection.HandleConnectionmethod
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));

                //Start the thread
                newThread.Start(client);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket client = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            client.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] data = new byte[1024];
                    int receivedLength = client.Receive(data);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data));
                    Console.WriteLine(Encoding.ASCII.GetString(data));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    Console.WriteLine(response.ResponseString);

                    // TODO: Send Response back to client
                    client.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            client.Close();
        }

        Response HandleRequest(Request request)
        {
            // throw new NotImplementedException();

            string content;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, "text/html", content, GetRedirectionPagePathIFExist(Configuration.BadRequestDefaultPageName));
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string fullPath = Configuration.RootPath + request.relativeURI;

                //TODO: check for redirect
                if (!GetRedirectionPagePathIFExist(request.relativeURI).Equals(string.Empty))
                {
                    content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                    return new Response(StatusCode.Redirect, "text/html", content, GetRedirectionPagePathIFExist(request.relativeURI));
                }

                //TODO: check file exists
                if (!File.Exists(fullPath))
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, "text/html", content, GetRedirectionPagePathIFExist(Configuration.NotFoundDefaultPageName));
                }

                //TODO: read the physical file
                content = File.ReadAllText(fullPath);

                // Create OK response
                return new Response(StatusCode.OK, "text/html", content, GetRedirectionPagePathIFExist(fullPath));
            }

            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);

                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, "text/html", content, GetRedirectionPagePathIFExist(Configuration.InternalErrorDefaultPageName));
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            for (int i = 0; i < Configuration.RedirectionRules.Count; i++)
            {
                if ("/"+Configuration.RedirectionRules.Keys.ElementAt(i) == relativePath)
                {
                    return Configuration.RedirectionRules.Values.ElementAt(i);
                }
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);

            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception(defaultPageName + " doesn't exist"));
                return string.Empty;
            }

            // else read file and return its content
            return File.ReadAllText(filePath);
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 

               
                string[] rules = File.ReadAllLines(filePath);
                Configuration.RedirectionRules = new Dictionary<string, string>();

                // then fill Configuration.RedirectionRules dictionary 
                for (int i = 0; i < rules.Length; i++)
                {
                    Configuration.RedirectionRules.Add(rules[i].Split(',')[0], rules[i].Split(',')[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
