using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pratice___AF.Models;
using Npgsql;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;

namespace Pratice_AF_Core
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("InsertData")]
        public static async Task<IActionResult> Insert([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var data = JsonConvert.DeserializeObject<InputModel>(requestBody);

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(Environment.GetEnvironmentVariable("PostgreyDB"))) //Getting connection string from local.settings.json
                {
                    await con.OpenAsync();

                    if (data != null)
                    {
                        var query = $"INSERT INTO employee VALUES({data.Id},'{data.UserName}','{data.Email}',{data.PhoneNumber})";

                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, con);
                        await npgsqlCommand.ExecuteNonQueryAsync();
                    }

                    await con.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            return new OkResult();
        }

        [FunctionName("UpdateData")]
        public static async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var data = JsonConvert.DeserializeObject<InputModel>(requestBody);

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(Environment.GetEnvironmentVariable("PostgreyDB")))
                {
                    await con.OpenAsync();

                    if (data != null)
                    {
                        var query = $"UPDATE employee SET empname = '{data.UserName}',emailid = '{data.Email}', phonenumber='{data.PhoneNumber}'  WHERE ID={data.Id}";

                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, con);
                        await npgsqlCommand.ExecuteNonQueryAsync();
                    }

                    await con.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            return new OkResult();
        }

        [FunctionName("GetDataById")]
        public static async Task<IActionResult> GetDataById([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            DataTable dt = new DataTable();

            string data = string.Empty;

            int? id = Convert.ToInt32(req.Query["id"]);

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(Environment.GetEnvironmentVariable("PostgreyDB")))
                {
                    await con.OpenAsync();

                    if (id != null)
                    {
                        var query = $"select * from Employee WHERE ID={id}";

                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, con);
                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(npgsqlCommand);
                        da.Fill(dt);

                        data = JsonConvert.SerializeObject(dt).ToString();
                    }

                    await con.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            return new OkObjectResult(data);

        }

        [FunctionName("DeleteDataByID")]
        public static async Task<IActionResult> DeleteDatatByID([HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)] HttpRequest req)
        {
            int? id = Convert.ToInt32(req.Query["id"]);

            string data = string.Empty;

            try
            {
                using (NpgsqlConnection con = new NpgsqlConnection(Environment.GetEnvironmentVariable("PostgreyDB")))
                {
                    await con.OpenAsync();

                    if (id != null)
                    {
                        var query = $"delete from Employee WHERE ID={id}";

                        NpgsqlCommand npgsqlCommand = new NpgsqlCommand(query, con);

                        data = "Deleted Sucessfully!";
                    }

                    await con.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

            return new OkObjectResult(data);

        }
    }
}
