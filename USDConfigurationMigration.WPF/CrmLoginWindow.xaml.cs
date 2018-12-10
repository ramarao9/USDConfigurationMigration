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
using USDConfigurationMigration.WPF.Models;
using USDConfigurationMigration.WPF.ViewModels;

namespace USDConfigurationMigration.WPF
{
    /// <summary>
    /// Interaction logic for CrmLoginWindow.xaml
    /// </summary>
    public partial class CrmLoginWindow : Window
    {
        public CrmConnectionInfo CrmConnectionInfo
        {
            get;
            set;
        }





        public CrmLoginWindow()
        {
            InitializeComponent();

            InitializeLoginControl();
        }


        public void InitConnectionControl(CrmConnectionInfo crmConnectionToExclude)
        {
            var connectionViewModel = connectionUC.DataContext as ConnectionViewModel;
            connectionViewModel.ConnectionToExclude = crmConnectionToExclude;

            connectionViewModel.Init();
        }


        private void InitializeLoginControl()
        {
            connectionUC.CloseConnectionControl += CloseConnectionWindow;
        }




        private void CloseConnectionWindow(object sender, EventArgs e)
        {
            if (sender != null)
            {
                CrmConnectionInfo = sender as CrmConnectionInfo;
            }

            this.Close();
        }
    }
}
