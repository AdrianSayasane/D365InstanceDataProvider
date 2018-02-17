using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Query;
using System;

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
            IOrganizationService externalCrmService = ExternalD365ServiceHelper.GetOrgWebProxyClient(service, metadataHelper.GetDatasourceId());

            var externalResults = externalCrmService.RetrieveMultiple(Mapper.MapQuery(metadataHelper, query));
            var results = Mapper.MapExternalResults(metadataHelper, externalResults);
            context.OutputParameters["BusinessEntityCollection"] = results;
        }
    }
}
