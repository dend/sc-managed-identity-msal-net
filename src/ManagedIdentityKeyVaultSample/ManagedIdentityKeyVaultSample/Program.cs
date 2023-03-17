using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace ManagedIdentityKeyVaultSample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IManagedIdentityApplication managedIdApplication = ManagedIdentityApplicationBuilder.Create("/FULL/RESOURCE/ID")
                .WithExperimentalFeatures()
                .Build();

            AuthenticationResult result =
                await managedIdApplication.AcquireTokenForManagedIdentity("https://vault.azure.net/.default")
                .ExecuteAsync()
                .ConfigureAwait(false);

            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://YOUR_VAULT_ID.vault.azure.net/secrets?api-version=7.3"),
                Method = HttpMethod.Get,
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Success!");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine("Failed to get the secret list.");
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
    }
}