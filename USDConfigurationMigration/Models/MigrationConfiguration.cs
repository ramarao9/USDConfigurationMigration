using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.Models
{
    public class MigrationConfiguration
    {
        public string ConfigurationName { get; set; }
        public string ConfigExportFolder { get; set; }
        public string SourceConnectionString { get; set; }
        public string TargetConnectionString { get; set; }
        public List<LookupMatchCriteria> LookupMatchCriterias { get; set; }
    }
}
