namespace ChatGPT_Api.Helper
{
    public class Connection
    {
        private readonly IConfiguration _configuration;

        public Connection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString()
        {
            return _configuration["ApiKey"];
        }
    }
}
