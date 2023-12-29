using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ChatServerInterface;

namespace ChatServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string url = "net.tcp://localhost:8201/ChatServer"; // Connect to client using this URL
            Console.WriteLine("Launching Chat Server");
            ServiceHost host = null;

            try
            {
                NetTcpBinding tcpBinding = new NetTcpBinding();
                host = new ServiceHost(typeof(ServerImplementation));
                host.AddServiceEndpoint(typeof(IChatServer), tcpBinding, url);
                host.Open();
                Console.WriteLine("Chat Server Operational");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                host?.Close();
            }
        }

        /* TEST MAIN METHOD DELTE BEFORE SUBMISSION*/
        /*        static void Main(string[] args)
                {
                    Console.WriteLine("Server Started");
                    ServerImplementation server = new ServerImplementation();
                    string username = "vansitha";
                    Console.WriteLine(server.Login(username));

                    Console.WriteLine(server.Login("vanuja"));

                    Console.WriteLine(server.Login("vansitha"));

                    Console.WriteLine("-------Joining Chat Roomm-----");

                    server.JoinChatRoom("randomRoom", "vansitha");
                    server.JoinChatRoom("randomRoom", "vanuja");

                    server.Login("apishenth");
                    server.JoinChatRoom("randomRoom", "apishenth");

                    Console.ReadLine();
                }*/
    }
}
