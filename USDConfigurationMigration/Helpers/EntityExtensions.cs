using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Globalization;
using USDConfigurationMigration.Models;
using USDConfigurationMigration.Helpers;

namespace USDConfigurationMigration.Helpers
{
    public static class EntityExtensions
    {

        public static CRMRecord ToCRMRecord(this Entity entity)
        {
            CRMRecord crmrecord = null;

            if (entity == null)
                return crmrecord;

            List<string> attributesToExclude = Util.GetCRMAttributesToExclude();

            List<CRMAttribute> crmAttributes = new List<CRMAttribute>();

            foreach (var attribute in entity.Attributes)
            {

                if (attributesToExclude.Contains(attribute.Key))
                    continue;

                CRMAttribute crmAttribute = GetCRMAttribute(attribute);
                crmAttributes.Add(crmAttribute);
            }

            crmrecord = new CRMRecord { LogicalName = entity.LogicalName, Id = entity.Id, CRMAttributes = crmAttributes };

            return crmrecord;
        }

        public static CRMM2MRecord ToCRMM2MRecord(this Entity entity, CRMM2MEntityMapping crmM2MEntityMapping)
        {
            CRMM2MRecord crmM2Mrecord = null;

            if (entity == null)
                return crmM2Mrecord;

            Guid entity1AttributeId = entity.GetAttributeValue<Guid>(crmM2MEntityMapping.Entity1Attribute);
            Guid entity2AttributeId = entity.GetAttributeValue<Guid>(crmM2MEntityMapping.Entity2Attribute);

            crmM2Mrecord = new CRMM2MRecord
            {
                EntityLogicalName = entity.LogicalName,
                Entity1LogicalName = crmM2MEntityMapping.Entity1,
                Entity1AttributeId = entity1AttributeId,
                Entity2LogicalName = crmM2MEntityMapping.Entity2,
                Entity2AttributeId = entity2AttributeId
            };

            return crmM2Mrecord;
        }


        public static CRMAttribute GetCRMAttribute(KeyValuePair<string, object> attribute)
        {

            string attValue = null;
            string lookupEntity = null;

            Object attObj = attribute.Value;

            string attType = (attObj != null) ? attObj.GetType().Name : null;

            if (attType != null)
            {
                switch (attType.ToLower())
                {
                    case "entityreference":
                        EntityReference er = attObj as EntityReference;
                        lookupEntity = er.LogicalName;
                        attValue = er.Id.ToString();
                        break;

                    case "datetime":
                        DateTime dt = (DateTime)attObj;
                        attValue = dt.ToString("M/d/yyyy hh:mm:ss tt",
                                    CultureInfo.InvariantCulture);
                        break;

                    case "money":
                        Money money = (Money)attObj;
                        attValue = money.Value.ToString();
                        break;

                    case "optionsetvalue":
                        OptionSetValue osv = (OptionSetValue)attObj;
                        attValue = osv.Value.ToString();
                        break;

                    default:
                        attValue = attObj.ToString();
                        break;

                }
            }

            CRMAttribute crmAttribute = new CRMAttribute
            {
                LogicalName = attribute.Key,
                Value = attValue,
                AttributeType = attType,
                LookupEntity = lookupEntity
            };


            return crmAttribute;
        }


        public static Entity GetModifiedEntity(this Entity wsSource, Entity crmTarget)
        {
            Entity entity = new Entity(crmTarget.LogicalName);
            entity.Id = crmTarget.Id;

            foreach (var attribute in wsSource.Attributes)
            {
                String keyName = attribute.Key;
                object sourceValue = wsSource[keyName];
                if (crmTarget.Attributes.Contains(keyName))
                {

                    object targetValue = crmTarget[keyName];
                    // Nothing to do if source and target values are null
                    if ((sourceValue == null) &&
                        (targetValue == null))
                    {
                        continue;
                    }

                    // if source is null, overwrite target regardless. 
                    if ((sourceValue == null) &&
                        (targetValue != null))
                    {
                        entity[keyName] = null;
                        continue;
                    }

                    // For DateTime attributes, perform special comparison
                    if (sourceValue != null)
                    {
                        String attributeType = sourceValue.GetType().Name;
                        if (attributeType == "DateTime")
                        {
                            if (targetValue != null)
                            {

                                DateTime sourceCrmDate = (DateTime)sourceValue;
                                DateTime targetCrmDate = (DateTime)targetValue;

                                if (sourceCrmDate != DateTime.MinValue && sourceCrmDate != targetCrmDate)
                                {
                                    entity[keyName] = sourceCrmDate;
                                }
                            }
                            else
                            {
                                DateTime sourceDate = (DateTime)sourceValue;
                                if (sourceDate >= new DateTime(1900, 1, 1))//Since CRM does not take <1900
                                {
                                    entity[keyName] = sourceDate;
                                }

                            }
                            continue;
                        }
                    }
                    if (!sourceValue.Equals(targetValue))
                    {
                        entity[keyName] = sourceValue;
                    }

                }
                else
                {
                    entity[keyName] = sourceValue;
                }

            }
            return entity;
        }

    }
}
