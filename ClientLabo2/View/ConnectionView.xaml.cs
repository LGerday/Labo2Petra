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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientLabo2.View
{
    /// <summary>
    /// Logique d'interaction pour ConnectionView.xaml
    /// </summary>
    public partial class ConnectionView : UserControl
    {
        private MainViewModel _viewModel;
        public ConnectionView(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
        private void Connect(object sender, RoutedEventArgs e)
        {
            string ip = IpBox.Text;
            string port = PortBox.Text;
            int prt = Int32.Parse(port);
            _viewModel.ConnectToServer(ip,prt);
        }

        private void Localhost_Button(object sender, RoutedEventArgs e)
        {
            _viewModel.TypeRunningApp = 2;
        }

        private void PETRA_Button(object sender, RoutedEventArgs e)
        {
            _viewModel.TypeRunningApp = 1;
        }
    }
}
