using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class ClientViewModel:ViewModelBase
    {
        string currentPlayer;
        public string CurrentPlayer
        {
            get { return currentPlayer; }
            set { Set(ref currentPlayer, value); }
        }

        string buttonSymb1;
        public string ButtonSymb1
        {
            get { return buttonSymb1; } set { Set(ref buttonSymb1, value);}
        }

        string buttonSymb2;
        public string ButtonSymb2
        {
            get { return buttonSymb2; }
            set { Set(ref buttonSymb2, value); }
        }

        string buttonSymb3;
        public string ButtonSymb3
        {
            get { return buttonSymb3; }
            set { Set(ref buttonSymb3, value); }
        }

        string buttonSymb4;
        public string ButtonSymb4
        {
            get { return buttonSymb4; }
            set { Set(ref buttonSymb4, value); }
        }

        string buttonSymb5;
        public string ButtonSymb5
        {
            get { return buttonSymb5; }
            set { Set(ref buttonSymb5, value); }
        }

        string buttonSymb6;
        public string ButtonSymb6
        {
            get { return buttonSymb6; }
            set { Set(ref buttonSymb6, value); }
        }

        string buttonSymb7;
        public string ButtonSymb7
        {
            get { return buttonSymb7; }
            set { Set(ref buttonSymb7, value); }
        }

        string buttonSymb8;
        public string ButtonSymb8
        {
            get { return buttonSymb8; }
            set { Set(ref buttonSymb8, value); }
        }

        string buttonSymb9;
        public string ButtonSymb9
        {
            get { return buttonSymb9; }
            set { Set(ref buttonSymb9, value); }
        }

        bool isButtonPressed = false;

        bool isConnectedToServer = false;

        public TcpClient Client { get; set; }

        public ClientViewModel()
        {
            checkBoard();
        }

        RelayCommand connect;
        public RelayCommand Connect
        {
            get
            {
                return connect ?? new RelayCommand(() =>
                {
                    ConnectToServer();
                });
            }
        }

        RelayCommand<string> setSymb;
        public RelayCommand<string> SetSymb
        {
            get
            {
                return setSymb ?? new RelayCommand<string>((button) =>
                {
                    SetSymbol(button);
                });
            }
        }

        void SetSymbol(string button)
        {
            if(isButtonPressed == false)
            {
                bool isDone = false;
                if (button == "1" && ButtonSymb1 == null) { ButtonSymb1 = CurrentPlayer; isDone = true; } 
                else if (button == "2" && ButtonSymb2 == null) { ButtonSymb2 = CurrentPlayer; isDone = true; }
                else if (button == "3" && ButtonSymb3 == null) { ButtonSymb3 = CurrentPlayer; isDone = true; }
                else if (button == "4" && ButtonSymb4 == null) { ButtonSymb4 = CurrentPlayer; isDone = true; }
                else if (button == "5" && ButtonSymb5 == null) { ButtonSymb5 = CurrentPlayer; isDone = true; }
                else if (button == "6" && ButtonSymb6 == null) { ButtonSymb6 = CurrentPlayer; isDone = true; }
                else if (button == "7" && ButtonSymb7 == null) { ButtonSymb7 = CurrentPlayer; isDone = true; }
                else if (button == "8" && ButtonSymb8 == null) { ButtonSymb8 = CurrentPlayer; isDone = true; }
                else if (button == "9" && ButtonSymb9 == null) { ButtonSymb9 = CurrentPlayer; isDone = true; }

                if (isDone == true)
                {
                    NetworkStream networkStream = Client.GetStream();
                    StreamWriter writer = new StreamWriter(networkStream) { AutoFlush = true };
                    writer.WriteLine(button);
                    isButtonPressed = true;
                }
                else { }
            }
            else if (isButtonPressed == true) { }
        }

        async Task ConnectToServer()
        {
            Client = new TcpClient();
            Client.ConnectAsync(IPEndPoint.Parse("127.0.0.1:8080"));
            MessageBox.Show("Подключился");
            isConnectedToServer = true;
            NetworkStream networkStream = Client.GetStream();
            StreamReader reader = new StreamReader(networkStream);
            CurrentPlayer = reader.ReadLine();
        }

        async Task checkBoard()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() =>
            {
                while(true)
                {
                    if (isButtonPressed == false && isConnectedToServer == true)
                    {
                        NetworkStream networkStream = Client.GetStream();
                        StreamReader reader = new StreamReader(networkStream);
                        string symb = reader.ReadLine();
                        string position = reader.ReadLine();
                        if (position == "1") ButtonSymb1 = symb;
                        else if (position == "2") ButtonSymb2 = symb;
                        else if (position == "3") ButtonSymb3 = symb;
                        else if (position == "4") ButtonSymb4 = symb;
                        else if (position == "5") ButtonSymb5 = symb;
                        else if (position == "6") ButtonSymb6 = symb;
                        else if (position == "7") ButtonSymb7 = symb;
                        else if (position == "8") ButtonSymb8 = symb;
                        else if (position == "9") ButtonSymb9 = symb;

                        isButtonPressed = false;
                    }
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
