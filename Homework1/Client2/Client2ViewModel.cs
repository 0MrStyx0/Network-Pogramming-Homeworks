using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace Client2
{
    public class Client2ViewModel:ViewModelBase
    {
        public Socket Socket { get; set; }
        public IPEndPoint endPoint { get; set; }


        private ObservableCollection<string> messageList;
        public ObservableCollection<string> MessageList
        {
            get { return messageList; }
            set { Set(ref messageList, value); }
        }

        public Client2ViewModel()
        {
            MessageList = new ObservableCollection<string>();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = IPEndPoint.Parse("127.0.0.1:8080");
            Connect(Socket);
        }

        private RelayCommand<string> showTime;
        public RelayCommand<string> ShowTime
        {
            get
            {
                return showTime ?? new RelayCommand<string>((param) =>
                {
                    SendRequest(param);
                });
            }
        }

        private RelayCommand<string> showDate;
        public RelayCommand<string> ShowDate
        {
            get
            {
                return showDate ?? new RelayCommand<string>((param) =>
                {
                    SendRequest(param);
                });
            }
        }

        private async void Connect(Socket socket)
        {
            await socket.ConnectAsync(endPoint);
        }

        private async void SendRequest(string param)
        {
            string message = null;
            if (param == "Time")
            {
                message = "Time";
            }
            else if (param == "Date")
            {
                message = "Date";
            }
            
            message = $"{message}";
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await Socket.SendAsync(messageBytes);

            var buffer = new byte[1024];
            var bytes = await Socket.ReceiveAsync(buffer);
            string answer = Encoding.UTF8.GetString(buffer, 0, Convert.ToInt32(bytes));
            MessageList.Add(answer);
        }
    }
}
