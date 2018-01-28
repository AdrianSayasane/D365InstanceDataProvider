using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.D365InstanceDataProvider
{
    public class MetadataHelper
    {
        private EntityMetadata EntityMetadata { get; set; }

        public MetadataHelper(IOrganizationService service, string entityLogicalName)
        {
            var req = new RetrieveEntityRequest()
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = entityLogicalName,
                RetrieveAsIfPublished = true
            };

            RetrieveEntityResponse res = (RetrieveEntityResponse)service.Execute(req);
            EntityMetadata = res.EntityMetadata;
        }

        public string GetEntityName() => EntityMetadata.LogicalName;

        public string GetAttributeName(string externalAttributeName) => EntityMetadata.Attributes.FirstOrDefault(x => x.ExternalName == externalAttributeName).LogicalName;

        public Guid GetDatasourceId() => EntityMetadata.DataSourceId.Value;

        public string GetExternalEntityName() => EntityMetadata.ExternalName;

        public string GetExternalAttributeName(string attributeName) => EntityMetadata.Attributes.First(x => x.LogicalName == attributeName).ExternalName;
    }
}
