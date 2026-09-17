using HotChocolate.Execution;

namespace HealthTracker.Api.GraphQl;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(delegate (IWebHostBuilder webBuilder)
                {
                    webBuilder.UseStartup<Startup>();
                })
            .Build();
            var exportSchemaIndex = System.Array.FindIndex(args, a => a.Contains("--export-schema"));
            if (exportSchemaIndex >= 0)
            {
                var arg = args[exportSchemaIndex];
                var outputPath = "../schema.graphql";
                if (arg.Contains(" "))
                {
                    var parts = arg.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1) { outputPath = parts[1].Trim((char)34); }
                }
                else if (exportSchemaIndex + 1 < args.Length && !args[exportSchemaIndex + 1].StartsWith("-"))
                {
                    outputPath = args[exportSchemaIndex + 1];
                }
                using var scope = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.CreateScope(host.Services);
                var executor = scope.ServiceProvider.GetRequestExecutorAsync().Result;
                System.IO.File.WriteAllText(outputPath, executor.Schema.ToString());
                return;
            }
            await host.RunWithGraphQLCommandsAsync(args);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
