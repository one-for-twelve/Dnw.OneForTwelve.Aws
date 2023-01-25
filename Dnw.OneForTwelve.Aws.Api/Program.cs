using Dnw.OneForTwelve.Core.Models;
using Dnw.OneForTwelve.Core.Services;
using JetBrains.Annotations;
using Serilog;

namespace Dnw.OneForTwelve.Aws.Api;

[UsedImplicitly]
internal class Program
{
    private static int Main(string[] args)
    {
        var (statusCode, app) = WebAppBuilder.Build(args);

        if (statusCode != 0) return statusCode;

        var homeRouteBuilder = app!.MapGet("/", (IHttpContextAccessor ctx) =>
        {
            var userName = ctx.HttpContext?.User.Identity?.Name ?? "anonymous";
            var architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
            var dotnetVersion = Environment.Version.ToString();
            return $"Username: {userName}, Env: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}, Architecture: {architecture}, .NET Version: {dotnetVersion}";
        });

        var startGameRouteBuilder = app!.MapGet("/games/{language}/{questionSelectionStrategy}", 
            (
                IHttpContextAccessor ctx,
                Languages language, 
                QuestionSelectionStrategies questionSelectionStrategy, 
                IGameService gameService) =>
            {
                if (ctx.HttpContext!.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    var log = Log.ForContext<Program>();
                    log.Debug("AuthHeader: {authHeader}",authHeader);
                }
    
                var game = gameService.Start(language, questionSelectionStrategy);
                
                return game == null ? Results.BadRequest() : Results.Ok(game);
            });

        WebAppBuilder.RequireAuthorization(new[] {homeRouteBuilder, startGameRouteBuilder});

        app!.Run();

        return statusCode;
    }
}