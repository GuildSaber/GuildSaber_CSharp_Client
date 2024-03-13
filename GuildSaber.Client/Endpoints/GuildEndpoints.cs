using CP_SDK.Network;
using GuildSaber.ApiStruct;
using GuildSaber.Enums;
using GuildSaber.Models;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace GuildSaber.Client.Endpoints;

public class GuildEndpoints
{
    private readonly IWebClient _webClient;

    public GuildEndpoints(IWebClient webClient)
        => _webClient = webClient;

    public void GetAllAsync(
        uint                       page,
        uint                       pageSize,
        bool                       includeRankedCount,
        bool                       includeMemberCount,
        EGuildSorters              sortBy,
        EOrder                     order,
        EGuildType?                guildTypes     = null,
        string?                    search         = null,
        uint?                      userID         = null,
        EPermission?               permissionFlag = null,
        Action<PagedList<Guild?>>? callback       = null,
        CancellationToken?         token          = null)
    {
        var l_UrlBuilder = new StringBuilder();
        l_UrlBuilder.Append("guilds");
        l_UrlBuilder.AppendFormat("?page={0}&pageSize={1}&sortBy={2}&order={3}&include={4}", page, pageSize, (uint)sortBy, (uint)order,
            (uint)((includeRankedCount ? EIncludeFlags.RankedMaps : EIncludeFlags.None) & (includeMemberCount ? EIncludeFlags.Members : EIncludeFlags.None)));

        if (guildTypes.HasValue)
            l_UrlBuilder.AppendFormat("&guildTypes={0}", (int)guildTypes.Value);

        if (search != null)
            l_UrlBuilder.AppendFormat("&search={0}", search);

        if (userID.HasValue)
            l_UrlBuilder.AppendFormat("&userID={0}", userID.Value);

        if (permissionFlag.HasValue)
        {
            if (!userID.HasValue)
                throw new ArgumentException($"[{Constants.GUILD_ENDPOINT_DEBUG_NAME}.{nameof(GetAllAsync)}] UserID must be specified when PermissionFlag is specified");

            l_UrlBuilder.AppendFormat("&permissionFlag={0}", (int)permissionFlag.Value);
        }

        _webClient.GetAsync(l_UrlBuilder.ToString(), token ?? CancellationToken.None, callback != null
            ? webResponse =>
            {
                DebugLog(false);
                callback.Invoke(JsonConvert.DeserializeObject<PagedList<Guild?>>(webResponse.BodyString));
            }
            : _ => { DebugLog(true); });
    }

    public void GetAsync(
        uint               guildID,
        bool               includeRankedCount,
        bool               includeMemberCount,
        Action<Guild?>?    callback = null,
        CancellationToken? token    = null)
    {
        var l_UrlBuilder = new StringBuilder();
        l_UrlBuilder.AppendFormat("guild/by-id/{0}", guildID);
        l_UrlBuilder.AppendFormat("?include={0}",    (uint)((includeRankedCount ? EIncludeFlags.RankedMaps : EIncludeFlags.None) & (includeMemberCount ? EIncludeFlags.Members : EIncludeFlags.None)));

        _webClient.GetAsync(l_UrlBuilder.ToString(), token ?? CancellationToken.None, callback != null
            ? webResponse =>
            {
                DebugLog(false);
                callback.Invoke(JsonConvert.DeserializeObject<Guild?>(webResponse.BodyString)!);
            }
            : _ => { DebugLog(true); });
    }

    public void GetStatsAsync(
        uint                 guildID,
        Action<GuildStats?>? callback = null,
        CancellationToken?   token    = null)
    {
        var l_UrlBuilder = new StringBuilder();
        l_UrlBuilder.AppendFormat("guild/by-id/{0}/stats", guildID);

        _webClient.GetAsync(l_UrlBuilder.ToString(), token ?? CancellationToken.None, callback != null
            ? webResponse =>
            {
                DebugLog(false);
                callback.Invoke(JsonConvert.DeserializeObject<GuildStats?>(webResponse.BodyString)!);
            }
            : _ => { DebugLog(true); });
    }

    public static void DebugLog(bool isNull, [CallerMemberName] string caller = "")
        => GSClient.Logger?.Debug($"[{Constants.GUILD_ENDPOINT_DEBUG_NAME}.{caller}] {(isNull ? "No callback" : "Callback invoked")}");
}
