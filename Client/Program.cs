using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;

namespace Client
{
    class Program //Client side
    {
        public static void Send(NetworkStream stream)
        {
            //Send messages to Server
            var sendtoserverMessage = Console.ReadLine();
            var comm = new communicate {Protomessage = sendtoserverMessage};
            comm.WriteDelimitedTo(stream);
        }

        public static void Receive(NetworkStream stream)
        {
            //Receive messages from Server
            var comm = communicate.Parser.ParseDelimitedFrom(stream);
            Console.WriteLine("Server:" + comm.Protomessage);
        }
        static void Main(string[] args)
        {
            var tcpClient = new TcpClient("127.0.0.1", 1988);
            Console.WriteLine("----------------------CLIENT--------------------------");
            var stream = tcpClient.GetStream();
            Console.WriteLine("Successfully connected to the server");
            Console.WriteLine("");
            Console.WriteLine("WELCOME TO THE ANIMAL GUESSING GAME!");
            Console.WriteLine("");
            Console.WriteLine("Think of one of the following animals: Eagle, Dog, Fish, Cobra, Crab, or Gorilla");
            Console.WriteLine("You will be asked a series of questions. Simply answer yes or no.");
            Console.WriteLine("");

            //Data exchange
            while (true)
            {
                Receive(stream);
                Send(stream);
            }
            
            Console.ReadKey();

        }
    }
}
