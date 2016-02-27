using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;


namespace Server
{

    class Program //Server side
    {
        public static void Send(NetworkStream stream, String sendtoclientMessage)
        {
            //Send message to Client
            var comm = new communicate {Protomessage = sendtoclientMessage};
            comm.WriteDelimitedTo(stream);
        }

        public static string Receive(NetworkStream stream,TcpClient tcpClient)
        { 
            //Receive message from Client
            var comm = communicate.Parser.ParseDelimitedFrom(stream);
            var receivefromclientMessage = comm.Protomessage;
            Console.WriteLine("Client:" + receivefromclientMessage);

            return receivefromclientMessage;
        }

        static void Main(string[] args)
        {
            //Establish connection
            var tcpListener = new TcpListener(IPAddress.Any, 1988);
            tcpListener.Start();
            Console.WriteLine("------------------------SERVER----------------------------");
            var tcpClient = tcpListener.AcceptTcpClient();
            Console.WriteLine("Client has succesfully connected");
            var stream = tcpClient.GetStream();

            //Questionnaire
            var questionsList = new Dictionary<string, string>
            {
                {"Fly", "Does it fly?"},
                {"FourLegs", "Does it have four legs?"},
                {"Swim", "Does it swim?"},
                {"Shell", "Does it have a shell?"},
                {"Thumb", "Does it have thumbs?"},
                {"Poison", "Is it poisonous?"}

            };

            //List of Animals
            var animalsList = new List<Animal>
            {
                new Animal()
                {
                    Name = "Eagle",
                    Fly = true,
                    FourLegs = false,
                    Shell = false,
                    Swim = false,
                    Thumb = false,
                    Poison = false
                },
                new Animal()
                {
                    Name = "Dog",
                    Fly = false,
                    FourLegs = true,
                    Shell = false,
                    Swim = false,
                    Thumb = false,
                    Poison = false
                },
                new Animal()
                {
                    Name = "Fish",
                    Fly = false,
                    FourLegs = false,
                    Shell = false,
                    Swim = true,
                    Thumb = false,
                    Poison = false
                },
                new Animal()
                {
                    Name = "Crab",
                    Fly = false,
                    FourLegs = false,
                    Shell = true,
                    Swim = true,
                    Thumb = false,
                    Poison = false
                },
                new Animal()
                {
                    Name = "Gorilla",
                    Fly = false,
                    FourLegs = false,
                    Shell = false,
                    Swim = false,
                    Thumb = true,
                    Poison = false
                },
                new Animal()
                {
                    Name = "Cobra",
                    Fly = false,
                    FourLegs = false,
                    Shell = false,
                    Swim = false,
                    Thumb = false,
                    Poison = true
                },
            };

            var animalListCopy = new List<Animal>();
            animalListCopy.AddRange(animalsList);

            foreach (var question in questionsList)
            {
                Send(stream, question.Value);
                var answer = Receive(stream, tcpClient)=="yes";

                foreach (var animal in animalsList)
                {
                    var property = animal.GetType().GetProperty(question.Key);
                    var attribute = (bool) property.GetValue(animal);

                    if (attribute != answer)
                    {
                        animalListCopy.Remove(animal);
                    }

                }

                if (animalListCopy.Count == 1)
                    break;
            }

            if (animalListCopy.Count == 1)
                Send(stream, "Your animal is a(n) " + animalListCopy.ElementAt(0).Name);
            else
            {
                Send(stream, "Animal not found");
            }

           }
     }
}

