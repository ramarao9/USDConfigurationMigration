using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;
using USDConfigurationMigration.Models;
using USDConfigurationMigrationTests.Helpers;
using USDConfigurationMigration.Repositories;

namespace USDConfigurationMigrationTests.Repositories
{
    [TestClass]
    public class TestHostedControlRepository
    {

        private HostedControlRepository _hostedControlRepository;
        public TestHostedControlRepository()
        {
            _hostedControlRepository = new HostedControlRepository();
        }

        [TestMethod]
        public void TestGetHostedControlsFetchXML()
        {

            Guid configurationid = Guid.NewGuid();
            EntityReference configuration = new EntityReference("msdyusd_configuration", configurationid);
            string fetchxml = _hostedControlRepository.GetHostedControlsFetchXML(configuration);

            bool isValid = fetchxml.Contains(configurationid.ToString());

            Assert.IsTrue(isValid);

        }

        [TestMethod]
        public void TestGetHostedControls()
        {
            CrmServiceClient crmService = TestUtil.GetSourceCRMClient();

            Assert.IsTrue(crmService.IsReady);
            EntityReference configuration = GetConfigurationId();


            CRMEntity crmEntity = _hostedControlRepository.GetHostedControls(crmService, configuration);


            Assert.IsNotNull(crmEntity);

        }


        private EntityReference GetConfigurationId()
        {
            string testConfigId = ConfigurationManager.AppSettings["TestConfigId"];
            Guid configurationid = new Guid(testConfigId);
            EntityReference configuration = new EntityReference("msdyusd_configuration", configurationid);
            return configuration;
        }





    }
}
