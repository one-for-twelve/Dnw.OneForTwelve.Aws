using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;
using Dnw.OneForTwelve.Core.Models;

namespace Dnw.OneForTwelve.Aws.Api;

[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
[JsonSerializable(typeof(Game))]
[JsonSerializable(typeof(GameQuestion))]
[JsonSerializable(typeof(Question))]
[JsonSerializable(typeof(QuestionCategories))]
[JsonSerializable(typeof(QuestionLevels))]
[JsonSerializable(typeof(RemoteVideo))]
[JsonSerializable(typeof(RemoteVideoSources))]
[JsonSerializable(typeof(QuestionSelectionStrategies))]
[JsonSerializable(typeof(Languages))]
public partial class HttpApiJsonSerializerContext : JsonSerializerContext
{
}