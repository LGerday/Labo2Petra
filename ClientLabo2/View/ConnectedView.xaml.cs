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
        public int[] saveAction { get; set; }
        public ConnectedView(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            CaptorList.DataContext = _viewModel;
            ActuatorAction.DataContext = _viewModel;
            saveAction = new[] {0, 0, 0, 0};
        }

        private void TurnOn(object sender, RoutedEventArgs e)
        {
            var rowItem = ((Button)sender).DataContext as Actuator;

            Debug.WriteLine("On passe ICI " + rowItem.Name);
            switch (rowItem.Captor)
            {
                case 1:
                case 2:
                {
                    PopUpTime popup = new PopUpTime();
                    popup.ShowDialog();
                    if (popup.choice)
                    {
                        _viewModel.SendMessage(1,rowItem.Captor, popup.time,0);
                    }
                }
                    break;
                case 3: case 4: case 5: case 6:
                {
                    int temp = rowItem.Captor - 3;
                    if (saveAction[temp] == 0)
                    {
                        // activer
                        _viewModel.SendMessage(1,rowItem.Captor,0,0);
                        saveAction[temp] = 1;

                    }
                    else
                    {
                            //desactiver
                            _viewModel.SendMessage(3, rowItem.Captor, 0, 0);
                            saveAction[temp] = 0;
                    }

                }
                    break;
                case 7:
                {
                    PopPupPosition popup = new PopPupPosition();
                    popup.ShowDialog();
                    if (popup.Position > -1 && popup.Position < 4)
                    {
                        _viewModel.SendMessage(2,rowItem.Captor, 0,popup.Position);
                    }
                }
                    break;
                default:
                {
                        //error
                }
                    break;
            }

        }

        private void TextAnalyzer(object sender, RoutedEventArgs e)
        {
            SyntaxeNotif.Text = "";
            if (SyntaxeBox.Text.Length != 0)
            {
                _viewModel.CompileSyntaxe(SyntaxeBox.Text);
            }
            else
            {
                SyntaxeNotif.Foreground = new SolidColorBrush(Colors.Red);
                SyntaxeNotif.Text = "No text to analyze";
            }

        }
    }
}
