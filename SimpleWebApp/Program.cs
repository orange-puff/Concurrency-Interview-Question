using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace SimpleWebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connString = builder.Configuration["ConnectionString"];
            builder.Services.AddScoped<MySqlConnection>(_ => new MySqlConnection(connString));
            var app = builder.Build();

            app.Map(new PathString("/data"), (Action<IApplicationBuilder>)(b => b.UseDataHandler()));

            await app.RunAsync();
        }
    }
}