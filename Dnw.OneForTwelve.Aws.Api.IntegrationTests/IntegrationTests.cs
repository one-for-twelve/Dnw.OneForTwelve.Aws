using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Dnw.OneForTwelve.Aws.Api.IntegrationTests.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Dnw.OneForTwelve.Aws.Api.IntegrationTests;

public class IntegrationTests
{
    public IntegrationTests()
    {
        // Setting this indicates to the api startup code that authentication should be disabled
        // The alternative is to inherit from WebApplicationFactory<Program>. Maybe its then possible to 'unregister' the middleware
        Environment.SetEnvironmentVariable("INTEGRATION_TESTING", "Y");
    }
    
    [Fact]
    public async Task GetGame()
    {
        // Given
        await using var api = new WebApplicationFactory<Program>();
        var client = api.CreateClient();
        
        // When
        var game = await client.GetFromJsonAsync<Game>("/games/Dutch/Random");

        // Then
        Assert.NotNull(game);
        Assert.Equal(12, game.Questions.Count);
    }
}