using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.Helpers
{
    public static class Util
    {

        public static List<string> GetCRMAttributesToExclude()
        {
            return new List<string>() { "createdby",
                                        "createdonbehalfby",
                                        "modifiedby",
                                        "createdon",
                                        "modifiedonbehalfby",
                                        "modifiedon",
                                        "ownerid",
                                        "owningbusinessteam",
                                        "owningbusinessunit",
                                        "owningteam",
                                        "importsequencenumber",
                                        "msdyusd_parentconfigurationid"};
        }



    }

}
