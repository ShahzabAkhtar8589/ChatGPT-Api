using ChatGPT_Api.Model;
using Microsoft.EntityFrameworkCore;

namespace ChatGPT_Api.DBContext
{
    public class LoggingContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public LoggingContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
            }
        }

        public DbSet<ChatGPTLog> ChatGPTLogs { get; set; }

    }
}
