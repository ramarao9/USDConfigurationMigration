using System;

namespace USDConfigurationMigration.Models
{
    public class CRMM2MRecord
    {
        public string EntityLogicalName { get; set; }

        public string Entity1LogicalName { get; set; }
        public Guid Entity1AttributeId { get; set; }

        public string Entity2LogicalName { get; set; }
        public Guid Entity2AttributeId { get; set; }
    }
}
