using ChatGPT_Api.Helper;
using ChatGPT_Consumer_Api.Models;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace ChatGPT_Consumer_Api.Services.Consumer
{
    public class ConsumerService : IConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _oracleConnectionString;
        private readonly ILogger<ConsumerService> _logger;
        public ConsumerService(IConfiguration configuration, ILogger<ConsumerService> logger)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _oracleConnectionString = _configuration.GetConnectionString("OracleDb");
            _logger = logger;
        }
        //public async Task<string> RunExectible()
        //{
        //    const int maxDegreeOfParallelism = 10; // Adjust based on limits and performance testing
        //    var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

        //    var tasks = Enumerable.Range(0, 10).Select(async i =>
        //    {
        //        await semaphore.WaitAsync();
        //        try
        //        {
        //            TokenResults name = await GetTokenFromDatabase();
        //            if (name != null)
        //            {
        //                await GetTokenFromApiAsync(name);
        //                await SaveTokenToDatabase(name, i);
        //            }
        //        }
        //        finally
        //        {
        //            semaphore.Release();
        //        }
        //    });

        //    await Task.WhenAll(tasks);
        //    return "Processing Complete";
        //}

        public async Task<string> RunExectible()
        {
            for (int i = 0; i < 500; i++)
            {
                TokenResults token = await GetTokenFromDatabase();
                if (token != null)
                {
                    await GetTokenFromApiAsync(token);
                    await SaveTokenToDatabase(token, i);
                }
            }
            return "0";
        }
        private async Task<TokenResults> GetTokenFromApiAsync(TokenResults token)
        {
            string apiUrl = _configuration["ApiUrl"];

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl+"?Query="+token.Token);
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string Result = await response.Content.ReadAsStringAsync();
                token.Translation= Result.Replace("-"," ");
                return token;
            }
            else
            {
                _logger.LogError(response.StatusCode.ToString());
                return null;
            }
        }
        private async Task<TokenResults> GetTokenFromDatabase()
        {
            using (OracleConnection conn = new OracleConnection(_oracleConnectionString))
            {
                string PKG_PRC = "ALT_AKSA_PRD.PKG_OPENAPI.PRC_TOKEN_GET";

                try
                {

                    using (OracleCommand cmd = new OracleCommand(PKG_PRC, conn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        OracleParameter pUToken = new OracleParameter("pUToken", OracleDbType.Varchar2);
                        pUToken.Direction = ParameterDirection.Output;
                        pUToken.Size = 1000;
                        cmd.Parameters.Add(pUToken);

                        OracleParameter PTID = new OracleParameter("PTID", OracleDbType.Int64);
                        PTID.Direction = ParameterDirection.Output;
                        PTID.Size = 10;
                        cmd.Parameters.Add(PTID);

                        OracleParameter PCode = new OracleParameter("pCODE", OracleDbType.Varchar2);
                        PCode.Direction = ParameterDirection.Output;
                        PCode.Size = 1000;
                        cmd.Parameters.Add(PCode);

                        OracleParameter pDESC = new OracleParameter("pDESC", OracleDbType.Varchar2);
                        pDESC.Direction = ParameterDirection.Output;
                        pDESC.Size = 1000;
                        cmd.Parameters.Add(pDESC);

                        OracleParameter pMSG = new OracleParameter("pMSG", OracleDbType.Varchar2);
                        pMSG.Direction = ParameterDirection.Output;
                        pMSG.Size = 1000;
                        cmd.Parameters.Add(pMSG);

                        conn.Open();

                        cmd.ExecuteNonQuery();


                        var result = new TokenResults
                        {
                            Code = cmd.Parameters["pCODE"].Value.ToString(),
                            Description = cmd.Parameters["pDESC"].Value.ToString(),
                            Message = cmd.Parameters["pMSG"].Value.ToString(),
                            PsID = Convert.ToInt64(cmd.Parameters["PTID"].Value.ToString()),
                            Token = cmd.Parameters["pUToken"].Value.ToString()

                        };
                        //_logger.LogInformation($"Get Response: Token {result.Token}, Code: {result.Code}, Description: {result.Description}, Message: {result.Message}");
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return null;
                }


            }
        }

        private async Task SaveTokenToDatabase(TokenResults token, int i)
        {
            using (OracleConnection conn = new OracleConnection(_oracleConnectionString))
            {
                string PKG_PRC = "ALT_AKSA_PRD.PKG_OPENAPI.PRC_TOKEN_UPDATE";

                try
                {

                    using (OracleCommand cmd = new OracleCommand(PKG_PRC, conn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        cmd.Parameters.Add("PETOKEN", OracleDbType.Varchar2, ParameterDirection.Input).Value = token.Translation;
                        cmd.Parameters.Add("pTID", OracleDbType.Int32, ParameterDirection.Input).Value = token.PsID;

                        OracleParameter PCode = new OracleParameter("pCODE", OracleDbType.Varchar2);
                        PCode.Direction = ParameterDirection.Output;
                        PCode.Size = 1000;
                        cmd.Parameters.Add(PCode);

                        OracleParameter pDESC = new OracleParameter("pDESC", OracleDbType.Varchar2);
                        pDESC.Direction = ParameterDirection.Output;
                        pDESC.Size = 1000;
                        cmd.Parameters.Add(pDESC);

                        OracleParameter pMSG = new OracleParameter("pMSG", OracleDbType.Varchar2);
                        pMSG.Direction = ParameterDirection.Output;
                        pMSG.Size = 1000;
                        cmd.Parameters.Add(pMSG);

                        conn.Open();

                        cmd.ExecuteNonQuery();
                        var result = new TokenResults
                        {
                            Code = cmd.Parameters["pCODE"].Value.ToString(),
                            Description = cmd.Parameters["pDESC"].Value.ToString(),
                            Message = cmd.Parameters["pMSG"].Value.ToString()

                        };
                        _logger.LogInformation($"Sr No. {i} : Urdu - {token.Token}, Translation - {token.Translation}");
                        //_logger.LogInformation($"Save Response : Token {token.Token},Translation {token.Translation}, Code: {result.Code}, Description: {result.Description}, Message: {result.Message}");

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }


            }
        }
    }
}
