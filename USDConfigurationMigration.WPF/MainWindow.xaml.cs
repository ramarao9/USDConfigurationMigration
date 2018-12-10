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
using USDConfigurationMigration.WPF.Models;
using USDConfigurationMigration.WPF.ViewModels;

namespace USDConfigurationMigration.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainViewModel _mainViewModel;
        public MainWindow()
        {
            InitializeComponent();

            _mainViewModel = new MainViewModel();
            this.DataContext = _mainViewModel;
        }

        private void OnSourceConnection(object sender, RoutedEventArgs e)
        {
            OpenConnectionWindow(null);
        }


        private void OnTargetConnection(object sender, RoutedEventArgs e)
        {
            var mainVM = this.DataContext as MainViewModel;

            OpenConnectionWindow(mainVM.SelectedSourceConnection);
        }


        public void OpenConnectionWindow(CrmConnectionInfo selectedSourceConnection)
        {


            CrmLoginWindow loginWindow = new CrmLoginWindow();
            loginWindow.InitConnectionControl(selectedSourceConnection);
            loginWindow.ShowDialog();

            if (loginWindow.CrmConnectionInfo != null)
            {
                var mainVM = this.DataContext as MainViewModel;

                if (selectedSourceConnection != null)
                {
                    mainVM.SelectedTargetConnection = loginWindow.CrmConnectionInfo;
                }
                else
                {
                    mainVM.SelectedSourceConnection = loginWindow.CrmConnectionInfo;
                }
            }
        }
    }
}
