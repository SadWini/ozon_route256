using DefaultSite.GrpcServices;
using FluentValidation;
using Infrastructure;
using Grpc.AspNetCore.Server;
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c=>{
            c.EnableAnnotations();
        });

        services.AddGrpc(op=>{
            op.Interceptors.Add<LoggerInterceptor>();
            op.Interceptors.Add<ExceptionInterceptor>();
        });

        services.AddGrpcReflection();

        services.AddSingleton<IGoodsRepository, GoodsRepository>();
        services.AddScoped<GoodsServiceGrpc>();
        services.AddScoped<CurrentMiddleware>();

        services.AddValidatorsFromAssemblyContaining<ItemValidator>();
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseMiddleware<LoggerMiddleware>();
        app.UseMiddleware<CurrentMiddleware>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseEndpoints(endpoints =>
        {
           endpoints.MapControllers();
           endpoints.MapGrpcService<GoodsServiceGrpc>();
           endpoints.MapGrpcReflectionService();
        });
    }
}
