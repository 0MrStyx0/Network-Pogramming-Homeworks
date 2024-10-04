using System.Net;
using System.Net.Sockets;
using System.Text;

Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPAddress iP = IPAddress.Parse("127.0.0.1");
IPEndPoint endPoint = new IPEndPoint(iP, 8080);

socket.Bind(endPoint);

Console.WriteLine("Server started....");

while (true)
{
    CheckingRequests(socket);
}

static  void CheckingRequests(Socket socket)
{
    socket.Listen();

    Task.Run(async() =>
    {
        var client = await socket.AcceptAsync();
        Console.WriteLine(client.RemoteEndPoint + " is connected.");
        while (true)
        {

            var buffer = new byte[1024];
            var bytes = await client.ReceiveAsync(buffer);
            string message = Encoding.UTF8.GetString(buffer, 0, Convert.ToInt32(bytes));

            string answer = $"{message}\n{DateTime.Now.ToShortTimeString()} [{socket.LocalEndPoint}]: Hello Client!";
            Console.WriteLine(answer);

            var answerBytes = Encoding.UTF8.GetBytes(answer);
            await client.SendAsync(answerBytes);
        }
    });

}