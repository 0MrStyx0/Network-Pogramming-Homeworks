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

namespace Client
{
    public class ClientViewModel : ViewModelBase
    {
        private string ip;
        public string IP
        {
            get { return ip; } set { Set(ref ip, value); }
        }
        private ObservableCollection<string> currency1;
        public ObservableCollection<string> Currency1
        {
            get { return currency1; } set { Set(ref currency1, value); }
        }

        private ObservableCollection<string> currency2;
        public ObservableCollection<string> Currency2
        {
            get { return currency2; }
            set { Set(ref currency2, value); }
        }
        public string SelectedItem1 { get; set; }
        public string SelectedItem2 { get; set; }

        private string result;
        public string Result
        {
            get { return result; } set { Set(ref result, value); }
        }

        private bool isLimiteReached = false;
        private const int waitTimeSeconds = 15;
        public ClientViewModel()
        {
            Currency1 = new ObservableCollection<string>() { "USD","RUB","AZN" };
            Currency2 = new ObservableCollection<string>() { "USD","RUB","AZN" };
            Client = new TcpClient();
        }

        public TcpClient Client {  get; set; }

        private RelayCommand connect;
        public RelayCommand Connect
        {
            get
            {
                return connect ?? new RelayCommand(() =>
                {
                    ConnectToServerAsync();
                });
            }
        }


        private RelayCommand disconnect;
        public RelayCommand Disconnect
        {
            get
            {
                return disconnect ?? new RelayCommand(() =>
                {
                    DisconnectFromServerAsync();
                });
            }
        }

        private RelayCommand show;
        public RelayCommand Show
        {
            get
            {
                return show ?? new RelayCommand(() =>
                {
                    CalculateRate();
                });
            }
        }

        private void CalculateRate()
        {
            if (Client.Connected)
            {
                if (SelectedItem1 == null || SelectedItem2 == null)
                {
                    MessageBox.Show("Значения не должны быть пустыми!");
                }
                else if (SelectedItem1 == SelectedItem2) MessageBox.Show("Конвертация Невозможна");
                else
                {
                    try
                    {
                        NetworkStream networkstream = Client.GetStream();
                        StreamWriter writer = new StreamWriter(networkstream) { AutoFlush = true };
                        StreamReader reader = new StreamReader(networkstream);
                        writer.WriteLine(SelectedItem1);
                        writer.WriteLine(SelectedItem2);
                        Result = reader.ReadLine();
                    }
                    catch
                    {
                        MessageBox.Show("Вы превысили лимит запросов и Сервер отключил вас. Повторная попытка через 15 секунд...");
                        isLimiteReached = true;
                        Client.Close();
                    }

                }
            }
            else MessageBox.Show("Клиент Не подключен!!!");
        }
        private async Task ConnectToServerAsync()
        {
            try
            {
                if (Client.Connected) throw new Exception("Клиент уже подключен");

                if(isLimiteReached)
                {
                    MessageBox.Show("Вы превысили лимит запросов. Повторная попытка через 15 секунд...");
                    await Task.Delay(waitTimeSeconds * 1000);
                    isLimiteReached = false;
                    return;
                }

                await Task.Delay(3000);
                Client = new TcpClient();
                await Client.ConnectAsync(IPEndPoint.Parse(IP));
                NetworkStream networkstream = Client.GetStream();
                StreamReader reader = new StreamReader(networkstream);
                if(reader.ReadLine()== "Сервер Переполнен!!!")
                {
                    throw new Exception("Сервер Переполнен!!!");
                }

                MessageBox.Show("Клиент подключился к серверу");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if(ex.Message == "Сервер Переполнен!!!") Client.Close();
            }
        }
        private async Task DisconnectFromServerAsync()
        {
            try
            {
                if (Client.Connected)
                {
                    Client.Close();
                    MessageBox.Show("Клиент отключился от сервера");
                }
                else
                {
                    MessageBox.Show("Клиент ни присоединен ни к одному серверу");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
