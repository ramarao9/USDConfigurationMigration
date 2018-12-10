using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.Models
{
    public enum CRMAttributeType
    {
        String = 0,
        Int = 1,
        Money = 2,
        OptionSetValue = 3,
        Lookup = 4,
        DateTime=5,
        Bool=6
    }
}
