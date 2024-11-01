using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;

public class OpenAIConversation
{
    private readonly OpenAIAPI _api;
    private readonly List<ChatMessage> _messages;

    public OpenAIConversation(string apiKey, string initialContext)
    {
        _api = new OpenAIAPI(apiKey);
        _messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, initialContext)
        };
    }

    public async Task<string> StartConversation()
    {
        var chatRequest = new ChatRequest
        {
            Model = OpenAI_API.Models.Model.ChatGPTTurbo,
            Messages = _messages
        };

        var result = await _api.Chat.CreateChatCompletionAsync(chatRequest);

        _messages.Add(result.Choices[0].Message);
        return result.Choices[0].Message.Content;
    }

    public async Task<string> ContinueConversation(string userInput)
    {
        
        _messages.Add(new ChatMessage(ChatMessageRole.User, userInput));

        var chatRequest = new ChatRequest
        {
            Model = OpenAI_API.Models.Model.ChatGPTTurbo,
            Messages = _messages
        };

        var result = await _api.Chat.CreateChatCompletionAsync(chatRequest);

        _messages.Add(result.Choices[0].Message);
        return result.Choices[0].Message.Content;
    }
}
