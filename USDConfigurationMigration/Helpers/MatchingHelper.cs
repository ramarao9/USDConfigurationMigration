using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDConfigurationMigration.Models;

namespace USDConfigurationMigration.Helpers
{
    public class MatchingHelper
    {

        public Entity GetMatchedEntity(LookupMatchCriteria matchCriteria, Entity sourceEntity, List<Entity> targetEntities)
        {
            Entity matchedEntity = null;
            Guid primaryId = sourceEntity.Id;

            string primaryAttributeName = sourceEntity.LogicalName.StartsWith("uii") ? "uii_name" : "msdyusd_name";
            string primaryAttributeValue = sourceEntity.GetAttributeValue<string>(primaryAttributeName);

            if (matchCriteria.PrimaryIdOrNameMatchOnly)
            {
                matchedEntity = FindEntityUsingId(primaryId, targetEntities);

                if (matchedEntity == null)//No match found on Id, try primary Attribute
                {
                    matchedEntity = FindEntityUsingPrimaryAttribute(primaryAttributeName, primaryAttributeValue, targetEntities);
                }
            }
            else if (matchCriteria.PrimaryIdMatchOnly)
            {
                matchedEntity = FindEntityUsingId(primaryId, targetEntities);
            }
            else if (matchCriteria.PrimaryNameMatchOnly)
            {
                matchedEntity = FindEntityUsingPrimaryAttribute(primaryAttributeName, primaryAttributeValue, targetEntities);
            }
            else if (matchCriteria.AttributesToMatchOn != null)
            {
                matchedEntity = FindEntityUsingMatchOnAttributes(sourceEntity, matchCriteria.AttributesToMatchOn, matchCriteria.OperatorWhenAttributesMatching, targetEntities);
            }

            return matchedEntity;
        }


        private Entity FindEntityUsingMatchOnAttributes(Entity sourceEntity, List<string> attributesToMatchOn, string operatorWhenAttributesMatching, List<Entity> entitiesToFindFrom)
        {
            if (entitiesToFindFrom == null)
                return null;


            Entity matchedEntity = null;

            foreach (var targetEntity in entitiesToFindFrom)
            {

                bool matchFound = false;

                for (int i = 0; i < attributesToMatchOn.Count; i++)
                {

                    string attributeToMatchOn = attributesToMatchOn[i];
                    bool matchedOnAttribute = (targetEntity.Contains(attributeToMatchOn) && sourceEntity.Contains(attributeToMatchOn) &&
                        sourceEntity[attributeToMatchOn] != null && sourceEntity[attributeToMatchOn].Equals(targetEntity[attributeToMatchOn]));


                    if (operatorWhenAttributesMatching != null && operatorWhenAttributesMatching.ToLower() == "or")
                    {
                        matchFound = matchedOnAttribute || matchFound;
                    }
                    else
                    {

                        if (i == 0)
                        {
                            matchFound = matchedOnAttribute;
                        }
                        else
                        {
                            matchFound = matchedOnAttribute && matchFound;
                        }

                    }
                }


                if (matchFound)
                {
                    matchedEntity = targetEntity;
                    break;
                }


            }



            return matchedEntity;//No match found

        }



        private Entity FindEntityUsingId(Guid id, List<Entity> entitiesToFindFrom)
        {
            if (entitiesToFindFrom == null)
                return null;

            Entity entity = entitiesToFindFrom.Where(x => x.Id == id).FirstOrDefault();
            return entity;
        }



        private Entity FindEntityUsingPrimaryAttribute(string primaryAttributeName, string primaryAttributeValue, List<Entity> entitiesToFindFrom)
        {
            if (entitiesToFindFrom == null || string.IsNullOrWhiteSpace(primaryAttributeValue))
                return null;


            Entity entity = entitiesToFindFrom.Where(x => x.GetAttributeValue<string>(primaryAttributeName) != null &&
                                                          x.GetAttributeValue<string>(primaryAttributeName).ToLower() == primaryAttributeValue.ToLower()).FirstOrDefault();
            return entity;
        }




    }
}
