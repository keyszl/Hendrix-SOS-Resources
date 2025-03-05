using Azure.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.Graph;
using System.Net.Http;
using System.Drawing.Printing;
using Microsoft.Graph.Models;

namespace SOS_Resources.Pages.Shared;

public class LayoutModel : PageModel
{
    private readonly ILogger<LayoutModel> _logger;

    public LayoutModel(ILogger<LayoutModel> logger)
    {
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        Console.WriteLine("on get running");
        /*// API check to Microsoft entra to see what group the user is a part of
        var client = new HttpClient();
        string apiUrl = "https://graph.microsoft.com/v1.0/groups";
        var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode) {
            string responseContent = await response.Content.ReadAsStringAsync();
            var apiData = JsonConvert.DeserializeObject<YourDataClass>(responseContent);
        }*/

        var scopes = new[] { "User.Read" };

        // Multi-tenant apps can use "common",
        // single-tenant apps must use the tenant ID from the Azure portal
        var tenantId = "0cf6c18c-d0d2-4a3f-83d0-663d620a63d3";

        // Value from app registration
        var clientId = "6c003b0d-e4a0-4db7-ab95-ef0785aa43de";

        // using Azure.Identity;
        var options = new DeviceCodeCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
            ClientId = clientId,
            TenantId = tenantId,
            // Callback function that receives the user prompt
            // Prompt contains the generated device code that user must
            // enter during the auth process in the browser
            DeviceCodeCallback = (code, cancellation) =>
            {
                Console.WriteLine(code.Message);
                return Task.FromResult(0);
            },
        };

        // https://learn.microsoft.com/dotnet/api/azure.identity.devicecodecredential
        var deviceCodeCredential = new DeviceCodeCredential(options);

        var graphClient = new GraphServiceClient(deviceCodeCredential, scopes);
        var result = await graphClient.Groups["{group-id}"].Members.GetAsync();

        Console.Write(result);
        Console.WriteLine("test");


    }
}
