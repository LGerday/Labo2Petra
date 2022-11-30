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
    /// Logique d'interaction pour AnalyzerTextView.xaml
    /// </summary>
    public partial class AnalyzerTextView : UserControl
    {
        private MainViewModel _viewModel;
        public AnalyzerTextView(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        private void TryAnalyzer(object sender, RoutedEventArgs e)
        {
            
            _viewModel.TextAnalyzer(BoxAnalyzer.Text);


        }
    }
}
