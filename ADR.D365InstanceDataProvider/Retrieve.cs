using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Data.Exceptions;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            CrmServiceClient externalCrmService = ExternalD365ServiceHelper.GetServiceClient(service, metadataHelper);
            
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
