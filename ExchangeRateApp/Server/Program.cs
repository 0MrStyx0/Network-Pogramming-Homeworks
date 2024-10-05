using System.IO;
using System.Net;
using System.Net.Sockets;

TcpListener listener = new TcpListener(IPEndPoint.Parse("127.0.0.1:8080"));
const int MaxClients = 2;
const int MaxRequestsPerClient = 1;
listener.Start(MaxClients);
Console.WriteLine($"{DateTime.Now.ToString()}: Server Started on {listener.LocalEndpoint}");

List<Rate> Rates = new List<Rate>()
{
   new Rate {From = "USD" , To = "AZN" , Value = 1.7m},
   new Rate {From = "USD" , To = "RUB" , Value = 95.1198m},
   new Rate {From = "RUB" , To = "USD" , Value = 0.0105m},
   new Rate {From = "RUB" , To = "AZN" , Value = 0.01785m},
   new Rate {From = "AZN" , To = "USD" , Value = 0.58823m},
   new Rate {From = "AZN" , To = "RUB" , Value = 55.9528m},
};

List<TcpClient> Clients = new List<TcpClient>();


while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    NetworkStream networkstream = client.GetStream();
    StreamReader reader = new StreamReader(networkstream);
    StreamWriter writer = new StreamWriter(networkstream) { AutoFlush = true };
    if (Clients.Count >= MaxClients)
    {
        writer.WriteLine("Сервер Переполнен!!!");
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {client.Client.RemoteEndPoint} отключился из-за переполнения сервера");
        client.Close();
    }
    else
    {
        writer.WriteLine();
        var clientInfo = new ClientInfo { Client = client, RequestCount = 0 };
        Clients.Add(client);
        RequestProcessingAsync(clientInfo, reader, writer);
    }
}

 async Task RequestProcessingAsync(ClientInfo clientInfo, StreamReader reader, StreamWriter writer)
{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    Task.Run(async() =>
    {
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} {clientInfo.Client.Client.RemoteEndPoint} connected to server");

        while (true)
        {
            try
            {
                string currency1 = await reader.ReadLineAsync();
                string currency2 = await reader.ReadLineAsync();
                if (currency1 == null || currency2 == null) throw new Exception($"{DateTime.Now.ToLongTimeString()} {clientInfo.Client.Client.RemoteEndPoint} Disconnect!");

                Console.WriteLine($"Rates to exchange: from {currency1} to {currency2}");

                var item = Rates.Find(x => { if (x.From == currency1 && x.To == currency2) return true; return false; });

                if (item != null)
                {
                    writer.WriteLine(item.Value.ToString());
                }

                clientInfo.RequestCount++;
                if (clientInfo.RequestCount >= MaxRequestsPerClient) 
                {
                    writer.WriteLine($"Вы превысили лимит в {MaxRequestsPerClient} запросов. Соединение будет закрыто.");
                    throw new Exception($"Вы превысили лимит в {MaxRequestsPerClient} запросов. Соединение будет закрыто.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                clientInfo.Client.Close();
                reader.Close();
                writer.Close();
                Clients.Remove(clientInfo.Client);
                break;
            }
        }
    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
}


public class Rate
{
    public string From { get; set; }
    public string To { get; set; }
    public decimal Value { get; set; }
}

public class ClientInfo
{
    public TcpClient Client { get; set; }
    public int RequestCount { get; set; }
}