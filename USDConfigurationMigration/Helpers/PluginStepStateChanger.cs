using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.Helpers
{
    public class PluginStepStateChanger
    {
        public enum PluginStatus
        {
            Enabled = 0,
            Disabled = 1
        }

        public enum PluginStatusReason
        {
            Enabled = 1,
            Disabled = 2
        }


        public bool EnablePlugins(IOrganizationService service, List<string> pluginAssemblies)
        {
            try
            {
                List<EntityReference> pluginSteps = GetPluginSteps(service, pluginAssemblies);
                foreach (EntityReference pluginStep in pluginSteps)
                {
                    ChangeState(service, pluginStep, (int)PluginStatus.Enabled, (int)PluginStatusReason.Enabled);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;// Do not continue
            }
        }


        public bool DisablePlugins(IOrganizationService service, List<string> pluginAssemblies)
        {
            try
            {
                List<EntityReference> pluginSteps = GetPluginSteps(service, pluginAssemblies);
                foreach (EntityReference pluginStep in pluginSteps)
                {
                    ChangeState(service, pluginStep, (int)PluginStatus.Disabled, (int)PluginStatusReason.Disabled);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;// Do not continue
            }
        }


        private List<EntityReference> GetPluginSteps(IOrganizationService service, List<string> pluginAssemblies)
        {
            List<EntityReference> pluginSteps = new List<EntityReference>();

            string assemblyConditions = "";
            foreach (string assemblyName in pluginAssemblies)
            {
                assemblyConditions += @" <condition attribute='assemblyname' operator='eq' value='" + assemblyName + "' />";
            }

            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                  <entity name='sdkmessageprocessingstep'>
                                    <attribute name='name' />
                                    <attribute name='sdkmessageprocessingstepid' />
                                    <link-entity name='plugintype' from='plugintypeid' to='plugintypeid' alias='ae'>
                                      <filter type='and'>
                                        <filter type='or'>
                                          {0}
                                        </filter>
                                      </filter>
                                    </link-entity>
                                  </entity>
                                </fetch>";


            fetchXml = string.Format(fetchXml, assemblyConditions);


            EntityCollection results = service.RetrieveMultiple(new FetchExpression(fetchXml));
            if (results != null && results.Entities != null && results.Entities.Count > 0)
            {
                pluginSteps = results.Entities.Select(x => x.ToEntityReference()).ToList();
            }

            return pluginSteps;
        }

        private void ChangeState(IOrganizationService service, EntityReference pluginStep, int stateCode, int statusCode)
        {

            SetStateRequest setStaterequest = new SetStateRequest
            {
                EntityMoniker = pluginStep,
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };

            service.Execute(setStaterequest);
        }

    }
}
