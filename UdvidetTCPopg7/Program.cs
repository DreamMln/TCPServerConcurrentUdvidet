using DiceLib29aug;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UdvidetTCPopg7
{
    class Program
    {
        //static list med dice værdier
        private static List<DiceResult> _dices = new List<DiceResult>()
            {
                new DiceResult(){ PlayerName = "Lone", DiceValue = 1},
                new DiceResult(){ PlayerName = "Lars", DiceValue = 2},
                new DiceResult(){ PlayerName = "Aben", DiceValue = 6},
            };
        static void Main(string[] args)
        {
            Console.WriteLine("TCP Udvidet!");

            //Concurrent ser der modtager flere klienter, modtager requests fra klienten 
            //og gemmer de værdier i en static list, den liste skal afl, til klient

            //udvides i opg 7, del funktionen op i to

            //TCP er en af main protokollerne i TCP/IP netværk. dealer med pakker
            //TCP etablere to hosts til en connection der udviksler streams af data.
            //en port er et unikt nummer der assignes til identificere en connection/endpoint
            //Local.IPAddress - Any, alle lokale ip adr, med port 7
            TcpListener Listener = new TcpListener(IPAddress.Any, 7);

            //start ''lyt'' efter indkommende messages
            Listener.Start();

            //lyt til flere klienter - while loop
            while (true)
            {
                //AcceptTcpClient - dette returnere et TcpClient object
                //der bliver ikke eksekveret mere kode, før der kommer en ny klient
                TcpClient socket = Listener.AcceptTcpClient();
                //modtage flere klienter, kaldes handleClient(), bliver concurrent
                //håndtere flere klienter - håndtere flere tråde, Task, kører asynkront
                Task.Run(() => HandleClient(socket));
            }
        }
        public static void HandleClient(TcpClient socket)
        {
            //sernder en strøm af data
            NetworkStream ns = socket.GetStream();
            //derefter splittes de op i to streams
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            //der skal aflæses/læses det client anmoder om
            string message = reader.ReadLine();

            //hvordan modtager jeg getall og add - i to funktioner
            if (message == "GetAll")
            {
                //lav objekter i listen til JSON format
                string jsonSerializeValues = JsonConvert.SerializeObject(_dices);
                //der skal sendes til client i JSON format
                writer.WriteLine(jsonSerializeValues);
            }
            //Protokol - Add;Morten;2
            else if (message.StartsWith("Add"))
            {
                string[] splitJSONMessage = message.Split(";");
                //array 0 er Add, array 1 er navn, array 2 er dicevalue
                AddDice(splitJSONMessage[1], int.Parse(splitJSONMessage[2]));
                writer.WriteLine("Dice added!");
            }
            else
            {
                writer.WriteLine("Der er fejl, protokollen er ikke blevet fulgt korrekt!");
            }
            //makes sure that the server sends the data immediately (it should wait for potentially more data)
            writer.Flush();
            //closes the connection, single use server.
            socket.Close();
        }
 
        public static void AddDice(string addPlayer, int addDiceValue)
        {
            DiceResult diceResult = new DiceResult(addPlayer, addDiceValue);
            _dices.Add(diceResult);
        }
    }
}
