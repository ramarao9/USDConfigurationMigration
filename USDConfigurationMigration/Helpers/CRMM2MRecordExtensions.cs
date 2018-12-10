using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDConfigurationMigration.Models;

namespace USDConfigurationMigration.Helpers
{
    public static class CRMM2MRecordExtensions
    {

        public static Entity ToEntity(this CRMM2MRecord crmM2MRecord, CRMM2MEntityMapping m2mEntityMapping, Dictionary<Guid, Guid> sourceTargetIdMappings)
        {
            Entity entity = null;

            if (crmM2MRecord == null)
                return entity;


            entity = new Entity(crmM2MRecord.EntityLogicalName);
            entity[m2mEntityMapping.Entity1Attribute] = crmM2MRecord.Entity1AttributeId;
            entity[m2mEntityMapping.Entity2Attribute] = crmM2MRecord.Entity2AttributeId;

            return entity;

        }
    }
}
