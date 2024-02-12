using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BlazorApp2.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAmazonCognitoIdentityProvider _cognitoProvider;
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        public AuthenticationService(HttpClient httpClient, IConfiguration configuration)
        {
            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("7oG#0U$dgQcREwT7", "bzlrO8suw1Zrp9P7egsZUf3u+oDpSw8HcfdvkXEs");
            var awsRegion = RegionEndpoint.GetBySystemName("eu-west-3");
            _cognitoProvider = new AmazonCognitoIdentityProviderClient(awsCredentials, awsRegion);
            this.httpClient = httpClient;
            this.configuration = configuration;
        }

        public async Task<string> RefreshToken(string idToken)
        {
            var request = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.CUSTOM_AUTH,
                ClientId = "6f4m403a7ocbfa16275rpqn7d4",
                AuthParameters = new Dictionary<string, string>
            {
                { "TOKEN", idToken }
            }
            };

            var response = await _cognitoProvider.InitiateAuthAsync(request);
            return response.AuthenticationResult.IdToken;


        }

        public async Task<string> SignInAsync(string username, string password)
        {
            var request = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = "6f4m403a7ocbfa16275rpqn7d4",
                AuthParameters = new Dictionary<string, string>
            {
                { "USERNAME", username },
                { "PASSWORD", password }
            }
            };

            var response = await _cognitoProvider.InitiateAuthAsync(request);
            return response.AuthenticationResult.IdToken;


        }

        public async Task<string> GetIdToken(string code)
        {
            try
            {
                string apiUrl = "https://ttkapidomain.auth.eu-west-3.amazoncognito.com/oauth2/token";

                var parameters = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "client_id", "6f4m403a7ocbfa16275rpqn7d4" },
                    { "code", code },
                    { "redirect_uri", "https://blazorapp2.teletech-int.info/weather" }
                };

                // Créer le contenu form-url-encoded
                var content = new FormUrlEncodedContent(parameters);

                // Spécifier le Content-Type
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                // Envoyer la requête POST
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
                    return tokenResponse.id_token;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }

    public class TokenResponse
    {
        public string id_token { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
