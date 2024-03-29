using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public static class NetworkUtility
{
    private static readonly string AuthToken = $"Bearer {Environment.GetEnvironmentVariable("CONTENTFUL_ACCESS_TOKEN")}";
    private static readonly string SpaceId = $"{Environment.GetEnvironmentVariable("SPACE_ID")}";
    private static readonly string BaseUrl = $"https://graphql.contentful.com/content/v1/spaces/{SpaceId}";
    private static readonly HttpClient client = new HttpClient();

    public static async Task<T> MakeContentfulRequest<T>(string query, object variables = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl)
        {
            Content = new StringContent(JsonConvert.SerializeObject(new { query, variables }), Encoding.UTF8, "application/json")
        };

        request.Headers.Add("Authorization", AuthToken);

        var response = await client.SendAsync(request);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
        }

        // Artificial delay to miss rate limit.
        await Task.Delay(85);

        return JsonConvert.DeserializeObject<T>(responseBody);
    }
}