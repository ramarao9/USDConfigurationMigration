using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDConfigurationMigration.Models;
using USDConfigurationMigrationTests.Helpers;
using USDConfigurationMigration.Repositories;

namespace USDConfigurationMigrationTests.Repositories
{
    [TestClass]
    public class TestSubActionCallsRepository
    {

        private SubActionCallsRepository _subActionCallsRepository;
        public TestSubActionCallsRepository()
        {
            _subActionCallsRepository = new SubActionCallsRepository();
        }

      //  [TestMethod]
        public void TestGetSubActionCalls()
        {

            CrmServiceClient crmService = TestUtil.GetSourceCRMClient();

            List<CRMRecord> crmRecords = new List<CRMRecord>();
            crmRecords.Add(new CRMRecord { Id = new Guid("8D7D25F5-653E-E811-8109-00155DCFD33F"), CRMAttributes= new List<CRMAttribute>() });
            crmRecords.Add(new CRMRecord { Id = new Guid("7842888B-257E-E811-810E-00155DCFD33F") , CRMAttributes = new List<CRMAttribute>() });

            CRMEntity actionCallCE = new CRMEntity { CRMRecords = crmRecords };

            CRMEntity subActionCallsCE = _subActionCallsRepository.GetSubActionCalls(crmService, actionCallCE);

         
            Assert.IsNotNull(subActionCallsCE);

        }



    }
}
