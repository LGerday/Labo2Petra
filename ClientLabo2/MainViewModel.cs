using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientLabo2.View;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Markup;
using ClientLabo2.Classe;

namespace ClientLabo2
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ConnectedView Connected { get; set; }
        public ConnectionView ConnectionView { get; set; }
        private object _currentView;
        public IPHostEntry ipHost { get; set; }
        public IPAddress ipAddr { get; set; }
        public IPEndPoint localEndPoint { get; set; }
        public Socket sender { get; set; }
        public Thread threadCaptor { get; set; }
        public ObservableCollection<Captor> Captor { get; set; }
        public ObservableCollection<Actuator> Actuator { get; set; }



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
            CurrentView = ConnectionView;
            Actuator = new ObservableCollection<Actuator>();
            Actuator.Add(new Actuator("Convoyeur 1",1));
            Actuator.Add(new Actuator("Convoyeur 2",2));
            Actuator.Add(new Actuator("Ventouse",3));
            Actuator.Add(new Actuator("Plongeur",4)); 
            Actuator.Add(new Actuator("Arbre",5));
            Actuator.Add(new Actuator("Grappin",6));
            Actuator.Add(new Actuator("Chariot",7));

            Captor = new ObservableCollection<Captor>();
            Captor.Add(new Captor("DE",0));
            Captor.Add(new Captor("CS", 0));
            Captor.Add(new Captor("PP", 0));
            Captor.Add(new Captor("S", 0));
            Captor.Add(new Captor("L1", 0));
            Captor.Add(new Captor("L2", 0)); 
            Captor.Add(new Captor("AP", 0));
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        private void PropertyChangedEventHandler(object view, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(view, e);
        }

        public void ConnectToServer(string ip,int port)
        {
            ipHost = Dns.GetHostEntry(Dns.GetHostName());
            ipAddr = IPAddress.Parse(ip);
            //192.168.182.134
            //10.59.40.64
            localEndPoint = new IPEndPoint(ipAddr, port);

            sender = new Socket(ipAddr.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(localEndPoint);
            CurrentView = Connected;
            threadCaptor = new Thread(ThreadFunction);
            threadCaptor.Start();
        }

        public void SendMessage(int captor,int time)
        {
            Debug.WriteLine("Test on passe dans le send\n");
            if (captor != 0)
            {
                string msg = captor.ToString() + "-" + time.ToString();
                byte[] messageSent = Encoding.ASCII.GetBytes(msg);
                sender.Send(messageSent);
            }
        }

        public int FindNbCaptor(string captor)
        {
            switch (captor)
            {
                case "Convoyeur 1":
                    return 1;
                case "Convoyeur 2":
                    return 2;
                case "Ventouse":
                    return 3;
                case "Plongeur":
                    return 4;
                case "Arbre":
                    return 5;
                case "Grappin":
                    return 6;
                case "Chariot":
                    return 7;
                default: return 0;
            }
        }
public void ThreadFunction()
{
    byte[] messageReceived = new byte[1024];
    while (true)
    {
        sender.Receive(messageReceived);
        int i = 0;
        String[] strlist = Encoding.ASCII.GetString(messageReceived).Split('-');
        foreach (String s in strlist)
        {
            Captor[i].State = int.Parse(s);
            i =+ 1;
        }
    }
}
    }
}
