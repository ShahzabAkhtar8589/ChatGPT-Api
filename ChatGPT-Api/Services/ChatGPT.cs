using ChatGPT_Api.DBContext;
using ChatGPT_Api.Interface;
using ChatGPT_Api.Model;
using Google.Cloud.Translation.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using static Google.Rpc.Context.AttributeContext.Types;

namespace ChatGPT_Api.Services
{
    public class ChatGPT : IChatGPT
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly IConfiguration Configuration;
        private readonly LoggingContext _context;

        public ChatGPT(IConfiguration configuration, LoggingContext context)
        {
            Configuration = configuration;
            _context = context;
        }

        public async Task<string> UseChatGPT(string query)
        {
            var requestBody = new
            {
                model = "gpt-4o-2024-08-06", //gpt-4o-mini
                messages = new[]
        {
            new
            {
                role = "system",
                content = new[]
                {
                    new
                    {
                        type = "text",
                        text = "Transliterate the Urdu name to English in plain text. Exclude macron and diacritic marks."
                    }
                }
            },
            new
            {
                role = "user",
                content = new[]
                {
                    new
                    {
                        type = "text",
                        text = query
                    }
                }
            }
        },
                temperature = 0.4,
                frequency_penalty = 0,
                presence_penalty = 0

            };
            var apiKey = Configuration["ApiKey"];
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    ChatCompletion responseObject = JsonConvert.DeserializeObject<ChatCompletion>(responseString);
                    //await LogChatInteractionAsync(JsonConvert.SerializeObject(requestBody), responseString.ToString());


                    return responseObject.Choices[0].Message.Content;
                }
            }
            catch (Exception ex)
            {
                await LogChatInteractionAsync(JsonConvert.SerializeObject(requestBody), "No Response Recorded", ex);
                return "Error: " + ex.Message;
            }
            //  return "";

        }
        public async Task LogChatInteractionAsync(string request, string response, Exception ex = null)
        {
            try
            {
                var log = new ChatGPTLog
                {
                    Timestamp = DateTime.UtcNow,
                    Request = request,
                    Response = response,
                    Message = ex?.Message,
                    StackTrace = ex?.StackTrace
                };

                _context.ChatGPTLogs.Add(log);
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

    }
}
