using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace ADR.D365InstanceDataProvider
{

    public class Retrieve : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public Retrieve(string unsecureConfig, string secureConfig)
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }
        #endregion

        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            EntityReference target = (EntityReference)context.InputParameters["Target"];
            ColumnSet entityCols = (ColumnSet)context.InputParameters["ColumnSet"];
            var metadataHelper = new MetadataHelper(service, target.LogicalName);
            IOrganizationService externalCrmService = ExternalD365ServiceHelper.GetOrgWebProxyClient(service, metadataHelper);
            
            if(entityCols != null)
            {
                entityCols = Mapper.MapColumnSet(metadataHelper, entityCols);
            }

            var externalResult = externalCrmService.Retrieve(metadataHelper.GetExternalEntityName(), target.Id, entityCols);
            var result = Mapper.MapExternalResult(metadataHelper, externalResult);
            context.OutputParameters["BusinessEntity"] = result;
        }
    }
}
