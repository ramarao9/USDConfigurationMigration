using Microsoft.Xrm.Tooling.CrmConnectControl.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using USDConfigurationMigration.WPF.Models;

namespace USDConfigurationMigration.WPF.Helpers
{
    public class ConnectionHelper
    {

        public string BuildConnectionString(CrmConnectionInfo crmConnectionInfo)
        {
            string credentialId = GetCredentialId(crmConnectionInfo);


            SavedCredentials credential = CredentialManager.ReadCredentials(credentialId);




            string connectionString = BuildConnectionString(crmConnectionInfo.OrganizationURL,
                                                                              crmConnectionInfo.AuthType,
                                                                              crmConnectionInfo.Domain,
                                                                              credential.UserName,
                                                                              credential.Password);

            return connectionString;
        }


        public string BuildConnectionString(string organizationUrl, string authType, string domain, string userName, SecureString securePassword)
        {
            string connectionString = "";

            Dictionary<string, string> connectionParameters = new Dictionary<string, string>();
            connectionParameters.Add("Url", organizationUrl);
            connectionParameters.Add("RequireNewInstance", "true");
            connectionParameters.Add("AuthType", authType);

            if (!string.IsNullOrWhiteSpace(domain))
            {
                connectionParameters.Add("Domain", domain);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                connectionParameters.Add("Username", userName);
            }
            string password = (securePassword != null) ? SecureStringToString(securePassword) : "";

            if (!string.IsNullOrWhiteSpace(password))
            {
                connectionParameters.Add("Password", password);
            }

            foreach (var kvp in connectionParameters)
            {
                connectionString += kvp.Key + "=" + kvp.Value + ";";
            }

            return connectionString;
        }

        private string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }


        public string GetCredentialId(CrmConnectionInfo crmConnectionInfo)
        {

            string credentialId = crmConnectionInfo.OrganizationId.ToString().Replace("-", "") + "_" + crmConnectionInfo.UserId.ToString().Replace("-", "");
            return credentialId;
        }


        public void AddNewConnection(CrmConnectionInfo crmConnectionInfo)
        {
            List<CrmConnectionInfo> crmConnections = RetrieveAvailableConnections();

            CrmConnectionInfo existingCrmConnectionInfo = crmConnections.FirstOrDefault(x => x.OrganizationId == crmConnectionInfo.OrganizationId);

            if (existingCrmConnectionInfo != null)
            {
                existingCrmConnectionInfo = crmConnectionInfo;//To Do might need to remove the Old Credential if one exists
            }
            else
            {
                crmConnections.Add(crmConnectionInfo);
            }
            UpdateConnections(crmConnections);
        }





        public List<CrmConnectionInfo> RetrieveAvailableConnections()
        {
            List<CrmConnectionInfo> crmConnections = null;

            string connectionsFilePath = GetConnectionsFilePath();

            XMLSerialization xmlSerialization = new XMLSerialization();
            crmConnections = xmlSerialization.DeSerialize<List<CrmConnectionInfo>>(connectionsFilePath);

            if (crmConnections == null)
            {
                crmConnections = new List<CrmConnectionInfo>();
            }

            return crmConnections;
        }


        public CrmConnectionInfo RetrieveConnection(Guid orgId)
        {
            List<CrmConnectionInfo> crmConnections = RetrieveAvailableConnections();

            CrmConnectionInfo connectionInfo = crmConnections.FirstOrDefault(x => x.OrganizationId == orgId);

            return connectionInfo;

        }


        public void DeleteConnection(Guid orgId)
        {
            List<CrmConnectionInfo> crmConnections = RetrieveAvailableConnections();

            CrmConnectionInfo connectionToRemove = crmConnections.FirstOrDefault(x => x.OrganizationId == orgId);


            if (connectionToRemove != null)
            {
                crmConnections.Remove(connectionToRemove);
            }

            UpdateConnections(crmConnections);

        }



        private string GetConnectionsFilePath()
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            string appDirectory = Path.Combine(appDataDirectory, appName);
            if (!Directory.Exists(appDirectory))
                Directory.CreateDirectory(appDirectory);


            string connectionsFileName = "Connections.xml";
            string connectionsFilePath = Path.Combine(appDirectory, connectionsFileName);

            if (!File.Exists(connectionsFilePath))
            {
                File.Create(connectionsFilePath);

            }

            return connectionsFilePath;


        }

        private void UpdateConnections(List<CrmConnectionInfo> crmConnections)
        {

            string connectionsFilePath = GetConnectionsFilePath();

            XMLSerialization xmlSerialization = new XMLSerialization();
            string crmConnectionsSerialized = xmlSerialization.Serialize(crmConnections);


            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(crmConnectionsSerialized);
            xdoc.Save(connectionsFilePath);

            // File.WriteAllText(connectionsFilePath, crmConnectionsSerialized);

        }

    }
}
