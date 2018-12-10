using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System.Collections.Generic;
using USDConfigurationMigration.Models;
using USDConfigurationMigration.Helpers;
using USDConfigurationMigration.Repositories;

namespace USDConfigurationMigration
{
    public class ExportDataService
    {
        EntityTypeRepository _entityTypeRepository;
        EventRepository _eventrepository;
        ScriptletRepository _scriptletRepository;
        EntitySearchRepository _entitySearchRepository;
        ConfigurationRepository _configurationRepository;
        HostedControlRepository _hostedControlRepository;
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
        List<CRMEntity> _crmEntities;

        public ExportDataService()
        {
            _crmEntities = new List<CRMEntity>();
            _eventrepository = new EventRepository();
            _entityTypeRepository = new EntityTypeRepository();
            _configurationRepository = new ConfigurationRepository();
            _hostedControlRepository = new HostedControlRepository();
            _scriptletRepository = new ScriptletRepository();
            _entitySearchRepository = new EntitySearchRepository();
            _sessionLineRepository = new SessionLineRepository();
            _optionRepository = new OptionRepository();
            _actionRepository = new ActionRepository();
            _actionCallrepository = new ActionCallRepository();
            _toolbarRepository = new ToolbarRepository();
            _subActionCallsRepository = new SubActionCallsRepository();
            _eventActionCallRepository = new EventActionCallRepository();
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




        public void ExportData(CrmServiceClient crmServiceClient, string configurationName, string filePath)
        {
            USDConfiguration usdConfiguration = GetUSDConfiguration(crmServiceClient, configurationName);

            string xmlData = usdConfiguration.XmlSerialize();

            FileHelper.CreateXmlDocument(filePath + @"\data.xml", xmlData);

        }


        public USDConfiguration GetUSDConfiguration(CrmServiceClient crmService, string configurationName)
        {

            CRMEntity configurationCE = _configurationRepository.GetConfigurationEntity(crmService, configurationName);

            if (configurationCE == null)
                return null;

            AddToCollection(configurationCE);

            EntityReference configurationER = configurationCE.CRMRecords[0].ToEntityReference();



            CRMEntity hostedControlsCE = _hostedControlRepository.GetHostedControls(crmService, configurationER);
            AddToCollection(hostedControlsCE);

            CRMEntity entityTypesCE = _entityTypeRepository.GetEntityTypes(crmService);
            AddToCollection(entityTypesCE);

            CRMEntity eventsCE = _eventrepository.GetEvents(crmService, configurationER);
            AddToCollection(eventsCE);


            CRMEntity scriptletsCE = _scriptletRepository.GetScriptlets(crmService, configurationER);
            AddToCollection(scriptletsCE);


            CRMEntity entitySearchesCE = _entitySearchRepository.GetEntitySearches(crmService, configurationER);
            AddToCollection(entitySearchesCE);

            CRMEntity sessionLinesCE = _sessionLineRepository.GetSessionLines(crmService, configurationER);
            AddToCollection(sessionLinesCE);

            CRMEntity optionCE = _optionRepository.GetOptions(crmService, configurationER);
            AddToCollection(optionCE);


            CRMEntity actionCE = _actionRepository.GetActions(crmService, configurationER);
            AddToCollection(actionCE);

            CRMEntity actionCallCE = _actionCallrepository.GetActionCalls(crmService, configurationER);
            AddToCollection(actionCallCE);

            CRMEntity subActionCallsCE = _subActionCallsRepository.GetSubActionCalls(crmService, actionCallCE);
            AddToCollection(subActionCallsCE);

            CRMEntity eventActionCallsCE = _eventActionCallRepository.GetEventActionCalls(crmService, eventsCE, actionCallCE);
            AddToCollection(eventActionCallsCE);

            CRMEntity toolbarsCE = _toolbarRepository.GetToolbars(crmService, configurationER);
            AddToCollection(toolbarsCE);

            CRMEntity toolbarButtonsCE = _toolbarButtonRepository.GetToolbarButtons(crmService, configurationER);
            AddToCollection(toolbarButtonsCE);

            CRMEntity toolbarButtonActionCallsCE = _toolbarButtonActionCallRepository.GetToolbarButtonActionCalls(crmService, toolbarButtonsCE, actionCallCE);
            AddToCollection(toolbarButtonActionCallsCE);


            CRMEntity toolbarHostedControlsCE = _toolbarHostedControlRepository.GetToolbarHostedControls(crmService, toolbarsCE, hostedControlsCE);
            AddToCollection(toolbarHostedControlsCE);


            CRMEntity windowNavigationRulesCE = _wnrRepository.GetWindowNavigationRules(crmService, configurationER);
            AddToCollection(windowNavigationRulesCE);


            CRMEntity navigationRulesActionCallsCE = _wnrActionCallrepository.GetWNRActionCalls(crmService, windowNavigationRulesCE, actionCallCE);
            AddToCollection(navigationRulesActionCallsCE);


            CRMEntity agentScriptTasksCE = _agentScriptTaskRepository.GetAgentScriptTasks(crmService, configurationER);
            AddToCollection(agentScriptTasksCE);

            CRMEntity taskActionCallsCE = _taskActionCallRepository.GetTaskActionCalls(crmService, agentScriptTasksCE, actionCallCE);
            AddToCollection(taskActionCallsCE);

            CRMEntity taskAnswersCE = _taskAnswerRepository.GetTaskAnswers(crmService, agentScriptTasksCE);
            AddToCollection(taskAnswersCE);


            CRMEntity agentScriptAnswersCE = _agentScriptAnswerRepository.GetAgentScriptAnswers(crmService, taskAnswersCE);
            AddToCollection(agentScriptAnswersCE);

            CRMEntity answerActionCallsCE = _answerActionCallRepository.GetAnswerActionCalls(crmService, agentScriptAnswersCE, actionCallCE);
            AddToCollection(answerActionCallsCE);

            USDConfiguration usdConfiguration = new USDConfiguration { CRMEntities = _crmEntities, Name = configurationName };
            return usdConfiguration;
        }


        private void AddToCollection(CRMEntity crmEntity)
        {

            if (crmEntity != null)
            {
                _crmEntities.Add(crmEntity);
            }
        }

        //Gets both the Hosted Controls and the Configuration Hosted Controls for a Specified Configuration
        // public List<CRMEntity> GetHostedControlData()

    }
}
