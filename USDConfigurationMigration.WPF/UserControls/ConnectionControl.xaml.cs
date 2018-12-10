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
using USDConfigurationMigration.WPF.ViewModels;

namespace USDConfigurationMigration.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ConnectionControl.xaml
    /// </summary>
    public partial class ConnectionControl : UserControl
    {
        public event EventHandler CloseConnectionControl;

        ConnectionViewModel _connectionViewModel;

        public ConnectionControl()
        {
            InitializeComponent();

            _connectionViewModel = new ConnectionViewModel();
            this.DataContext = _connectionViewModel;

            _connectionViewModel.OnLoginCancelled += Cancel_Click;
            _connectionViewModel.OnConnectionComplete += Connection_Complete;
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_connectionViewModel != null)
            {
                _connectionViewModel.ConnectionItem.Password = ((PasswordBox)sender).SecurePassword;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            CloseConnectionControl?.Invoke(sender, e);
        }

        private void Connection_Complete(object sender, EventArgs e)
        {
            CloseConnectionControl?.Invoke(sender, e);

        }
    }
}
