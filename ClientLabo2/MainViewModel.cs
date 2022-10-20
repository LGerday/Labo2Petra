using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLabo2.View;
using System.Net;
using System.Net.Sockets;

namespace ClientLabo2
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ConnectedView Connected { get; set; }
        public ConnectionView ConnectionView { get; set; }
        private object _currentView;
        private string _msg;

        public string Msg
        {
            get => _msg;
            set
            {
                _msg = value;
                PropertyChangedEventHandler(this, new PropertyChangedEventArgs("MSG"));

            }
        }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                PropertyChangedEventHandler(this, new PropertyChangedEventArgs("CurrentView"));

            }
        }
        public MainViewModel()
        {
            Connected = new ConnectedView(this);
            ConnectionView = new ConnectionView(this);
            Msg = "Test";
            CurrentView = ConnectionView;
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        private void PropertyChangedEventHandler(object view, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(view, e);
        }

        public void ConnectToServer()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = IPAddress.Parse("192.168.182.134");
            //192.168.182.134
            //10.59.40.64
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 6670);

            Socket sender = new Socket(ipAddr.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(localEndPoint);

            byte[] messageSent = Encoding.ASCII.GetBytes("Test<EOF>");
            sender.Send(messageSent);

            byte[] messageReceived = new byte[1024];

            
            CurrentView = Connected;

            sender.Receive(messageReceived);

            Msg = Encoding.ASCII.GetString(messageReceived);
            Console.WriteLine(Msg);
        }
    }
}
