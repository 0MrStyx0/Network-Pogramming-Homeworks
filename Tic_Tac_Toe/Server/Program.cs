using System.Net;
using System.Net.Sockets;

TcpListener listener = new TcpListener(IPEndPoint.Parse("127.0.0.1:8080"));
listener.Start();
Console.WriteLine($"{DateTime.Now.ToString()}: Server Started on {listener.LocalEndpoint}");

while (true)
{
    var client1 = await listener.AcceptTcpClientAsync();
    NetworkStream networkStream1 = client1.GetStream();
    StreamWriter writer1 = new StreamWriter(networkStream1) { AutoFlush = true };
    string symb1 = "X";
    writer1.WriteLine(symb1);

    var client2 = await listener.AcceptTcpClientAsync();
    NetworkStream networkStream2 = client2.GetStream();
    StreamWriter writer2 = new StreamWriter(networkStream2) { AutoFlush = true };
    string symb2 = "O";
    writer2.WriteLine(symb2);
    Handle(client1, writer1, symb1,client2, writer2, symb2);
}

async Task Handle(TcpClient client1, StreamWriter writer1, string symbX, TcpClient client2, StreamWriter writer2, string symbO)
{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    Task.Run(async() =>
    {
        string[][] board = new string[3][];
        board[0] = new string[3];
        board[1] = new string[3];
        board[2] = new string[3];
        bool isXWin = false;
        bool isOWin = false;
        while (true)
        {
            StreamReader reader1 = new StreamReader(client1.GetStream());
            string positionX = await reader1.ReadLineAsync();
            if(positionX == "1") board[0][0] = symbX;
            else if(positionX=="2") board[0][1] = symbX;
            else if(positionX=="3") board[0][2] = symbX;
            else if(positionX=="4") board[1][0] = symbX;
            else if(positionX=="5") board[1][1] = symbX;
            else if(positionX=="6") board[1][2] = symbX;
            else if(positionX=="7") board[2][0] = symbX;
            else if(positionX=="8") board[2][1] = symbX;
            else if(positionX=="9") board[2][2] = symbX;

            writer2.WriteLine(symbX);
            writer2.WriteLine(positionX);

            for (int i = 0; i < board.Length; i++) 
            {
                for (int j = 0; j < board[i].Length; j++) 
                {
                    if (j == 2) Console.WriteLine("|  " + board[i][j]);
                    else Console.Write("|  " + board[i][j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");


            StreamReader reader2 = new StreamReader(client2.GetStream());
            string positionO = await reader2.ReadLineAsync();
            if (positionO == "1") board[0][0] = symbO;
            else if (positionO == "2") board[0][1] = symbO;
            else if (positionO == "3") board[0][2] = symbO;
            else if (positionO == "4") board[1][0] = symbO;
            else if (positionO == "5") board[1][1] = symbO;
            else if (positionO == "6") board[1][2] = symbO;
            else if (positionO == "7") board[2][0] = symbO;
            else if (positionO == "8") board[2][1] = symbO;
            else if (positionO == "9") board[2][2] = symbO;

            writer1.WriteLine(symbO);
            writer1.WriteLine(positionO);

            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (j == 2) Console.WriteLine("|  " + board[i][j]);
                    else Console.Write("|  " + board[i][j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n");

            //CheckWin(board, isXWin, isOWin, writer1, writer2);
        }
    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
}

void CheckWin(string[][] board, bool isXWin, bool isOWin, StreamWriter writer1, StreamWriter writer2)
{
    for(int i = 0;i < board.Length; i++)
    {
        if (board[i][0] != null && board[i][0] == board[i][1] && board[i][1] == board[i][2])
        {
            if(board[i][0] == "X") isXWin = true;
            else if (board[i][0] == "O") isOWin = true;
        }
    }

    for(int i = 0; i < board.Length; i++)
    {
        if (board[0][i]!=null &&board[0][i] == board[1][i] && board[1][i] == board[2][i])
        {
            if (board[0][i] == "X") isXWin = true;
            else if (board[0][i] == "O") isOWin = true;
        }
    }

    if (board[0][0]!=null &&board[0][0] == board[1][1] && board[1][1] == board[2][2]) 
    {
        if (board[0][0] == "X") isXWin = true;
        else if (board[0][0] == "O") isOWin = true;
    }
    if (board[0][2] != null && board[0][2] == board[1][1] && board[1][1] == board[2][0])
    {
        if (board[0][2] == "X") isXWin = true;
        else if (board[0][2] == "O") isOWin = true;
    }

    if(isXWin == true || isOWin == true)
    {
        writer1.WriteLine(isXWin);
        writer2.WriteLine(isOWin);
    }
    else
    {
        writer1.WriteLine(" ");
        writer2.WriteLine(" ");
    }

}