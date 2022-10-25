using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientLabo2.View
{
    /// <summary>
    /// Logique d'interaction pour PopUpTime.xaml
    /// </summary>
    public partial class PopUpTime : Window
    {
        public int time;
        public bool choice;
        public PopUpTime()
        {
            InitializeComponent();
            time = 0;
            choice = false;
        }

        private void DoChange(object sender, RoutedEventArgs e)
        {
            time = int.Parse(TextBoxTime.Text);
            choice = true;
            this.Close();
        }

        private void StopChange(object sender, RoutedEventArgs e)
        {
            choice = false;
            this.Close();
        }
    }
}
