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
    /// Logique d'interaction pour ConnectedView.xaml
    /// </summary>
    public partial class ConnectedView : UserControl
    {
        public MainViewModel _viewModel;
        public ConnectedView(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            CaptorList.DataContext = _viewModel.Captor;
            ActuatorAction.DataContext = _viewModel.Actuator;
        }

        private void TurnOn(object sender, RoutedEventArgs e)
        {
            var rowItem = ((Button)sender).DataContext as String;
            PopUpTime popup = new PopUpTime();
            popup.ShowDialog();
            if (popup.choice)
            {
                _viewModel.SendMessage(rowItem,popup.time);
            }

        }
    }
}
