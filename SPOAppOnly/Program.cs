using System;
using Microsoft.Identity.Client;
using System.Configuration;
using System.Net.Http.Headers;
using Microsoft.Graph;

namespace SPOAppOnly
{
    class Program
    {
       
        static void Main(string[] args)
        {
            
            GetListData();
            Console.ReadKey();
        }


        private static async void GetListData()
        {
            var tenantId = ConfigurationManager.AppSettings["TenantId"];
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var clientSecret = ConfigurationManager.AppSettings["ClientSecret"]; // Or some other secure place.
            var scopes = new string[] { "https://graph.microsoft.com/.default" };

            // Configure the MSAL client as a confidential client
            var confidentialClient = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority("https://login.microsoftonline.com/"+ tenantId + "/v2.0")
                .WithClientSecret(clientSecret)
                .Build();

            // Create the Microsoft Graph client. 
            GraphServiceClient graphServiceClient =
                new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) => {
            // Retrieve an access token for Microsoft Graph.
                var authResult = await confidentialClient
                    .AcquireTokenForClient(scopes)
                    .ExecuteAsync();

                    // Add the access token in the Authorization header of the API request.
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                        })
                );

            // Make a Microsoft Graph API query
            var listitems = await graphServiceClient.Sites["ibmdev.sharepoint.com"].Lists["TestList"].Items.Request().GetAsync();
            
        }
        


    }
}
