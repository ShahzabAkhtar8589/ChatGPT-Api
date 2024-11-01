using Microsoft.AspNetCore.Mvc;
using OpenAI_API.Completions;
using OpenAI_API;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.Mime.MediaTypeNames;
using ChatGPT_Api.Interface;
using Google.Protobuf.WellKnownTypes;
using Google.Cloud.AIPlatform.V1;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
namespace ChatGPT_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenAIController : ControllerBase
    {
        public readonly IChatGPT _IChatGpt;
        public OpenAIController(IChatGPT _iChatGpt)
        {
            _IChatGpt=_iChatGpt;
        }

        [HttpGet]
        [Route("UseChatGPT")]
        public async Task<IActionResult> UseChatGPT(string query)
        {
            var results =await _IChatGpt.UseChatGPT(query);
            return Ok(Regex.Replace(results, @"(@|&|'|\(|\)|<|>|#)", ""));



        }
    }
}
