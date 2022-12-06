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
    /// Logique d'interaction pour PopPupPosition.xaml
    /// </summary>
    public partial class PopPupPosition : Window
    {
        public int Position { get; set; }
        public PopPupPosition()
        {
            InitializeComponent();
            Position = -1;
        }

        private void DoChange(object sender, RoutedEventArgs e)
        {
            Position = int.Parse(TextBoxPosition.Text);
            this.Close();
        }

        private void StopChange(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
