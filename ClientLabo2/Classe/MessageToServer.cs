using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLabo2.Classe
{
    public class MessageToServer
    {
        public int Action { get; set; }
        public int Time { get; set; }
        public int Actuator { get; set; }
        public int Position { get; set; }

        public MessageToServer()
        {
            Actuator = 0;
            Position = 0;
            Time = 0;
            Action = 0;
        }

        public void ToZero()
        {
            Actuator = 0;
            Position = 0;
            Time = 0;
            Action = 0;
        }

        public override string ToString()
        {
            return "Action :" + Action + "Actuator :" + Actuator + "Time :" + Time + "Position : " + Position;
        }

        public string MsgForSendInGroup()
        {
            return Action.ToString() + "-" + Actuator.ToString() + "-" + Time.ToString() + "-" + Position.ToString() + ";";
        }
    }
}
