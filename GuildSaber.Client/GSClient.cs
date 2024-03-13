using CP_SDK.Logging;
using CP_SDK.Network;
using GuildSaber.Client.Endpoints;
using System;

namespace GuildSaber.Client;

public static partial class GSClient
{
    public const           string     BASE_ADDRESS     = "https://api-dev.guildsaber.com/";
    public static readonly TimeSpan   s_DefaultTimeout = TimeSpan.FromSeconds(10);
    public static readonly IWebClient s_WebClient;

    public static readonly GuildEndpoints Guilds;
    public static readonly PlayerEndpoints Players;

    static GSClient()
    {
#if CP_SDK_UNITY
        s_WebClient = new WebClientUnity(BASE_ADDRESS, s_DefaultTimeout, true);
#else
        s_WebClient = new WebClientCore(BASE_ADDRESS, s_DefaultTimeout, true);
#endif
        Guilds = new GuildEndpoints(s_WebClient);
        Players = new PlayerEndpoints(s_WebClient);
    }

    public static ILogger? Logger { get; set; }

    public static void SetToken(string token)
    {
        s_WebClient.SetHeader("Authorization", $"Bearer {token}");
    }
}
