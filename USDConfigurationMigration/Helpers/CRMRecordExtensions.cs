using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using USDConfigurationMigration.Models;
using System.Linq;

namespace USDConfigurationMigration.Helpers
{
    public static class CRMRecordExtensions
    {

        public static List<string> CrmAttributeToExclude = new List<string>() { "createdby",
                                                                                "createdonbehalfby",
                                                                                "modifiedby",
                                                                                "createdon",
                                                                                "modifiedonbehalfby",
                                                                                "modifiedon",
                                                                                "ownerid",
                                                                                "owninguser",
                                                                                "owningbusinessteam",
                                                                                "owningbusinessunit",
                                                                                "owningteam",
                                                                                "importsequencenumber",
                                                                                "msdyusd_parentconfigurationid"};

        public static EntityReference ToEntityReference(this CRMRecord crmRecord)
        {
            string primaryAttributeName = crmRecord.LogicalName.StartsWith("uii") ? "uii_name" : "msdyusd_name";


            CRMAttribute primaryNameAtt = (crmRecord.CRMAttributes != null) ? crmRecord.CRMAttributes.Where(x => x.LogicalName == primaryAttributeName).FirstOrDefault() : null;
            string primaryAttributeValue = (primaryNameAtt != null && primaryNameAtt.Value != null) ? primaryNameAtt.Value.ToString() : null;

            EntityReference entityReference = new EntityReference
            {
                Id = crmRecord.Id,
                LogicalName = crmRecord.LogicalName,
                Name = primaryAttributeValue
            };

            return entityReference;
        }


        public static Entity ToEntity(this CRMRecord crmRecord, Dictionary<Guid, Guid> sourceTargetIdMappings)
        {
            Entity entity = null;

            if (crmRecord == null)
                return entity;

            AttributeCollection attributes = new AttributeCollection();

            foreach (var crmAttribute in crmRecord.CRMAttributes)
            {
                if (CrmAttributeToExclude.Contains(crmAttribute.LogicalName) || (crmAttribute.AttributeType != null && crmAttribute.AttributeType.ToLower() == "guid"))
                    continue;

                attributes[crmAttribute.LogicalName] = GetEntityAttribute(crmAttribute, sourceTargetIdMappings);
            }
            entity = new Entity(crmRecord.LogicalName);

            if (crmRecord.Id != Guid.Empty)
            {
                entity.Id = crmRecord.Id;
            }
            entity.Attributes = attributes;
            return entity;
        }


        private static object GetEntityAttribute(CRMAttribute crmAttribute, Dictionary<Guid, Guid> sourceTargetIdMappings)
        {
            object val = null;
            if (crmAttribute.Value != null && crmAttribute.AttributeType != null)
            {
                switch (crmAttribute.AttributeType.ToLower())
                {
                    case "entityreference":

                        Guid sourceId = new Guid(crmAttribute.Value);
                        if (sourceTargetIdMappings != null && sourceTargetIdMappings.ContainsKey(sourceId))
                        {
                            Guid targetId = sourceTargetIdMappings[sourceId];
                            val = new EntityReference(crmAttribute.LookupEntity, targetId);
                        }
                        else
                        {

                            val = new EntityReference(crmAttribute.LookupEntity, sourceId);
                        }


                        break;

                    case "datetime":
                        val = DateTime.Parse(crmAttribute.Value);
                        break;
                    case "boolean":
                        val = bool.Parse(crmAttribute.Value);
                        break;
                    case "string":
                        val = crmAttribute.Value;
                        break;
                    case "int32":
                    case "integer":
                        val = int.Parse(crmAttribute.Value);
                        break;
                    case "decimal":
                        val = decimal.Parse(crmAttribute.Value);
                        break;
                    case "money":
                        val = new Money(Decimal.Parse(crmAttribute.Value));
                        break;
                    case "double":
                        val = double.Parse(crmAttribute.Value);
                        break;
                    case "optionsetvalue":
                        val = new OptionSetValue(int.Parse(crmAttribute.Value));
                        break;

                }
            }
            return val;
        }

    }
}
