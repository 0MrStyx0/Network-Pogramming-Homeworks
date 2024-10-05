using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class ClientViewModel:ViewModelBase
    {
        ObservableCollection<string> messageList = new ObservableCollection<string>();
        public ObservableCollection<string> MessageList
        {
            get { return messageList; } set { Set(ref messageList, value); }
        }

        public TcpClient Client {  get; set; }

        public bool isPlayerAI {  get; set; }
        public bool isEnemyAI {  get; set; }

        public bool isWin {  get; set; }

        RelayCommand<string> humanMode;
        public RelayCommand<string> HumanMode
        {
            get
            {
                return humanMode ?? new RelayCommand<string>((mode) =>
                {
                    ConnectToServerAsync(mode);
                });
            }
        }

        RelayCommand<string> aIMode;
        public RelayCommand<string> AIMode
        {
            get
            {
                return aIMode ?? new RelayCommand<string>((mode) =>
                {
                    ConnectToServerAsync(mode);
                });
            }
        }

        RelayCommand enemyAI;
        public RelayCommand EnemyAI
        {
            get
            {
                return enemyAI ?? new RelayCommand(() =>
                {
                    SetEnemyAI();
                });
            }
        }

        RelayCommand enemyHuman;
        public RelayCommand EnemyHuman
        {
            get
            {
                return enemyHuman ?? new RelayCommand(() =>
                {
                    SetEnemyHuman();
                });
            }
        }

        RelayCommand<string> send;
        public RelayCommand<string> Send
        {
            get
            {
                return send ?? new RelayCommand<string>((value) =>
                {
                    RockPaperScissors(value);
                });
            }
        }

        void RockPaperScissors(string value)
        {
            MessageList.Add($"Вы: {value}");
            NetworkStream networkStream = Client.GetStream();
            StreamWriter writer = new StreamWriter(networkStream) { AutoFlush = true };
            StreamReader reader = new StreamReader(networkStream);
            writer.WriteLine(value);

            string result = reader.ReadLine();
            MessageList.Add($"Противник: {result}");
            isWin = Convert.ToBoolean(reader.ReadLine());

            if (value == result) { MessageList.Add("Ничья!"); MessageList.Add(""); }
            else if (isWin) { MessageList.Add("Вы победили"); MessageList.Add(""); }
            else { MessageList.Add("Противник победил"); MessageList.Add(""); }
        }

        async void SetEnemyAI()
        {
            isEnemyAI = true;
            NetworkStream networkStream = Client.GetStream();
            StreamWriter writer = new StreamWriter(networkStream) { AutoFlush = true };
            StreamReader reader = new StreamReader(networkStream);
            writer.WriteLine(isEnemyAI);
            if (isPlayerAI)
            {
                while (isPlayerAI)
                {
                    await Task.Delay(1000);
                    List<string> list = new List<string>()
                    {
                        "Rock","Paper","Scissors"
                    };
                    Random random = new Random();
                    string value = list[random.Next(list.Count - 1)];
                    MessageList.Add($"Вы: {value}");
                    writer.WriteLine(value);

                    string result = reader.ReadLine();
                    MessageList.Add($"Противник: {result}");
                    isWin = Convert.ToBoolean(reader.ReadLine());

                    if (value == result) { MessageList.Add("Ничья!"); MessageList.Add(""); }
                    else if (isWin) { MessageList.Add("Вы победили"); MessageList.Add(""); }
                    else { MessageList.Add("Противник победил"); MessageList.Add(""); }
                }
            }
        }

        void SetEnemyHuman()
        {
            isEnemyAI = false;
            NetworkStream networkStream = Client.GetStream();
            StreamWriter writer = new StreamWriter(networkStream) { AutoFlush = true };
            StreamReader reader = new StreamReader(networkStream);
            writer.WriteLine(isEnemyAI);
        }
        async Task ConnectToServerAsync(string mode)
        {
            if (mode == "AI") { isPlayerAI = true; MessageBox.Show("Вы вошли как комп"); } 
            else if(mode == "Human") { isPlayerAI = false; MessageBox.Show("Вы вошли как человек"); }
            Client = new TcpClient();
            await Client.ConnectAsync(IPEndPoint.Parse("127.0.0.1:8080"));
            NetworkStream networkStream = Client.GetStream();
            StreamWriter writer = new StreamWriter(networkStream) { AutoFlush = true };
            writer.WriteLine(isPlayerAI);
        }
    }
}
