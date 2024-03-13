using CP_SDK.Network;
using GuildSaber.ApiStruct;
using GuildSaber.Enums;
using Newtonsoft.Json;
using System;
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
        int                           userId,
        EIncludeFlags                 includeFlags,
        Action<PlayerResponseStruct?>? callback = null,
        CancellationToken?            token    = null)
    {
        var l_UrlBuilder = new StringBuilder();
        l_UrlBuilder.Append("player/by-id/");
        l_UrlBuilder.AppendFormat("{0}?include={1}", userId, (int)includeFlags);

        if ((includeFlags & ~(EIncludeFlags.Users | EIncludeFlags.Guilds | EIncludeFlags.Categories | EIncludeFlags.Points)) != 0)
            throw new ArgumentException($"[{Constants.PLAYER_ENDPOINT_DEBUG_NAME}.{nameof(GetAsync)}] Unsupported include flags");

        _webClient.GetAsync(l_UrlBuilder.ToString(), token ?? CancellationToken.None, callback != null
            ? webResponse =>
            {
                DebugLog(false);
                var deserialized = JsonConvert.DeserializeObject<PlayerResponseStruct>(webResponse.BodyString);
                callback.Invoke(deserialized.Player is null ? null : deserialized);
            }
            : _ => DebugLog(true));
    }

    public void GetAtMe(Action<PlayerAtMeStruct?>? callback, CancellationToken? token = null)
        => _webClient.GetAsync("player/@me", token ?? CancellationToken.None, callback != null
            ? webResponse =>
            {
                DebugLog(false);
                var deserialized = JsonConvert.DeserializeObject<PlayerAtMeStruct>(webResponse.BodyString);
                // if deserialized is null or default struct value, invoke the callback with null
                callback.Invoke(deserialized.Player is null ? null : deserialized);
            }
            : _ => DebugLog(true));

    public void GetStats(int userId, int pointId, Action<PlayerPointStatsStruct?> callback, CancellationToken? token = null)
    => _webClient.GetAsync($"player/{userId}/stats/{pointId}", token ?? CancellationToken.None, webResponse =>
    {
        DebugLog(false);
        var deserialized = JsonConvert.DeserializeObject<PlayerPointStatsStruct>(webResponse.BodyString);
        callback.Invoke(deserialized.PlayerID == 0 ? null : deserialized);
    });

    public static void DebugLog(bool isNull, [CallerMemberName] string caller = "")
        => GSClient.Logger?.Debug($"[{Constants.GUILD_ENDPOINT_DEBUG_NAME}.{caller}] {(isNull ? "No callback" : "Callback invoked")}");
}
