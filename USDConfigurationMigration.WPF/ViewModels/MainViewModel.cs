using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using USDConfigurationMigration.Services;
using USDConfigurationMigration.WPF.Helpers;
using USDConfigurationMigration.WPF.Models;
using USDConfigurationMigration.WPF.Repositories;

namespace USDConfigurationMigration.WPF.ViewModels
{
    public class MainViewModel : NotificationBase
    {

        private bool _canInitiateMigration = true;
        public bool CanInitiateMigration
        {
            get { return _canInitiateMigration; }
            set
            {
                if (_canInitiateMigration != value)
                {
                    _canInitiateMigration = value;
                    RaisePropertyChanged(() => CanInitiateMigration);
                }
            }
        }

        private CrmConnectionInfo _selectedSourceConnection = new CrmConnectionInfo { OrgDisplayName = "Select source.." };
        public CrmConnectionInfo SelectedSourceConnection
        {
            get
            {
                return _selectedSourceConnection;
            }
            set
            {
                if (_selectedSourceConnection != value)
                {
                    _selectedSourceConnection = value;


                    RaisePropertyChanged(() => SelectedSourceConnection);

                    var retrieveConfigsTask = RetrieveConfigurationsAsync();
                }
            }
        }


        private CrmConnectionInfo _selectedTargetConnection = new CrmConnectionInfo { OrgDisplayName = "Select target.." };
        public CrmConnectionInfo SelectedTargetConnection
        {
            get
            {
                return _selectedTargetConnection;
            }
            set
            {
                if (_selectedTargetConnection != value)
                {
                    _selectedTargetConnection = value;
                    RaisePropertyChanged(() => SelectedTargetConnection);
                }
            }
        }




        private ICommand _migrateCommand;
        public ICommand MigrateCommand
        {
            get { return _migrateCommand; }
        }


        private ObservableCollection<ConfigurationItem> _sourceUSDConfigurations = new ObservableCollection<ConfigurationItem>();
        public ObservableCollection<ConfigurationItem> SourceUSDConfigurations
        {
            set
            {

                _sourceUSDConfigurations = value;
            }
            get { return _sourceUSDConfigurations; }
        }

        private ConfigurationItem _selectedConfiguration;
        public ConfigurationItem SelectedConfiguration
        {
            get
            {
                return _selectedConfiguration;
            }
            set
            {
                if (_selectedConfiguration != value)
                {
                    _selectedConfiguration = value;
                    RaisePropertyChanged(() => SelectedConfiguration);
                }
            }
        }

        private ConnectionHelper _connectionHelper;

        private ConfigurationRepository _configurationRepository;

        public MainViewModel()
        {
            Init();
        }


        public void Init()
        {
            _connectionHelper = new ConnectionHelper();

            _configurationRepository = new ConfigurationRepository();

            _migrateCommand = new RelayCommand(PerformMigration);
        }


        private async Task RetrieveConfigurationsAsync()
        {

            List<ConfigurationItem> configurationItems = await GetConfigurations();

            if (configurationItems != null)
            {
                foreach (ConfigurationItem configItem in configurationItems)
                {
                    SourceUSDConfigurations.Add(configItem);
                }
            }

        }


        private async Task<List<ConfigurationItem>> GetConfigurations()
        {
            return await Task.Factory.StartNew(() =>
            {


                CrmServiceClient crmService = GetCrmService(SelectedSourceConnection);
                List<ConfigurationItem> configurationItems = _configurationRepository.GetConfigurations(crmService);


                return configurationItems;
            });
        }



        private CrmServiceClient GetCrmService(CrmConnectionInfo crmConnectionInfo)
        {
            string connStr = _connectionHelper.BuildConnectionString(crmConnectionInfo);
            CrmServiceClient crmService = new CrmServiceClient(connStr);
            return crmService;
        }

        private void PerformMigration()
        {
            var tTask = MigrateAsync();

        }


        private async Task MigrateAsync()
        {

            await Task.Factory.StartNew(() =>
            {

                ExportDataService exportDataService = new ExportDataService();
                string currentAppDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string getjobFolderName = GetJobFolderName();
                string jobFolderPath = Path.Combine(currentAppDirectory, getjobFolderName);

                CreateFolderIfNotExists(jobFolderPath);

                CrmServiceClient sourceCrmService = GetCrmService(SelectedSourceConnection);
                exportDataService.ExportData(sourceCrmService, SelectedConfiguration.Name, jobFolderPath);


                CrmServiceClient targetCrmService = GetCrmService(SelectedTargetConnection);
                ImportDataService importDataService = new ImportDataService();

                string xmlDataPath = Path.Combine(jobFolderPath, "data.xml");

                string logFilePath = Path.Combine(jobFolderPath, "Logs");

                CreateFolderIfNotExists(logFilePath);

                importDataService.ImportData(targetCrmService, xmlDataPath, logFilePath, null);

            });
        }

        private string GetJobFolderName()
        {
            string configName = SelectedConfiguration.Name;
            string jobFolderName = configName + "_" + SelectedSourceConnection.OrgUniqueName + "_" +
                SelectedTargetConnection.OrgUniqueName + "_" + DateTime.Now.ToString("MMddyyyyHHmm");
            return jobFolderName;
        }

        private void CreateFolderIfNotExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}
