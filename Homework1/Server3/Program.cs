using System.Net;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;
using System.Text;

IPEndPoint iPEndPoint = IPEndPoint.Parse("127.0.0.1:8080");
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

socket.Bind(iPEndPoint);
socket.Listen();

Console.WriteLine($"Server created on {socket.LocalEndPoint}....");

Console.WriteLine("Choose Mode:\n1.Human\n2.AI");
Console.Write("--> ");
bool IsAI = false;
switch (Convert.ToInt32(Console.ReadLine()))
{
    case 1:
        IsAI = false;
        Console.WriteLine("Server on Human Mod");
        break;

    case 2:
        IsAI = true;
        Console.WriteLine("Server on AI Mod");
        break;
}

while (true)
{
    CheckRequest(socket, IsAI);
}

void CheckRequest(Socket socket,bool IsAI)
{
    var client = socket.Accept();

    Console.WriteLine($"Client {client.RemoteEndPoint} is connected.");

    while (true)
    {
        if (client.Connected)
        {
            using (NetworkStream networkStream = new NetworkStream(client))
            {
                using (StreamWriter writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
                {
                    using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                    {
                        try
                        {
                            string message = $"{client.RemoteEndPoint}: {reader.ReadLine()}";
                            Console.WriteLine(message);
                            if (!IsAI)
                            {
                                string HumanMessage1 = "Human: ";
                                Console.Write(HumanMessage1);
                                string HumanMessage2 = Console.ReadLine();
                                writer.WriteLine(HumanMessage1 + HumanMessage2);
                                if (HumanMessage2 == "Bye")
                                {
                                    Console.WriteLine(client.RemoteEndPoint + " DISCONNECTED");
                                    client.Disconnect(false);
                                }
                            }
                            else
                            {
                                AITalk(writer, reader);
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"{client.RemoteEndPoint} DISCONNECTED!");
                        }

                    }
                }
            }
        }
        else
        {
            break;
        }
    }
}

void AITalk(StreamWriter writer, StreamReader reader)
{
    string AIMessage = "Jarvis: ";
    string message1 = "Hello i am Jarvis";
    string message2 = "Send Bye if you want to leave";
    string message3 = "Am i talk with machine or human?";
    string message4 = "Nice to meet you";
    string message5 = "Bye";
    
    
    Random random = new Random();
    Thread.Sleep(1000);
    switch (random.Next(1, 6))
    {
        case 1:
            writer.WriteLine(AIMessage + message1);
            Console.WriteLine(AIMessage + message1);
            break;
        case 2:
            writer.WriteLine(AIMessage + message2);
            Console.WriteLine(AIMessage + message2);
            break;
        case 3:
            writer.WriteLine(AIMessage + message3);
            Console.WriteLine(AIMessage + message3);
            break;
        case 4:
            writer.WriteLine(AIMessage + message4);
            Console.WriteLine(AIMessage + message4);
            break;
        case 5:
            writer.WriteLine(AIMessage + message5);
            Console.WriteLine(AIMessage + message5);
            break;
    }
}