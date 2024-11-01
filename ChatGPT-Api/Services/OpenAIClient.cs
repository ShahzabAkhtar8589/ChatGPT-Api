using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class OpenAIClient
{
    private readonly string _apiKey;
    private readonly string _apiUrl;

    public OpenAIClient(string apiKey)
    {
        _apiKey = apiKey;
        _apiUrl = "https://api.openai.com/v1/chat/completions"; // Adjust this URL if needed
    }

    public async Task<string> SendMessage(string userInput)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo", // Specify your model here
                messages = new[]
                {
                    new { role = "user", content = userInput }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_apiUrl, content);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(responseContent);
            return jsonResponse.choices[0].message.content.ToString();
        }
    }
}
