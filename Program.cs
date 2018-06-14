using System;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

using Microsoft.Azure.Services.AppAuthentication;
namespace ADAL_KEYVAULT_EVENTHUBS_ConsoleApp
{
    class Program
    {

        public static string ClientID = "REPLACE THIS WITH YOUR CLIENT/APP ID HERE";
        public static string ClientSecret = "REPLACE THIS WITH YOUR CLIENT/APP PASSWORD";
        public static string SecretName = "REPLACE THIS WITH YOUR SECRET NAME IN KEY VAULT";
        public static string AzureVaultURI = "REPLACE THIS WITH YOUR KEY VAULT URI";
        public const string EhEntityPath = "REPLACE THIS WITH EVENT HUBS ENTITY NAME";


        private static EventHubClient eventHubClient;
        public static string EhConnectionString = "";
        


        //use Active Directory Authentication Library(ADAL) to authenticate and access KeyVault
        //private async Task authUsingADALCallbackAsync(string AzureVaultURI)
        private static async void authUsingADALCallbackAsync()
        {
            Console.WriteLine("Authenticating to Key Vault using ADAL callback.");
            Console.WriteLine(AzureVaultURI);

            // Set up a KV Client with an ADAL authentication callback function
            KeyVaultClient kvClient = new KeyVaultClient(
                async (string authority, string resource, string scope) =>
                {
                    var authContext = new AuthenticationContext(authority);
                    ClientCredential clientCred = new ClientCredential(ClientID, ClientSecret);
                    AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);
                    if (result == null)
                    {
                        throw new InvalidOperationException("Failed to retrieve access token for Key Vault, perhaps you DO NOT have authorization to access.  ");

                    }
                    Console.WriteLine("I got the token!");
                    return result.AccessToken;
                }
            );

            // Set and get an example secret            
            SecretBundle s = await kvClient.GetSecretAsync(AzureVaultURI, SecretName);
            EhConnectionString = s.Value;            
            Console.WriteLine("Retrieved secret data: " + EhConnectionString + "\"");


        }


        static void Main(string[] args)
        {

            authUsingADALCallbackAsync();
            Console.WriteLine(AzureVaultURI);
            // Get a secret                      


            while (EhConnectionString == "")
            {
                Console.WriteLine("Waiting for Authentication and ConnectionString....");

            }



            // continue Event Hub routine 
            MainAsync(args).GetAwaiter().GetResult();
            Console.WriteLine("Finished and exit.");
            //Console.ReadLine();

        }
        private static async Task MainAsync(string[] args)
        //private async Task MainAsync(string[] args)
        {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but this simple scenario
            // uses the connection string from the namespace.
            string ConnString = EhConnectionString;
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(ConnString)
            {
                EntityPath = EhEntityPath
            };

            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendMessagesToEventHub(100);

            await eventHubClient.CloseAsync();


        }

        //Random string functions
        private static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();

        }
        // Creates an event hub client and sends 100 messages to the event hub.
        private static async Task SendMessagesToEventHub(int numMessagesToSend)
        {
            Random rand = new Random();
            for (var i = 0; i < numMessagesToSend; i++)
            {
                try
                {

                    var telemetryDataPoint = new
                    {
                        deviceId = RandomString(3),
                        menuName = RandomString(1),
                        Message = RandomString(2),
                        Severity = rand.Next(1, 4)
                    };
                    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    //var message = Encoding.ASCII.GetBytes(messageString);
                    var message = Encoding.UTF8.GetBytes(messageString);
                    Console.WriteLine($"Sending message #: {i}");
                    await eventHubClient.SendAsync(new EventData(message));
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(10);
            }

            Console.WriteLine($"{numMessagesToSend} messages sent.");
        }
    }
}
