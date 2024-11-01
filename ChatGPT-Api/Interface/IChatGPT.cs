namespace ChatGPT_Api.Interface
{
    public interface IChatGPT
    {
        Task<string> UseChatGPT(string query);
    }
}
