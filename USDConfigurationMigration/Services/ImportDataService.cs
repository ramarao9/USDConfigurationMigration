using Microsoft.Xrm.Sdk;
using System.Linq;
using Microsoft.Xrm.Tooling.Connector;
using System.Collections.Generic;
using USDConfigurationMigration.Models;
using USDConfigurationMigration.Helpers;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Text;
using System.IO;
using USDConfigurationMigration.Models;
using USDConfigurationMigration.Helpers;
using USDConfigurationMigration.Repositories;

namespace USDConfigurationMigration.Services
{
    public class ImportDataService
    {
        List<ImportResult> _importResults;
        ExportDataService _exportDataService;
        CRMRepository _crmRepository;
        EventRepository _eventRepository;
        ScriptletRepository _scriptletRepository;
        ConfigurationRepository _configurationRespository;
        HostedControlRepository _hostedControlRepository;
        EntityTypeRepository _entityTypeRepository;
        EntitySearchRepository _entitySearchRepository;
        SessionLineRepository _sessionLineRepository;
        OptionRepository _optionRepository;
        ActionRepository _actionRepository;
        ActionCallRepository _actionCallrepository;
        SubActionCallsRepository _subActionCallsRepository;
        EventActionCallRepository _eventActionCallRepository;
        ToolbarRepository _toolbarRepository;
        ToolbarButtonRepository _toolbarButtonRepository;
        ToolbarButtonActionCallRepository _toolbarButtonActionCallRepository;
        ToolbarHostedControlRepository _toolbarHostedControlRepository;
        WNRRepository _wnrRepository;
        WNRActionCallRepository _wnrActionCallrepository;
        AgentScriptTaskRepository _agentScriptTaskRepository;
        TaskActionCallRepository _taskActionCallRepository;
        TaskAnswerRepository _taskAnswerRepository;
        AgentScriptAnswerRepository _agentScriptAnswerRepository;
        AnswerActionCallrepository _answerActionCallRepository;


        public ImportDataService()
        {


            _exportDataService = new ExportDataService();

            _crmRepository = new CRMRepository();
            _eventRepository = new EventRepository();
            _configurationRespository = new ConfigurationRepository();
            _hostedControlRepository = new HostedControlRepository();
            _entityTypeRepository = new EntityTypeRepository();
            _scriptletRepository = new ScriptletRepository();
            _importResults = new List<ImportResult>();
            _entitySearchRepository = new EntitySearchRepository();
            _sessionLineRepository = new SessionLineRepository();
            _optionRepository = new OptionRepository();
            _actionRepository = new ActionRepository();
            _actionCallrepository = new ActionCallRepository();
            _subActionCallsRepository = new SubActionCallsRepository();
            _eventActionCallRepository = new EventActionCallRepository();
            _toolbarRepository = new ToolbarRepository();
            _toolbarButtonRepository = new ToolbarButtonRepository();
            _toolbarButtonActionCallRepository = new ToolbarButtonActionCallRepository();
            _toolbarHostedControlRepository = new ToolbarHostedControlRepository();
            _wnrRepository = new WNRRepository();
            _wnrActionCallrepository = new WNRActionCallRepository();
            _agentScriptTaskRepository = new AgentScriptTaskRepository();
            _taskActionCallRepository = new TaskActionCallRepository();
            _taskAnswerRepository = new TaskAnswerRepository();
            _agentScriptAnswerRepository = new AgentScriptAnswerRepository();
            _answerActionCallRepository = new AnswerActionCallrepository();
        }
        private List<String> GetUSDAssemblies()
        {
            return new List<string>
            {
                "Microsoft.Crm.UnifiedServiceDesk.Plugin",
                "Microsoft.Uii.Customization.Plugin"
            };
        }


        public void ImportData(CrmServiceClient targetCrmService, string dataFilePath, string logFilePath, List<LookupMatchCriteria> lookupMatchCriterias)
        {
            PluginStepStateChanger pluginStateChanger = new PluginStepStateChanger();
            string xmlData = File.ReadAllText(dataFilePath);
            USDConfiguration sourceUSDConfiguration = xmlData.XmlDeSerialize<USDConfiguration>();
            if (sourceUSDConfiguration == null || sourceUSDConfiguration.CRMEntities == null)
                throw new Exception("Empty or invalid configuration. Please check the import data file.");

            List<String> usdAssemblies = GetUSDAssemblies();


            USDConfiguration targetUsdConfiguration = _exportDataService.GetUSDConfiguration(targetCrmService, sourceUSDConfiguration.Name);

            if (lookupMatchCriterias == null)
            {
                lookupMatchCriterias = _crmRepository.GetDefaultLookupMatchCriterias();
            }


            Guid configurationId = sourceUSDConfiguration.CRMEntities.First(x => x.LogicalName == "msdyusd_configuration").CRMRecords[0].Id;

            bool pluginsDisabled = pluginStateChanger.DisablePlugins(targetCrmService, usdAssemblies);

            if (!pluginsDisabled)
                throw new Exception("Unable to disbaled plugins. Import process cannot continue");


            BeginImport(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            pluginStateChanger.EnablePlugins(targetCrmService, usdAssemblies);

            GenerateLogs(logFilePath);

        }


        private void GenerateLogs(string logFilesPath)
        {
            if (!Directory.Exists(logFilesPath))
            {
                Directory.CreateDirectory(logFilesPath);
            }


            StringBuilder errorsSB = new StringBuilder();
            errorsSB.AppendLine("EntityLogicalName,Id,Action,Error");

            StringBuilder importResultsSB = new StringBuilder();
            importResultsSB.AppendLine("EntityLogicalName,StartedOn,EndedOn,CreateCount,UpdateCount,AssociateCount,DisassociateCount,SuccessCount,FailureCount,TotalProcessed");
            foreach (ImportResult importResult in _importResults)
            {
                importResultsSB.AppendLine(importResult.EntityLogicalName + "," + importResult.StartedOn + "," + importResult.EndedOn + "," + importResult.CreateCount + "," + importResult.UpdateCount + "," + importResult.AssociateCount + "," + importResult.DisassociateCount + "," + importResult.SuccessCount + "," + importResult.FailureCount + "," + importResult.TotalProcessed);


                if (importResult.Errors != null)
                {
                    foreach (string error in importResult.Errors)
                    {
                        errorsSB.AppendLine(error);
                    }
                }

            }
            File.WriteAllText(logFilesPath + @"\Results.csv", importResultsSB.ToString());
            File.WriteAllText(logFilesPath + @"\Errors.csv", errorsSB.ToString());




        }

        public void BeginImport(CrmServiceClient targetCrmService,
                                Guid configurationId,
                                USDConfiguration sourceUSDConfiguration,
                                USDConfiguration targetUsdConfiguration,
                                List<LookupMatchCriteria> lookupMatchCriterias)
        {


            ImportConfiguration(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration);

            ImportEntityTypes(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportHostedControls(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportEvents(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportScriptlets(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportEntitySearches(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportSessionLines(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportOptions(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportActions(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportActionCalls(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportSubActionCalls(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration);

            ImportEventActionCalls(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration);

            ImportToolbars(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportToolbarButtons(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportToolbarButtonActionCalls(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration);

            ImportToolbarHostedControls(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration);

            ImportWindowNavigationRules(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportWNRActionCalls(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration);

            ImportAgentScriptTasks(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportTaskActionCalls(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration);

            ImportAgentScriptAnswers(targetCrmService, configurationId, sourceUSDConfiguration, targetUsdConfiguration, lookupMatchCriterias);

            ImportTaskAnswers(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration);

            ImportAnswerActionCalls(targetCrmService, sourceUSDConfiguration, targetUsdConfiguration);
        }


        public void ImportConfiguration(CrmServiceClient crmService, USDConfiguration sourceUsdConfiguration,
                                                USDConfiguration targetUSDConfiguration)
        {

            CRMEntity sourceConfigurationCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_configuration");
            CRMEntity targetConfigurationCE = targetUSDConfiguration.GetCRMEntity("msdyusd_configuration");


            ImportResult configurationImportResult = _configurationRespository.ImportConfiguration(crmService,
                                                                                                   sourceUsdConfiguration,
                                                                                                   targetUSDConfiguration);

            _importResults.Add(configurationImportResult);


        }

        public void ImportHostedControls(CrmServiceClient crmService,
            Guid configurationId,
                                         USDConfiguration sourceUsdConfiguration,
                                         USDConfiguration targetUSDConfiguration,
                                         List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria hostedControlLookupMatchCriteria = GetLookupMatchCriteria("uii_hostedapplication", lookupMatchCriterias);

            CRMEntity sourceHostedControlsCE = sourceUsdConfiguration.GetCRMEntity("uii_hostedapplication");
            CRMEntity targetHostedControlsCE = targetUSDConfiguration.GetCRMEntity("uii_hostedapplication");


            ImportResult hostedControlsImportResult = _hostedControlRepository.ImportHostedControls(crmService,
                                                                                                    sourceHostedControlsCE,
                                                                                                    targetHostedControlsCE,
                                                                                                    hostedControlLookupMatchCriteria);

            ImportResult configurationHostedControlsImportResult = _hostedControlRepository.ImportConfigurationHostedControl(crmService,
                                                                                                                            configurationId,
                                                                                                                            sourceHostedControlsCE,
                                                                                                                            targetHostedControlsCE);
            _importResults.Add(hostedControlsImportResult);
            _importResults.Add(configurationHostedControlsImportResult);


        }


        public void ImportEntityTypes(CrmServiceClient crmService,
            USDConfiguration sourceUsdConfiguration,
            USDConfiguration targetUSDConfiguration, List<LookupMatchCriteria> lookupMatchCriterias)
        {

            LookupMatchCriteria entityTypeLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_entityassignment", lookupMatchCriterias);

            CRMEntity sourceEntityTypesCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_entityassignment");
            CRMEntity targetEntityTypesCE = targetUSDConfiguration.GetCRMEntity("msdyusd_entityassignment");




            ImportResult entityTypesImportResult = _entityTypeRepository.ImportEntityTypes(crmService,
                                                                                sourceEntityTypesCE,
                                                                                targetEntityTypesCE,
                                                                                entityTypeLookupMatchCriteria);

            _importResults.Add(entityTypesImportResult);


        }


        public void ImportEvents(CrmServiceClient crmService,
                                 Guid configurationId,
                                 USDConfiguration sourceUsdConfiguration,
                                 USDConfiguration targetUSDConfiguration,
                                 List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria eventLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_uiievent", lookupMatchCriterias);

            CRMEntity sourceEventsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_uiievent");
            CRMEntity targetEventsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_uiievent");


            ImportResult eventsImportResult = _eventRepository.ImportEvents(crmService,
                                                                            sourceEventsCE,
                                                                            targetEventsCE,
                                                                            eventLookupMatchCriteria);


            ImportResult configurationEventsImportResult = _eventRepository.ImportConfigurationEvent(crmService,
                                                                                                    configurationId,
                                                                                                    sourceEventsCE,
                                                                                                    targetEventsCE);


            _importResults.Add(eventsImportResult);
            _importResults.Add(configurationEventsImportResult);
        }


        public void ImportScriptlets(CrmServiceClient crmService,
                          Guid configurationId,
                          USDConfiguration sourceUsdConfiguration,
                          USDConfiguration targetUSDConfiguration,
                          List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria scriptletsLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_scriptlet", lookupMatchCriterias);

            CRMEntity sourceScriptletsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_scriptlet");
            CRMEntity targetScriptletsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_scriptlet");


            ImportResult scriptletsImportResult = _scriptletRepository.ImportScriptlets(crmService,
                                                                            sourceScriptletsCE,
                                                                            targetScriptletsCE,
                                                                            scriptletsLookupMatchCriteria);


            ImportResult configurationScriptletsImportResult = _scriptletRepository.ImportConfigurationScriptlet(crmService,
                                                                                                    configurationId,
                                                                                                    sourceScriptletsCE,
                                                                                                    targetScriptletsCE);


            _importResults.Add(scriptletsImportResult);
            _importResults.Add(configurationScriptletsImportResult);
        }



        public void ImportEntitySearches(CrmServiceClient crmService,
                  Guid configurationId,
                  USDConfiguration sourceUsdConfiguration,
                  USDConfiguration targetUSDConfiguration,
                  List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria entitySearchesLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_entitysearch", lookupMatchCriterias);

            CRMEntity sourceEntitySearchesCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_entitysearch");
            CRMEntity targetEntitySearchesCE = targetUSDConfiguration.GetCRMEntity("msdyusd_entitysearch");


            ImportResult entitySearchesImportResult = _entitySearchRepository.ImportEntitySearches(crmService,
                                                                            sourceEntitySearchesCE,
                                                                            targetEntitySearchesCE,
                                                                            entitySearchesLookupMatchCriteria);


            ImportResult configurationEntitySearchesImportResult = _entitySearchRepository.ImportConfigurationEntitySearches(crmService,
                                                                                                    configurationId,
                                                                                                    sourceEntitySearchesCE,
                                                                                                    targetEntitySearchesCE);


            _importResults.Add(entitySearchesImportResult);
            _importResults.Add(configurationEntitySearchesImportResult);
        }


        public void ImportSessionLines(CrmServiceClient crmService,
                Guid configurationId,
                USDConfiguration sourceUsdConfiguration,
                USDConfiguration targetUSDConfiguration,
                List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria sessionLinesLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_sessioninformation", lookupMatchCriterias);

            CRMEntity sourceSessionLinesCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_sessioninformation");
            CRMEntity targetSessionLinesCE = targetUSDConfiguration.GetCRMEntity("msdyusd_sessioninformation");


            ImportResult sessionLinesImportResult = _sessionLineRepository.ImportSessionLines(crmService,
                                                                            sourceSessionLinesCE,
                                                                            targetSessionLinesCE,
                                                                            sessionLinesLookupMatchCriteria);


            ImportResult configurationSessionLinesImportResult = _sessionLineRepository.ImportConfigurationSessionLines(crmService,
                                                                                                    configurationId,
                                                                                                    sourceSessionLinesCE,
                                                                                                    targetSessionLinesCE);


            _importResults.Add(sessionLinesImportResult);
            _importResults.Add(configurationSessionLinesImportResult);
        }



        public void ImportOptions(CrmServiceClient crmService,
              Guid configurationId,
              USDConfiguration sourceUsdConfiguration,
              USDConfiguration targetUSDConfiguration,
              List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria optionsLookupMatchCriteria = GetLookupMatchCriteria("uii_option", lookupMatchCriterias);

            CRMEntity sourceOptionsCE = sourceUsdConfiguration.GetCRMEntity("uii_option");
            CRMEntity targetOptionsCE = targetUSDConfiguration.GetCRMEntity("uii_option");


            ImportResult optionsImportResult = _optionRepository.ImportOptions(crmService,
                                                                            sourceOptionsCE,
                                                                            targetOptionsCE,
                                                                            optionsLookupMatchCriteria);


            ImportResult configurationOptionImportResult = _optionRepository.ImportConfigurationEntitySearches(crmService,
                                                                                                    configurationId,
                                                                                                    sourceOptionsCE,
                                                                                                    targetOptionsCE);


            _importResults.Add(optionsImportResult);
            _importResults.Add(configurationOptionImportResult);
        }


        public void ImportActions(CrmServiceClient crmService,
           USDConfiguration sourceUsdConfiguration,
           USDConfiguration targetUSDConfiguration, List<LookupMatchCriteria> lookupMatchCriterias)
        {

            LookupMatchCriteria actionLookupMatchCriteria = GetLookupMatchCriteria("uii_action", lookupMatchCriterias);

            CRMEntity sourceActionsCE = sourceUsdConfiguration.GetCRMEntity("uii_action");
            CRMEntity targetActionsCE = targetUSDConfiguration.GetCRMEntity("uii_action");




            ImportResult actionsImportResult = _actionRepository.ImportActions(crmService,
                                                                                sourceActionsCE,
                                                                                targetActionsCE,
                                                                                actionLookupMatchCriteria);

            _importResults.Add(actionsImportResult);


        }



        public void ImportActionCalls(CrmServiceClient crmService,
         Guid configurationId,
         USDConfiguration sourceUsdConfiguration,
         USDConfiguration targetUSDConfiguration,
         List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria actionCallsLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_agentscriptaction", lookupMatchCriterias);

            CRMEntity sourceActionCallsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_agentscriptaction");
            CRMEntity targetActionCallsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_agentscriptaction");


            ImportResult actionCallsImportResult = _actionCallrepository.ImportActionCalls(crmService,
                                                                            sourceActionCallsCE,
                                                                            targetActionCallsCE,
                                                                            actionCallsLookupMatchCriteria);


            ImportResult configurationActionCallsImportResult = _actionCallrepository.ImportConfigurationActionCalls(crmService,
                                                                                                    configurationId,
                                                                                                    sourceActionCallsCE,
                                                                                                    targetActionCallsCE);


            _importResults.Add(actionCallsImportResult);
            _importResults.Add(configurationActionCallsImportResult);
        }



        public void ImportSubActionCalls(CrmServiceClient crmService,
                                         USDConfiguration sourceUsdConfiguration,
                                         USDConfiguration targetUSDConfiguration)
        {



            CRMEntity sourceSubActionCallsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_subactioncalls");
            CRMEntity targetSubActionCallsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_subactioncalls");


            ImportResult subActionCallsImportResult = _subActionCallsRepository.ImportSubActionCalls(crmService,
                                                                            sourceSubActionCallsCE,
                                                                            targetSubActionCallsCE);




            _importResults.Add(subActionCallsImportResult);

        }


        public void ImportEventActionCalls(CrmServiceClient crmService,
                                    USDConfiguration sourceUsdConfiguration,
                                    USDConfiguration targetUSDConfiguration)
        {



            CRMEntity sourceEventActionCallsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_uiievent_agentscriptaction");
            CRMEntity targetEventActionCallsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_uiievent_agentscriptaction");


            ImportResult eventActionCallsImportResult = _eventActionCallRepository.ImportEventActionCalls(crmService,
                                                                                                        sourceEventActionCallsCE,
                                                                                                        targetEventActionCallsCE);




            _importResults.Add(eventActionCallsImportResult);

        }


        public void ImportToolbars(CrmServiceClient crmService,
                         Guid configurationId,
                         USDConfiguration sourceUsdConfiguration,
                         USDConfiguration targetUSDConfiguration,
                         List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria toolbarsLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_toolbarstrip", lookupMatchCriterias);

            CRMEntity sourceToolbarsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_toolbarstrip");
            CRMEntity targetToolbarsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_toolbarstrip");


            ImportResult toolbarsImportResult = _toolbarRepository.ImportToolbars(crmService,
                                                                            sourceToolbarsCE,
                                                                            targetToolbarsCE,
                                                                            toolbarsLookupMatchCriteria);


            ImportResult configurationToolbarsImportResult = _toolbarRepository.ImportConfigurationToolbar(crmService,
                                                                                                    configurationId,
                                                                                                    sourceToolbarsCE,
                                                                                                    targetToolbarsCE);


            _importResults.Add(toolbarsImportResult);
            _importResults.Add(configurationToolbarsImportResult);
        }



        public void ImportToolbarButtons(CrmServiceClient crmService,
                                          USDConfiguration sourceUsdConfiguration,
                                          USDConfiguration targetUSDConfiguration,
                                          List<LookupMatchCriteria> lookupMatchCriterias)
        {

            LookupMatchCriteria toolbarButtonLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_toolbarbutton", lookupMatchCriterias);

            CRMEntity sourceToolbarButtonsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_toolbarbutton");
            CRMEntity targetToolbarButtonsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_toolbarbutton");




            ImportResult toolbarButtonsImportResult = _toolbarButtonRepository.ImportToolbarButtons(crmService,
                                                                                sourceToolbarButtonsCE,
                                                                                targetToolbarButtonsCE,
                                                                                toolbarButtonLookupMatchCriteria);

            _importResults.Add(toolbarButtonsImportResult);


        }


        public void ImportToolbarButtonActionCalls(CrmServiceClient crmService,
                              USDConfiguration sourceUsdConfiguration,
                              USDConfiguration targetUSDConfiguration)
        {

            CRMEntity sourceToolbarButtonActionCallsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_toolbarbutton_agentscriptaction");
            CRMEntity targetToolbarButtonActionCallsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_toolbarbutton_agentscriptaction");


            ImportResult toolbarButtonActionCallsImportResult = _toolbarButtonActionCallRepository.ImportToolbarButtonActionCalls(crmService,
                                                                                                        sourceToolbarButtonActionCallsCE,
                                                                                                        targetToolbarButtonActionCallsCE);


            _importResults.Add(toolbarButtonActionCallsImportResult);

        }



        public void ImportToolbarHostedControls(CrmServiceClient crmService,
                      USDConfiguration sourceUsdConfiguration,
                      USDConfiguration targetUSDConfiguration)
        {

            CRMEntity sourceToolbarHostedControlsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_toolbarstrip_uii_hostedapplication");
            CRMEntity targetToolbarHostedControlsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_toolbarstrip_uii_hostedapplication");


            ImportResult toolbarHostedControlsImportResult = _toolbarHostedControlRepository.ImportToolbarHostedControls(crmService,
                                                                                                        sourceToolbarHostedControlsCE,
                                                                                                        targetToolbarHostedControlsCE);


            _importResults.Add(toolbarHostedControlsImportResult);

        }


        public void ImportWindowNavigationRules(CrmServiceClient crmService,
                                                  Guid configurationId,
                                                  USDConfiguration sourceUsdConfiguration,
                                                  USDConfiguration targetUSDConfiguration,
                                                  List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria navigationRulesLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_windowroute", lookupMatchCriterias);

            CRMEntity sourceNavigationRulesCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_windowroute");
            CRMEntity targetNavigationRulesCE = targetUSDConfiguration.GetCRMEntity("msdyusd_windowroute");


            ImportResult navigationRulesCEImportResult = _wnrRepository.ImportWindowNavigationRules(crmService,
                                                                            sourceNavigationRulesCE,
                                                                            targetNavigationRulesCE,
                                                                            navigationRulesLookupMatchCriteria);


            ImportResult configurationNavigationRulesCEImportResult = _wnrRepository.ImportConfigurationWindowNavigationRules(crmService,
                                                                                                    configurationId,
                                                                                                    sourceNavigationRulesCE,
                                                                                                    targetNavigationRulesCE);


            _importResults.Add(navigationRulesCEImportResult);
            _importResults.Add(configurationNavigationRulesCEImportResult);
        }


        public void ImportWNRActionCalls(CrmServiceClient crmService,
                                           USDConfiguration sourceUsdConfiguration,
                                           USDConfiguration targetUSDConfiguration)
        {



            CRMEntity sourceWNRActionCallsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_windowroute_agentscriptaction");
            CRMEntity targetWNRActionCallsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_windowroute_agentscriptaction");


            ImportResult wnrActionCallsImportResult = _wnrActionCallrepository.ImportWNRActionCalls(crmService,
                                                                                                        sourceWNRActionCallsCE,
                                                                                                        targetWNRActionCallsCE);




            _importResults.Add(wnrActionCallsImportResult);

        }



        public void ImportAgentScriptTasks(CrmServiceClient crmService,
                                          Guid configurationId,
                                          USDConfiguration sourceUsdConfiguration,
                                          USDConfiguration targetUSDConfiguration,
                                          List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria agentScriptTasksLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_task", lookupMatchCriterias);

            CRMEntity sourceAgentScriptTasksCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_task");
            CRMEntity targetAgentScriptTasksCE = targetUSDConfiguration.GetCRMEntity("msdyusd_task");


            ImportResult agentScriptTasksImportResult = _agentScriptTaskRepository.ImportAgentScriptTasks(crmService,
                                                                            sourceAgentScriptTasksCE,
                                                                            targetAgentScriptTasksCE,
                                                                            agentScriptTasksLookupMatchCriteria);


            ImportResult configurationAgentScriptTasksImportResult = _agentScriptTaskRepository.ImportConfigurationAgentScriptTasks(crmService,
                                                                                                    configurationId,
                                                                                                    sourceAgentScriptTasksCE,
                                                                                                    targetAgentScriptTasksCE);


            _importResults.Add(agentScriptTasksImportResult);
            _importResults.Add(configurationAgentScriptTasksImportResult);
        }


        public void ImportTaskActionCalls(CrmServiceClient crmService,
                                USDConfiguration sourceUsdConfiguration,
                                USDConfiguration targetUSDConfiguration)
        {



            CRMEntity sourceTaskActionCallsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_task_agentscriptaction");
            CRMEntity targetTaskActionCallsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_task_agentscriptaction");


            ImportResult taskActionCallsImportResult = _taskActionCallRepository.ImportTaskActionCalls(crmService,
                                                                                                        sourceTaskActionCallsCE,
                                                                                                        targetTaskActionCallsCE);




            _importResults.Add(taskActionCallsImportResult);

        }


        public void ImportAgentScriptAnswers(CrmServiceClient crmService,
                                     Guid configurationId,
                                     USDConfiguration sourceUsdConfiguration,
                                     USDConfiguration targetUSDConfiguration,
                                     List<LookupMatchCriteria> lookupMatchCriterias)
        {


            LookupMatchCriteria agentScriptAnswersLookupMatchCriteria = GetLookupMatchCriteria("msdyusd_answer", lookupMatchCriterias);

            CRMEntity sourceAgentScriptAnswersCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_answer");
            CRMEntity targetAgentScriptAnswersCE = targetUSDConfiguration.GetCRMEntity("msdyusd_answer");


            ImportResult agentScriptAnswersImportResult = _agentScriptAnswerRepository.ImportAgentScriptAnswers(crmService,
                                                                                                            sourceAgentScriptAnswersCE,
                                                                                                            targetAgentScriptAnswersCE,
                                                                                                            agentScriptAnswersLookupMatchCriteria);

            _importResults.Add(agentScriptAnswersImportResult);
        }


        public void ImportTaskAnswers(CrmServiceClient crmService,
                        USDConfiguration sourceUsdConfiguration,
                        USDConfiguration targetUSDConfiguration)
        {
            CRMEntity sourceTaskAnswersCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_task_answer");
            CRMEntity targetTaskAnswersCE = targetUSDConfiguration.GetCRMEntity("msdyusd_task_answer");


            ImportResult taskAnswersImportResult = _taskAnswerRepository.ImportTaskAnswers(crmService,
                                                                                                        sourceTaskAnswersCE,
                                                                                                        targetTaskAnswersCE);

            _importResults.Add(taskAnswersImportResult);

        }


        public void ImportAnswerActionCalls(CrmServiceClient crmService,
                        USDConfiguration sourceUsdConfiguration,
                        USDConfiguration targetUSDConfiguration)
        {



            CRMEntity sourceAnswerActionCallsCE = sourceUsdConfiguration.GetCRMEntity("msdyusd_answer_agentscriptaction");
            CRMEntity targetAnswerActionCallsCE = targetUSDConfiguration.GetCRMEntity("msdyusd_answer_agentscriptaction");


            ImportResult answerActionCallsImportResult = _answerActionCallRepository.ImportAnswerActionCalls(crmService,
                                                                                                        sourceAnswerActionCallsCE,
                                                                                                        targetAnswerActionCallsCE);




            _importResults.Add(answerActionCallsImportResult);

        }



        private LookupMatchCriteria GetLookupMatchCriteria(string entityLogicalName, List<LookupMatchCriteria> lookupMatchCriterias)
        {
            if (lookupMatchCriterias == null)
                return null;

            return lookupMatchCriterias.FirstOrDefault(x => x.EntityLogicalName == entityLogicalName);
        }
    }
}