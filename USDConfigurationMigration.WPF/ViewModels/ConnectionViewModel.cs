using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Tooling.CrmConnectControl.Utility;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using USDConfigurationMigration.WPF.Helpers;
using USDConfigurationMigration.WPF.Models;

namespace USDConfigurationMigration.WPF.ViewModels
{
    public class ConnectionViewModel : NotificationBase
    {

        private ConnectionItem _connectionItem = new ConnectionItem();
        public ConnectionItem ConnectionItem
        {
            get
            {
                return _connectionItem;
            }
            set
            {
                if (_connectionItem != value)
                {
                    _connectionItem = value;
                    RaisePropertyChanged(() => ConnectionItem);
                }
            }
        }

        private ICommand _closeNotificationCommand;
        public ICommand CloseNotificationCommand
        {
            get { return _closeNotificationCommand; }
        }

        private bool _saveConnection = false;
        public bool SaveConnection
        {
            get { return _saveConnection; }
            set
            {


                if (_saveConnection != value)
                {
                    _saveConnection = value;
                    RaisePropertyChanged(() => SaveConnection);
                }
            }

        }


        private bool _useNewConnection = false;
        public bool UseNewConnection
        {
            get { return _useNewConnection; }
            set
            {


                if (_useNewConnection != value)
                {
                    _useNewConnection = value;
                    RaisePropertyChanged(() => UseNewConnection);
                }
            }

        }

        private bool _useExistingConnection = false;
        public bool UseExistingConnection
        {
            get { return _useExistingConnection; }
            set
            {


                if (_useExistingConnection != value)
                {
                    _useExistingConnection = value;
                    RaisePropertyChanged(() => UseExistingConnection);
                }
            }

        }


        private bool _connectionInProgress = false;
        public bool ConnectionInProgress
        {
            get { return _connectionInProgress; }
            set
            {
                if (_connectionInProgress != value)
                {
                    _connectionInProgress = value;
                    RaisePropertyChanged(() => ConnectionInProgress);
                }
            }
        }

        private ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get { return _connectCommand; }
        }


        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }


        private int _connectionToUse;
        public int ConnectionToUse
        {
            get
            {
                return _connectionToUse;
            }
            set
            {

                if (_connectionToUse != value)
                {
                    _connectionToUse = value;

                    UseNewConnection = (_connectionToUse == 2);
                    UseExistingConnection = (_connectionToUse == 1);
                    RaisePropertyChanged(() => ConnectionToUse);
                }
            }
        }



        private ObservableCollection<CrmConnectionInfo> _availableConnections = new ObservableCollection<CrmConnectionInfo>();
        public ObservableCollection<CrmConnectionInfo> AvailableConnections
        {
            set
            {

                _availableConnections = value;
            }
            get { return _availableConnections; }
        }



        private bool _hasExistingConnections = false;
        public bool HasExistingConnections
        {
            get { return _hasExistingConnections; }
            set
            {
                if (_hasExistingConnections != value)
                {
                    _hasExistingConnections = value;
                    RaisePropertyChanged(() => HasExistingConnections);
                }
            }
        }

        private MessageNotification _messageNotification = new MessageNotification();
        public MessageNotification MessageNotification
        {
            get { return _messageNotification; }
            set
            {
                if (_messageNotification != value)
                {
                    _messageNotification = value;
                    RaisePropertyChanged(() => MessageNotification);
                }
            }
        }



        private bool _showMessageNotification = false;
        public bool ShowMessageNotification
        {
            get { return _showMessageNotification; }
            set
            {
                if (_showMessageNotification != value)
                {
                    _showMessageNotification = value;
                    RaisePropertyChanged(() => ShowMessageNotification);
                }
            }
        }

        public event EventHandler OnConnectionComplete;
        public event EventHandler OnLoginCancelled;


        private CrmConnectionInfo _connectionToExclude;
        public CrmConnectionInfo ConnectionToExclude
        {
            get
            {
                return _connectionToExclude;
            }
            set
            {
                _connectionToExclude = value;
            }
        }

        private CrmConnectionInfo _selectedConnection;
        public CrmConnectionInfo SelectedConnection
        {
            get
            {
                return _selectedConnection;
            }
            set
            {
                if (_selectedConnection != value)
                {
                    _selectedConnection = value;
                    RaisePropertyChanged(() => SelectedConnection);
                }
            }
        }


        ConnectionHelper _connectionHelper;
        public ConnectionViewModel()
        {
            _closeNotificationCommand = new RelayCommand(CloseNotification);

            _cancelCommand = new RelayCommand(CancelLogin);

            _connectCommand = new RelayCommand(BuildConnectionInfo);

            _connectionHelper = new ConnectionHelper();

        }


        public void Init()
        {
            List<CrmConnectionInfo> availableConnections = _connectionHelper.RetrieveAvailableConnections();

            if (ConnectionToExclude != null && availableConnections != null)
            {
                availableConnections.RemoveAll(x => x.UserId == ConnectionToExclude.UserId && x.OrganizationId == ConnectionToExclude.OrganizationId);
            }

            HasExistingConnections = availableConnections.Count > 0;

            if (!HasExistingConnections)
            {
                UseNewConnection = true;
            }

            foreach (CrmConnectionInfo crmConnectionInfo in availableConnections)
            {
                AvailableConnections.Add(crmConnectionInfo);
            }
        }


        public void BuildConnectionInfo()
        {
            Task t = BuildConnectionAsync();
        }


        public async Task BuildConnectionAsync()
        {
            ConnectionInProgress = true;

            string connectionString = "";
            CrmConnectionInfo crmConnectionInfo = null;


            ConnectionHelper connectionHelper = new ConnectionHelper();

            if (SelectedConnection != null)
            {
                crmConnectionInfo = SelectedConnection;
                connectionString = connectionHelper.BuildConnectionString(SelectedConnection);
            }
            else
            {



                if (ConnectionItem == null || string.IsNullOrWhiteSpace(ConnectionItem.OrganizationURL) || !IsURLValid(ConnectionItem.OrganizationURL))
                {
                    SetMessageNotification(NotificationType.Error, "Please provide a  valid Organization URL");
                }

                if (string.IsNullOrWhiteSpace(ConnectionItem.AuthType))
                {
                    SetMessageNotification(NotificationType.Error, "Please provide Auth Type");
                }

                connectionString = connectionHelper.BuildConnectionString(ConnectionItem.OrganizationURL,
                                                                                ConnectionItem.AuthType,
                                                                                ConnectionItem.Domain,
                                                                                ConnectionItem.UserName,
                                                                                ConnectionItem.Password);
            }
            CrmServiceClient crmServiceClient = null;
            string connectionError = null;
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    crmServiceClient = new CrmServiceClient(connectionString);

                    connectionError = crmServiceClient.LastCrmError;
                }
                catch (Exception ex)
                {
                    connectionError = ex.Message;
                }

            });

            if (!string.IsNullOrWhiteSpace(connectionError))
            {

                SetMessageNotification(NotificationType.Error, connectionError);
            }

            ConnectionInProgress = false;

            if (crmServiceClient != null && crmServiceClient.IsReady)
            {
                if (crmConnectionInfo == null)
                {
                    crmConnectionInfo = GetCrmConnectionInfo(crmServiceClient);

                    if (SaveConnection)
                    {
                        SaveConnectionInfo(crmConnectionInfo);
                    }
                }

                ConnectionComplete(crmConnectionInfo);
            }

        }


        private bool IsURLValid(string url)
        {
            Uri uri;
            bool validUrl = Uri.TryCreate(url, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
            return validUrl;
        }


        private void SaveConnectionInfo(CrmConnectionInfo crmConnectionInfo)
        {
            ConnectionHelper connectionHelper = new ConnectionHelper();

            string credentialId = connectionHelper.GetCredentialId(crmConnectionInfo);
            SaveCredential(credentialId);

            connectionHelper.AddNewConnection(crmConnectionInfo);

        }


        private CrmConnectionInfo GetCrmConnectionInfo(CrmServiceClient crmServiceClient)
        {
            WhoAmIRequest whoAmIRequest = new WhoAmIRequest();

            WhoAmIResponse whoAmIresponse = (WhoAmIResponse)crmServiceClient.Execute(whoAmIRequest);


            CrmConnectionInfo crmConnectionInfo = new CrmConnectionInfo
            {
                OrganizationId = whoAmIresponse.OrganizationId,
                UserId = whoAmIresponse.UserId,
                OrganizationURL = ConnectionItem.OrganizationURL,
                OrgDisplayName = crmServiceClient.ConnectedOrgFriendlyName,
                OrgUniqueName = crmServiceClient.ConnectedOrgUniqueName,
                AuthType = ConnectionItem.AuthType,
                Domain = ConnectionItem.Domain
            };

            return crmConnectionInfo;
        }

        private void SaveCredential(string credentialId)
        {

            SavedCredentials savedCredentials = new SavedCredentials { UserName = ConnectionItem.UserName, Password = ConnectionItem.Password };
            CredentialManager.WriteCredentials(credentialId, savedCredentials, true);

        }




        private void SetMessageNotification(NotificationType notificationType, string message)
        {
            ShowMessageNotification = true;
            MessageNotification.Message = message;
            MessageNotification.MessageType = (int)notificationType;
        }

        public void CloseNotification()
        {
            ShowMessageNotification = false;
            MessageNotification.MessageType = (int)NotificationType.None;
            MessageNotification.Message = string.Empty;

        }


        public void CancelLogin()
        {

            OnLoginCancelled?.Invoke(null, null);
        }


        public void ConnectionComplete(CrmConnectionInfo crmConnectionInfo)
        {

            OnConnectionComplete?.Invoke(crmConnectionInfo, null);
        }
    }
}
