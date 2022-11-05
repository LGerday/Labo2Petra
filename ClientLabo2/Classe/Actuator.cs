using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientLabo2.Classe
{
    public class Actuator : INotifyPropertyChanged
    {
        private string _name;
        private int captor;

        public int Captor
        {
            get => captor;
            set => captor = value;
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public Actuator()
        {
            Name = "";
            Captor = 0;
        }

        public Actuator(string name,int captor)
        {
            Name = name;
            Captor = captor;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
