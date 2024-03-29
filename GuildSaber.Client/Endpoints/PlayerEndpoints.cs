using CP_SDK.Network;
using GuildSaber.ApiStruct;
using GuildSaber.Client.Types;
using GuildSaber.Enums;
using Newtonsoft.Json;
using OneOf;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace GuildSaber.Client.Endpoints;

public class PlayerEndpoints
{
    private readonly IWebClient _webClient;

    public PlayerEndpoints(IWebClient webClient)
        => _webClient = webClient;

    public void GetAsync(
        int                                                      userId,
        EIncludeFlags                                            includeFlags,
        Action<OneOf<PlayerResponseStruct, ProblemDetailsLite>>? callback = null,
        CancellationToken?                                       token    = null)
    {
        var l_UrlBuilder = new StringBuilder();
        l_UrlBuilder.Append("player/by-id/");
        l_UrlBuilder.AppendFormat("{0}?include={1}", userId, (int)includeFlags);

        if ((includeFlags & ~(EIncludeFlags.Users | EIncludeFlags.Guilds | EIncludeFlags.Categories | EIncludeFlags.Points)) != 0)
            throw new ArgumentException($"[{Constants.PLAYER_ENDPOINT_DEBUG_NAME}.{nameof(GetAsync)}] Unsupported include flags");

        _webClient.GetAsync(l_UrlBuilder.ToString(), token ?? CancellationToken.None, callback != null
            ? webResponse =>
            {
                if (webResponse.StatusCode != HttpStatusCode.OK)
                    callback.Invoke(JsonConvert.DeserializeObject<ProblemDetailsLite>(webResponse.BodyString));
                else
                    callback.Invoke(JsonConvert.DeserializeObject<PlayerResponseStruct>(webResponse.BodyString));
            }
            : _ => EmptyDebugLog());
    }

    public void GetAtMe(Action<OneOf<PlayerAtMeStruct, ProblemDetailsLite>>? callback, CancellationToken? token = null)
        => _webClient.GetAsync("player/@me", token ?? CancellationToken.None, callback != null
            ? webResponse =>
            {
                if (webResponse.StatusCode != HttpStatusCode.OK)
                    callback.Invoke(JsonConvert.DeserializeObject<ProblemDetailsLite>(webResponse.BodyString));
                else
                    callback.Invoke(JsonConvert.DeserializeObject<PlayerAtMeStruct>(webResponse.BodyString));
            }
            : _ => EmptyDebugLog());

    public void GetStats(int userId, int guildId, Action<OneOf<PlayerGuildStatsStruct, ProblemDetailsLite>>? callback, CancellationToken? token = null)
        => _webClient.GetAsync($"player/by-id/{userId}/guild-stats/{guildId}", token ?? CancellationToken.None,
            callback != null
                ? webResponse =>
                {
                    if (webResponse.StatusCode != HttpStatusCode.OK)
                        callback.Invoke(JsonConvert.DeserializeObject<ProblemDetailsLite>(webResponse.BodyString));
                    else
                        callback.Invoke(JsonConvert.DeserializeObject<PlayerGuildStatsStruct>(webResponse.BodyString));
                }
                : _ => EmptyDebugLog());

    public static void EmptyDebugLog([CallerMemberName] string caller = "")
        => GSClient.Logger?.Debug($"[{Constants.GUILD_ENDPOINT_DEBUG_NAME}.{caller}] Executed with no callback");
}
