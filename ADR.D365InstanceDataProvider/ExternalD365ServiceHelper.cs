using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace ADR.D365InstanceDataProvider
{
    public class ExternalD365ServiceHelper
    {
        public static OrganizationWebProxyClient GetOrgWebProxyClient(IOrganizationService service, MetadataHelper metadataHelper)
        {
            ColumnSet cols = new ColumnSet("adr_orgserviceurl", "adr_clientId", "adr_secret", "adr_resourceId", "adr_tenantId");
            var datasource = service.Retrieve("adr_d365datasource", metadataHelper.GetDatasourceId(), cols);
            var uri = new Uri(datasource["adr_clientId"].ToString() + "/web?SdkClientVersion=9.0.0.0");

            Task<AzureAccessToken> token = CreateOAuthAuthorizationToken(
                datasource["adr_clientId"].ToString(), 
                datasource["adr_secret"].ToString(), 
                datasource["adr_resourceId"].ToString(), 
                datasource["adr_tenantId"].ToString());

            var proxy = new OrganizationWebProxyClient(uri, false);
            proxy.HeaderToken = token.Result.access_token;

            return proxy;
        }

        public async static Task<AzureAccessToken> CreateOAuthAuthorizationToken(string clientId, string clientSecret, string resourceId, string tenantId)
        {
            AzureAccessToken token = null;
            string oauthUrl = string.Format("https://login.microsoftonline.com/{0}/oauth2/token", tenantId);
            string reqBody = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&resource={2}", Uri.EscapeDataString(clientId), Uri.EscapeDataString(clientSecret), Uri.EscapeDataString(resourceId));

            HttpClient client = new HttpClient();
            HttpContent content = new StringContent(reqBody);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            using (HttpResponseMessage response = await client.PostAsync(oauthUrl, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AzureAccessToken));
                    Stream json = await response.Content.ReadAsStreamAsync();
                    token = (AzureAccessToken)serializer.ReadObject(json);
                }
            }
            return token;
        }
    }

    [DataContract]
    public class AzureAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string expires_on { get; set; }
        [DataMember]
        public string resource { get; set; }
    }
}
