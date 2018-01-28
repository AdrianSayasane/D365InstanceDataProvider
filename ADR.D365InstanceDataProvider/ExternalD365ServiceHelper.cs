using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.D365InstanceDataProvider
{
    public class ExternalD365ServiceHelper
    {
        public static CrmServiceClient GetServiceClient(IOrganizationService service, MetadataHelper metadataHelper)
        {
            ColumnSet cols = new ColumnSet("adr_accountname", "adr_accountpassword", "adr_region", "adr_instancename", "adr_region");
            var datasource = service.Retrieve("adr_d365datasource", metadataHelper.GetDatasourceId(), cols);
            CrmServiceClient externalCrmService = new CrmServiceClient(
                datasource["adr_accountname"].ToString(),
                CrmServiceClient.MakeSecureString(datasource["adr_accountpassword"].ToString()),
                datasource["adr_region"].ToString(),
                datasource["adr_instancename"].ToString(),
                false,
                false,
                null,
                true);

            return externalCrmService;
        }
    }
}
