using System.Net.Sockets;
using System.Net;
using System.Text.Json;

TcpListener listener = new TcpListener(IPEndPoint.Parse("127.0.0.1:8080"));
listener.Start();
Console.WriteLine($"{DateTime.Now.ToString()}: Server Started on {listener.LocalEndpoint}");

List<string> list = new List<string>()
{
    "Rock","Paper","Scissors"
};

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    NetworkStream networkStream = client.GetStream();
    StreamReader reader = new StreamReader(networkStream);
    StreamWriter writer = new StreamWriter(networkStream) { AutoFlush = true };

    bool isPlayerAI = Convert.ToBoolean(reader.ReadLine());
    bool isEnemyAI = Convert.ToBoolean(reader.ReadLine());
    if (!isPlayerAI && isEnemyAI) HumanVersusAIorAIVersusAI(client,writer,reader,list);
    else if (isPlayerAI && isEnemyAI) HumanVersusAIorAIVersusAI(client, writer, reader, list);
    else if(!isPlayerAI && !isEnemyAI)
    {
        var clientOther = await listener.AcceptTcpClientAsync();
        NetworkStream networkStreamOther = clientOther.GetStream();
        StreamReader readerOther = new StreamReader(networkStreamOther);
        StreamWriter writerOther = new StreamWriter(networkStreamOther) { AutoFlush = true };
        bool isPlayerAIOther = Convert.ToBoolean(readerOther.ReadLine());
        bool isEnemyAIOther = Convert.ToBoolean(readerOther.ReadLine());
        HumanVersusHuman(client, clientOther, writer, reader, writerOther, readerOther);
    }
}

async Task HumanVersusHuman(TcpClient client1, TcpClient client2, StreamWriter writer1, StreamReader reader1, StreamWriter writer2, StreamReader reader2)
{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    Task.Run(() =>
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {client1.Client.RemoteEndPoint} connected to server");
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {client2.Client.RemoteEndPoint} connected to server");
        while (true)
        {
            string human1 = reader1.ReadLine();
            string human2 = reader2.ReadLine();

            bool isWinhuman1 = false;
            bool isWinhuman2 = false;
            writer1.WriteLine(human2);
            writer2.WriteLine(human1);
            if (human1 == "Rock" && human2 == "Paper")
            {
                isWinhuman1 = false;
                isWinhuman2 = true;
                writer1.WriteLine(isWinhuman1);
                writer2.WriteLine(isWinhuman2);
            }
            else if (human1 == "Rock" && human2 == "Scissors")
            {
                isWinhuman1 = true;
                isWinhuman2 = false;
                writer1.WriteLine(isWinhuman1);
                writer2.WriteLine(isWinhuman2);
            }
            else if (human1 == "Paper" && human2 == "Scissors")
            {
                isWinhuman1 = false;
                isWinhuman2 = true;
                writer1.WriteLine(isWinhuman1);
                writer2.WriteLine(isWinhuman2);
            }
            else if (human1 == "Paper" && human2 == "Rock")
            {
                isWinhuman1 = true;
                isWinhuman2 = false;
                writer1.WriteLine(isWinhuman1);
                writer2.WriteLine(isWinhuman2);
            }
            else if (human1 == "Scissors" && human2 == "Rock")
            {
                isWinhuman1 = false;
                isWinhuman2 = true;
                writer1.WriteLine(isWinhuman1);
                writer2.WriteLine(isWinhuman2);
            }
            else if (human1 == "Scissors" && human2 == "Paper")
            {
                isWinhuman1 = true;
                isWinhuman2 = false;
                writer1.WriteLine(isWinhuman1);
                writer2.WriteLine(isWinhuman2);
            }
            else if (human1 == human2)
            {
                isWinhuman1 = false;
                isWinhuman2 = false;
                writer1.WriteLine(isWinhuman1);
                writer2.WriteLine(isWinhuman2);
            }
        }
    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
}

async Task HumanVersusAIorAIVersusAI(TcpClient client, StreamWriter writer, StreamReader reader, List<string> list)
{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    Task.Run(() =>
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {client.Client.RemoteEndPoint} connected to server");
        while (true)
        {
            string human = reader.ReadLine();

            Random random = new Random();
            string AI = list[random.Next(list.Count - 1)];
            bool isWin = false;
            writer.WriteLine(AI);
            if (AI == "Rock" && human == "Paper")
            {
                isWin = true;
                writer.WriteLine(isWin);
            }
            else if (AI == "Rock" && human == "Scissors")
            {
                isWin = false;
                writer.WriteLine(isWin);
            }
            else if (AI == "Paper" && human == "Scissors")
            {
                isWin = false;
                writer.WriteLine(isWin);
            }
            else if (AI == "Paper" && human == "Rock")
            {
                isWin = false;
                writer.WriteLine(isWin);
            }
            else if (AI == "Scissors" && human == "Rock")
            {
                isWin = true;
                writer.WriteLine(isWin);
            }
            else if (AI == "Scissors" && human == "Paper")
            {
                isWin = false;
                writer.WriteLine(isWin);
            }
            else if (AI == human) 
            {
                isWin = false;
                writer.WriteLine(isWin);
            }
        }
    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
}