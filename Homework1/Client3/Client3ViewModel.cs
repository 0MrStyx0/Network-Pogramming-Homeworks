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
using System.Threading.Tasks;
using System.Windows;

namespace Client3
{
    public class Client3ViewModel: ViewModelBase
    {
        public bool IsUser { get; set; } = true;
        public bool IsComputer { get; set; } = false;

        private string buttonText = "Send";
        public string ButtonText
        {
            get { return buttonText; }
            set { Set(ref buttonText, value); }
        }
        

        private string ip;
        public string IP
        {
            get { return ip; }
            set { Set(ref ip, value); }
        }

        private ObservableCollection<string> messageList = new ObservableCollection<string>();
        public ObservableCollection<string> MessageList
        {
            get { return messageList; }
            set { Set(ref messageList, value); }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set { Set(ref text, value); }
        }

        public Socket socket { get; set; }
        public IPEndPoint iPEndPoint { get; set; }

        private RelayCommand aiMod;
        public RelayCommand AiMod
        {
            get
            {
                return aiMod ?? new RelayCommand(() =>
                {
                    if (IsComputer) { ButtonText = "Start"; }
                });
            }
        }

        private RelayCommand userMod;
        public RelayCommand UserMod
        {
            get
            {
                return userMod ?? new RelayCommand(() =>
                {
                    if (IsUser) { ButtonText = "Send"; }
                });
            }
        }

        private RelayCommand connect;
        public RelayCommand Connect
        {
            get
            {
                return connect ?? new RelayCommand(() =>
                {
                    ServerConnect();
                });
            }
        }

        private void ServerConnect()
        {
            try
            {
                iPEndPoint = IPEndPoint.Parse(IP);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(iPEndPoint);
                MessageBox.Show("Connection Success!");
            }
            catch
            {
                MessageBox.Show("Connection Failed!");
            }
        }

        private RelayCommand sendMessage;
        public RelayCommand SendMessage
        {
            get
            {
                return sendMessage ?? new RelayCommand(() =>
                {
                    if(ButtonText == "Send")
                    {
                        HumanChat();
                    }
                    else if(ButtonText == "Start")
                    {
                        ComputerChat();
                    }
                });
            }
        }

        private void HumanChat()
        {
            try
            {
                MessageList.Add($"{socket.LocalEndPoint}: {Text}");
                using (NetworkStream networkStream = new NetworkStream(socket))
                {
                    using (StreamWriter writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
                    {
                        using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                        {
                            writer.WriteLine(Text);
                            string answer = reader.ReadLine();
                            MessageList.Add(answer);
                            Text = string.Empty;
                            if (answer == "Human: Bye")
                            {
                                socket.Disconnect(false);
                                MessageBox.Show("Сервер отключил вас!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComputerChat()
        {
            string AIMessage = "Computer: ";
            string message1 = "Hello i am Comp";
            string message2 = "You are Ugly";
            string message3 = "I want to talk you";
            string message4 = "Hi";
            string message5 = "Bye";
            Random random = new Random();
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        using (NetworkStream networkStream = new NetworkStream(socket))
                        {
                            using (StreamWriter writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true })
                            {
                                using (StreamReader reader = new StreamReader(networkStream, Encoding.UTF8))
                                {
                                    switch (random.Next(1, 6))
                                    {
                                        case 1:
                                            writer.WriteLine(message1);
                                            Application.Current.Dispatcher.Invoke(() => { MessageList.Add(AIMessage + message1); });                                           
                                            break;
                                        case 2:
                                            writer.WriteLine(message2);
                                            Application.Current.Dispatcher.Invoke(() => { MessageList.Add(AIMessage + message1); });
                                            break;
                                        case 3:
                                            writer.WriteLine(message3);
                                            Application.Current.Dispatcher.Invoke(() => { MessageList.Add(AIMessage + message1); });
                                            break;
                                        case 4:
                                            writer.WriteLine(message4);
                                            Application.Current.Dispatcher.Invoke(() => { MessageList.Add(AIMessage + message1); });
                                            break;
                                        case 5:
                                            writer.WriteLine(message5);
                                            Application.Current.Dispatcher.Invoke(() => { MessageList.Add(AIMessage + message1); });
                                            break;
                                    }

                                    string answer = reader.ReadLine();
                                    Application.Current.Dispatcher.Invoke(() => { MessageList.Add(answer); });
                                    Text = string.Empty;
                                    if (answer == "Human: Bye")
                                    {
                                        socket.Disconnect(false);
                                        MessageBox.Show("Сервер отключил вас!");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            });

        }
    }
}
