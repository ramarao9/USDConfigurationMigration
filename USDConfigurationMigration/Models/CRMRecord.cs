using System;
using System.Collections.Generic;

namespace USDConfigurationMigration.Models
{
    public class CRMRecord
    {
        public Guid Id { get; set; }
        public string LogicalName { get; set; }
        public List<CRMAttribute> CRMAttributes { get; set; }
    }
}
