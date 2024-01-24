using GuildSaber.ApiStruct;
using GuildSaber.Enums;
using GuildSaber.Models;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace GuildSaber.Client;

public static partial class GSClient
{
    public static class Guilds
    {
        public readonly static string s_DebugName = "GSClient.Controllers.Guilds";

        public static void GetAllAsync(
            uint                      p_Page,
            uint                      p_PageSize,
            bool                      p_IncludeRankedCount,
            bool                      p_IncludeMemberCount,
            EGuildSorters             p_SortBy,
            EOrder                    p_Order,
            EGuildType?               p_GuildTypes     = null,
            string?                   p_Search         = null,
            uint?                     p_UserID         = null,
            EPermission?              p_PermissionFlag = null,
            Action<PagedList<Guild>>? p_Callback       = null,
            CancellationToken?        p_Token          = null)
        {
            var l_UrlBuilder = new StringBuilder();
            l_UrlBuilder.Append("guilds");
            l_UrlBuilder.AppendFormat("?page={0}&pageSize={1}&sortBy={2}&order={3}&include={4}", p_Page, p_PageSize, (uint)p_SortBy, (uint)p_Order,
                (uint)((p_IncludeRankedCount ? EIncludeFlags.RankedMaps : EIncludeFlags.None) & (p_IncludeMemberCount ? EIncludeFlags.Members : EIncludeFlags.None)));

            if (p_GuildTypes.HasValue)
                l_UrlBuilder.AppendFormat("&guildTypes={0}", (int)p_GuildTypes.Value);

            if (p_Search != null)
                l_UrlBuilder.AppendFormat("&search={0}", p_Search);

            if (p_UserID.HasValue)
                l_UrlBuilder.AppendFormat("&userID={0}", p_UserID.Value);

            if (p_PermissionFlag.HasValue)
            {
                if (!p_UserID.HasValue)
                    throw new ArgumentException($"[{s_DebugName}.{nameof(GetAllAsync)}] UserID must be specified when PermissionFlag is specified");

                l_UrlBuilder.AppendFormat("&permissionFlag={0}", (int)p_PermissionFlag.Value);
            }

            GSClient.s_WebClient.GetAsync(l_UrlBuilder.ToString(), p_Token ?? CancellationToken.None, p_Callback != null
                ? p_WebResponse => { p_Callback.Invoke(JsonConvert.DeserializeObject<PagedList<Guild>>(p_WebResponse.BodyString)); }
                : _ => { });
        }

        public static void GetAsync(
            uint               p_GuildID,
            bool               p_IncludeRankedCount,
            bool               p_IncludeMemberCount,
            Action<Guild>?     p_Callback = null,
            CancellationToken? p_Token    = null)
        {
            var l_UrlBuilder = new StringBuilder();
            l_UrlBuilder.AppendFormat("guild/by-id/{0}", p_GuildID);
            l_UrlBuilder.AppendFormat("?include={0}", (uint)((p_IncludeRankedCount ? EIncludeFlags.RankedMaps : EIncludeFlags.None) & (p_IncludeMemberCount ? EIncludeFlags.Members : EIncludeFlags.None)));

            GSClient.s_WebClient.GetAsync(l_UrlBuilder.ToString(), p_Token ?? CancellationToken.None, p_Callback != null
                ? p_WebResponse => { p_Callback.Invoke(JsonConvert.DeserializeObject<Guild>(p_WebResponse.BodyString)!); }
                : _ => { });
        }
    }
}
