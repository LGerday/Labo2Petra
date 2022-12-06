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
using System.Reflection.Emit;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using ClientLabo2.Classe;
using Label = System.Windows.Controls.Label;

namespace ClientLabo2
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public int TypeRunningApp { get; set; }
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
        public MessageToServer MsgToSend { get; set; }



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
            TypeRunningApp = 0;
            MsgToSend = new MessageToServer();
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        private void PropertyChangedEventHandler(object view, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(view, e);
        }

        public void ConnectToServer(string ip,int port)
        {
            ipHost = Dns.GetHostEntry(Dns.GetHostName());
            if (TypeRunningApp == 1)
            {
                ipAddr = IPAddress.Parse("10.59.40.64");
            }
            else if (TypeRunningApp == 2)
            {
                ipAddr = IPAddress.Parse("192.168.139.134");
            }
            
            localEndPoint = new IPEndPoint(ipAddr, port);

            sender = new Socket(ipAddr.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(localEndPoint);
            CurrentView = Connected;
            threadCaptor = new Thread(ThreadFunction);
            threadCaptor.Start();
        }

        public void SendMessage(int action,int captor,int time,int position)
        {
            string msg = "1!"+action.ToString() + "-" + captor.ToString() + "-" + time.ToString() + "-" + position.ToString()+";";
            Debug.WriteLine("Client <: Send single message : " + msg);
            byte[] messageSent = Encoding.ASCII.GetBytes(msg);
            sender.Send(messageSent);
        }

        public void SendMessageGroup(string msg,int type)
        {
            string newMsg;
            if (type == 1)
            {
                newMsg = "1!" + msg;
            }
            else
            {
                newMsg = "2!" + msg;
            }
            Debug.WriteLine("Client <: Send group message : "+newMsg);
            byte[] messageSent = Encoding.ASCII.GetBytes(newMsg);
            sender.Send(messageSent);
        }

        /// <summary>
        /// msg type : x!1-5-8-0:
        /// x is a parameter for the server : 1 one message, 2 multiple message
        /// if there is a 0 in the line, the parameter is not use : ex : 4-8-0 -> no actuator, 2-0-5 -> no time
        /// First symbol : 1 activer (avec temps), 2 activer sans temps, 3 désactiver,4 attendre avec temps
        /// Second symbol : Time (if 0 no time)
        /// Third symbol : Number actuator
        /// </summary>
        public void CompileSyntaxe(string syntaxetmp)
        {
            Connected.SyntaxeNotif.Text = "";
            MsgToSend.ToZero();
            string msg = "";
            int cpt = 0;
            string[] textSplit = syntaxetmp.Split(';');

            //ajouter foreach
            foreach (var syntaxe in textSplit)
            {
                if (syntaxe != "")
                {
                    cpt++;
                    Debug.WriteLine(syntaxe);
                    FirstAnalyzer(syntaxe);
                    if (MsgToSend.Action != -1)
                    {
                        Connected.SyntaxeNotif.Foreground = new SolidColorBrush(Colors.Green);
                        Connected.SyntaxeNotif.Text += "The " + cpt + " message ok   ";
                        msg += MsgToSend.MsgForSendInGroup();
                        Debug.WriteLine("Client <: Msg sent via syntaxeCompiler to server with : " + MsgToSend.ToString());
                    }
                    else
                    {
                        MsgToSend.ToZero();
                        return;
                    }
                    MsgToSend.ToZero();
                }
            }

            if (cpt == 1)
            {
                SendMessageGroup(msg,1);
            }
            else
                SendMessageGroup(msg,2);
        }
        public void FirstAnalyzer(string synt)
        {
            string[] syntaxe = synt.Split(' ');
            switch (syntaxe[0])
            {
                case "activer":
                {
                    if (syntaxe[1] == "pendant")
                    {
                        // activer avec temps
                        MsgToSend.Action = 1;
                        TimeAnalyzer(syntaxe,2);
                    }
                    else
                    {
                        //activer sans temps
                        MsgToSend.Action = 2;
                        if (syntaxe.Length == 4)
                        {
                            CaptorAnalyzer(syntaxe[1],syntaxe[3]);
                        }
                        else
                            CaptorAnalyzer(syntaxe[1],"");
                    }
                }
                    break;
                case "desactiver":
                {
                    MsgToSend.Action = 3;
                    CaptorAnalyzer(syntaxe[1],"");
                }
                    break;
                case "attendre":
                {
                    MsgToSend.Action = 4;
                    TimeAnalyzer(syntaxe, 1);
                    
                }
                    break;
                default:
                {
                        // error
                        Debug.WriteLine("Client <: Error first word");
                        Connected.SyntaxeNotif.Foreground = new SolidColorBrush(Colors.Red);
                        Connected.SyntaxeNotif.Text = "Erreur in first word : " + syntaxe[0];
                        MsgToSend.Action = -1;
                    }
                    break;
            }

        }

        public void TimeAnalyzer(string[] synt,int type)
        {
            if (int.TryParse(synt[type], out _))
            {
                MsgToSend.Time = int.Parse(synt[type]);
                if (type == 2)
                {
                    CaptorAnalyzer(synt[4],"");
                }
            }
            else
            {
                Debug.WriteLine("Client <: Error number");
                Connected.SyntaxeNotif.Foreground = new SolidColorBrush(Colors.Red);
                Connected.SyntaxeNotif.Text = "Erreur in number : " + synt[type];
                MsgToSend.Action = -1;
            }
            
        }

        public void CaptorAnalyzer(string syntaxe,string position)
        {
            switch (syntaxe)
            {
                case "convoyeur1":
                {
                    MsgToSend.Actuator = 1;
                }
                    break;
                case "convoyeur2":
                {
                    MsgToSend.Actuator = 2;
                }
                    break;
                case "ventouse":
                {
                    MsgToSend.Actuator = 3;
                }
                    break;
                case "plongeur":
                {
                    MsgToSend.Actuator = 4;
                }
                    break;
                case "arbre":
                {
                    MsgToSend.Actuator = 5;
                }
                    break;
                case "grappin":
                {
                    MsgToSend.Actuator = 6;
                }
                    break;
                case "chariot":
                {
                    MsgToSend.Actuator = 7;
                    MsgToSend.Position = int.Parse(position);
                }
                    break;
                default:
                {
                    Debug.WriteLine("Client <: Error actuator");
                        Connected.SyntaxeNotif.Foreground = new SolidColorBrush(Colors.Red);
                    Connected.SyntaxeNotif.Text = "Erreur in actuator name : " + syntaxe;
                    MsgToSend.Action = -1;
                } break;

            }
        }

        public void ThreadFunction()
        {
            byte[] messageReceived = new byte[1024];
            while (true)
            {
                Thread.Sleep(1000);
                sender.Receive(messageReceived);
                int i = 0;
                string strlist = Encoding.ASCII.GetString(messageReceived);
                Debug.WriteLine("Client <: Reception message :"+ strlist);
                foreach (Captor c in Captor)
                {
                    c.State = strlist[i] - 48;
                    i += 1;
                }
            }
        }
    }
}
