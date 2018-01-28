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
    public class RetrieveMultiple : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            QueryExpression query = context.InputParameterOrDefault<QueryExpression>("Query");
            var metadataHelper = new MetadataHelper(service, query.EntityName);
            
            ColumnSet cols = new ColumnSet("adr_accountname", "adr_accountpassword","adr_region", "adr_instancename", "adr_region");
            var datasource = service.Retrieve("adr_d365datasource", metadataHelper.GetDatasourceId(), cols);
            CrmServiceClient externalCrmService = ExternalD365ServiceHelper.GetServiceClient(service, metadataHelper);

            var externalResults = externalCrmService.RetrieveMultiple(Mapper.MapQuery(metadataHelper, query));
            var results = Mapper.MapExternalResults(metadataHelper, externalResults);
            context.OutputParameters["BusinessEntityCollection"] = results;
        }
    }
}
