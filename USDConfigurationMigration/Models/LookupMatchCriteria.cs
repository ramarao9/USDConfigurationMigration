using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.Models
{
    public class LookupMatchCriteria
    {
        public string EntityLogicalName { get; set; }

        public bool PrimaryIdMatchOnly { get; set; }
        public bool PrimaryNameMatchOnly { get; set; }
        public bool PrimaryIdOrNameMatchOnly { get; set; }


        public List<string> AttributesToMatchOn { get; set; }

        public string OperatorWhenAttributesMatching { get; set; }
    }
}
