using System.Diagnostics;

namespace SimpleWebAppThrasher
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            var dataNames = new List<string> { Guid.NewGuid().ToString() };
            var n = 10;
            await Parallel.ForEachAsync(dataNames, async (dataName, _) =>
            {
                await Parallel.ForEachAsync(Enumerable.Repeat(1, n).ToArray(), async (i, _) =>
                {
                    var httpClient = new HttpClient();
                    await httpClient.GetAsync($"http://localhost:8080/data?dataName={dataName}");
                });
            });
            
            Console.WriteLine($"Writing {dataNames[0]} {n} times took {sw.ElapsedMilliseconds}ms");
        }
    }
}