using Amazon.Lambda.Serialization.SystemTextJson;
using Dnw.OneForTwelve.Core.Extensions;
using Microsoft.AspNetCore.Http.Json;
using Serilog;

namespace Dnw.OneForTwelve.Aws.Api;

public static class WebAppBuilder
{
    private static bool IntegrationTesting => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("INTEGRATION_TESTING"));
    
    public static (int, WebApplication?) Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        Log.Logger = logger;

        try
        {
            ConfigureServices(builder.Services);
            
            var app = builder.Build();
            
            AddMiddleware(app);

            return (0, app);
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "An error occured during startup");
            return (1, null);
        }
    }

    public static void RequireAuthorization(IEnumerable<RouteHandlerBuilder> routeHandlerBuilders)
    {
        if (IntegrationTesting) return;
        
        foreach (var routeHandlerBuilder in routeHandlerBuilders)
        {
            routeHandlerBuilder.RequireAuthorization();
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        if (!IntegrationTesting)
        {
            services.AddFirebaseAuth();
        }
        
        services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
        
        services.AddAWSLambdaHosting(LambdaEventSource.HttpApi, options =>
        {
            options.Serializer = new SourceGeneratorLambdaJsonSerializer<HttpApiJsonSerializerContext>();
        });

        // Add services to the container.

        services.AddHttpContextAccessor();

        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.AddContext<HttpApiJsonSerializerContext>();
        });

        // This does not work. Configuring JsonOptions like above does.
        // builder.Services.ConfigureJsonSerializerOptions();

        // Cache in the init phase of the lambda because more cpu is available and it's free
        services.AddGameServices();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void AddMiddleware(WebApplication app)
    {
        if (!IntegrationTesting)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseGameServices();
    }
}