using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    var client = new HttpClient();
        //    Task.Run(async () =>
        //    {
        //        var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
        //        if (disco.IsError)
        //        {
        //            Console.WriteLine(disco.Error);
        //            return;
        //        }
        //    }).GetAwaiter().GetResult();
        //    Console.ReadKey();
        //}

        static async Task Main(string[] args)
        {
            // discovery endpoint
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request accsess token 
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "console client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "api1"
            });
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            // call Identity Resource API
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var response = await apiClient.GetAsync("http://localhost:5001/Identity"); // api1中的api地址
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(JsonConvert.SerializeObject(response) );
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            Console.ReadKey();
        }
    }
}
