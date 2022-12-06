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
using ClientLabo2.Classe;

namespace ClientLabo2.View
{
    /// <summary>
    /// Logique d'interaction pour TestCaptor.xaml
    /// </summary>
    public partial class TestCaptor : Window
    {
        public TestCaptor(Captor captor)
        {
            InitializeComponent();
            DataContext = captor;
        }

        private void Leave(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
