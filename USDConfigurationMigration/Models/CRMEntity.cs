using System.Collections.Generic;

namespace USDConfigurationMigration.Models
{
    public class CRMEntity
    {
        public string LogicalName { get; set; }

        public bool IsIntersect { get; set; }

        public int RecordCount { get; set; }

        public List<CRMM2MRecord> CRMM2MRecords { get; set; }

        public List<CRMRecord> CRMRecords { get; set; }
    }
}
