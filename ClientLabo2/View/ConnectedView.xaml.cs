using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using ClientLabo2.Classe;

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
            CaptorList.DataContext = _viewModel;
            ActuatorAction.DataContext = _viewModel;
        }

        private void TurnOn(object sender, RoutedEventArgs e)
        {
            var rowItem = ((Button)sender).DataContext as Actuator;
            PopUpTime popup = new PopUpTime();
            popup.ShowDialog();
            if (popup.choice)
            {
                Debug.Write("On passe ICI "+rowItem.Name);
                _viewModel.SendMessage(rowItem.Captor,popup.time);
            }

        }
    }
}
