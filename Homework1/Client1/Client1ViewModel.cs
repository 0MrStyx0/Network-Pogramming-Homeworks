using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client1
{
    public class Client1ViewModel: ViewModelBase
    {
        public Socket Socket { get; set; }
        public IPEndPoint endPoint { get; set; }


        private ObservableCollection<string> messageList;
        public ObservableCollection<string> MessageList
        {
            get { return messageList; }
            set { Set(ref messageList, value); }
        }

        public Client1ViewModel()
        {
            MessageList = new ObservableCollection<string>();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = IPEndPoint.Parse("127.0.0.1:8080");
            Connect(Socket);
        }

        private RelayCommand confirm;
        public RelayCommand Confirm
        {
            get
            {
                return confirm ?? new RelayCommand(() =>
                {
                    CheckConnection();
                });
            }
        }

        private async void Connect(Socket socket)
        {
            await socket.ConnectAsync(endPoint);
        }

        private async void CheckConnection()
        {
            string message = "Hello Server!";
            message = $"{DateTime.Now.ToShortTimeString()} [{Socket.LocalEndPoint}]: {message}";
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await Socket.SendAsync(messageBytes);

            var buffer = new byte[1024];
            var bytes = await Socket.ReceiveAsync(buffer);
            string answer = Encoding.UTF8.GetString(buffer, 0, Convert.ToInt32(bytes));
            MessageList.Add(answer);
        }
    }
}
