using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Tooling.Connector;
using System.Configuration;
using USDConfigurationMigration;
using USDConfigurationMigration.Services;

namespace USDConfigurationMigrationTests
{
    [TestClass]
    public class TestMigrationService
    {
       //[TestMethod]
        public void TestExportData()
        {
            //Arrange
            string connectionStr = ConfigurationManager.ConnectionStrings["CRMSource"].ConnectionString;
            CrmServiceClient crmServiceClient = new CrmServiceClient(connectionStr);
            string configurationToExport = ConfigurationManager.AppSettings["ConfigurationToExport"];
            string exportConfigDataPath = ConfigurationManager.AppSettings["ExportDataFilePath"];

            ExportDataService exportDataService = new ExportDataService();

            //Act
            exportDataService.ExportData(crmServiceClient, configurationToExport, exportConfigDataPath);

            //Assert
         //   Assert.IsTrue(true);

        }


        //[TestMethod]
        public void TestImportData()
        {
            //Arrange
            string connectionStr = ConfigurationManager.ConnectionStrings["CRMTarget"].ConnectionString;
            CrmServiceClient crmServiceClient = new CrmServiceClient(connectionStr);
            ImportDataService importDataService = new ImportDataService();

            string xmlDataPath = ConfigurationManager.AppSettings["ImportDataFile"];


            string logFilePath = ConfigurationManager.AppSettings["LogFilePath"];

            //Act
            importDataService.ImportData(crmServiceClient, xmlDataPath, logFilePath, null);

            //Assert
          //  Assert.IsTrue(true);

        }
    }
}
