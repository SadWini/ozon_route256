using Microsoft.AspNetCore.Server.Kestrel.Core;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(op=>{
                    op.ListenLocalhost(5115, o=> o.Protocols = HttpProtocols.Http1);
                    op.ListenLocalhost(5001, o=> o.Protocols = HttpProtocols.Http2);
                });
                webBuilder.UseStartup<Startup>();
            }).Build().RunAsync();
    }
}
