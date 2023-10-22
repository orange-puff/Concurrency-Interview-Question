using System.Collections.Concurrent;
using System.Data;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;

namespace SimpleWebApp
{
    public class DataHandler
    {
        public static string ConnectionString;

        private const string DataNameQueryKey = "dataName";
        private const string ProcedureName = "MyAppDatabase.GetOrCreateDataId";
        private readonly ConcurrentDictionary<string, int> _cache;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public DataHandler(RequestDelegate next)
        {
            _cache = new ConcurrentDictionary<string, int>();
        }

        public async Task Invoke(HttpContext context)
        {
            // Check for valid request
            var dataName = context.Request.Query[DataNameQueryKey].ToString();
            if (dataName == string.Empty)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.Body.WriteAsync(
                    Encoding.ASCII.GetBytes("Pass data in with ?dataName=<your data>"));
                return;
            }

            // Check for dataId in in-memory cache
            if (_cache.TryGetValue(dataName, out var dataId))
            {
                await WriteResponse(context, dataName, dataId);
                return;
            }

            // If not found in cache, do DB lookup
            dataId = await GetDataId(dataName);

            // Update cache and write response
            _cache[dataName] = dataId;
            await WriteResponse(context, dataName, dataId);
        }

        private static async Task<int> GetDataId(string dataName)
        {
            int dataId;
            await using (var connection = new MySqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                await using (var cmd = new MySqlCommand(ProcedureName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dataName", dataName);
                    cmd.Parameters.Add(new MySqlParameter("@resultId", MySqlDbType.Int32));
                    cmd.Parameters["@resultId"].Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();
                    dataId = int.Parse(cmd.Parameters["@resultId"].Value.ToString() ?? "-1");
                }
            }

            return dataId;
        }

        private static async Task WriteResponse(HttpContext context, string dataName, int dataId)
        {
            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync(
                Encoding.ASCII.GetBytes($"DataName: {dataName}, DataId: {dataId}"));
        }
    }

    public static class DataHandlerExtensions
    {
        public static IApplicationBuilder UseDataHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DataHandler>();
        }
    }
}