using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.Models
{
    public class USDConfiguration
    {
        public string Name { get; set; }
        public List<CRMEntity> CRMEntities { get; set; }
    }
}
