using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatGPT_Api.Model
{
    [Table("ChatGPTLog")]
    public class ChatGPTLog
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
