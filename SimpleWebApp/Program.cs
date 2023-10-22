using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SimpleWebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connString = builder.Configuration["ConnectionString"];
            DataHandler.ConnectionString = connString;
            var app = builder.Build();

            app.Map(new PathString("/data"), (Action<IApplicationBuilder>)(b => b.UseDataHandler()));

            await app.RunAsync();
        }
    }
}