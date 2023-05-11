using RestSharp;

namespace AspNetIdentity.WebApi.Helper
{
    public class RestClientHelper
    {
        public static string HitTheRestClient(string baseURL)
        {
            var client = new RestClient(baseURL);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var body = @"";

            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}