using CP_SDK.Network;
using System;

namespace GuildSaber.Client;

public static partial class GSClient
{
    public const           string   BASE_ADDRESS     = "https://api-dev.guildsaber.com/";
    public static readonly TimeSpan s_DefaultTimeout = TimeSpan.FromSeconds(10);

#if CP_SDK_UNITY
    private static readonly IWebClient s_WebClient = new WebClientUnity(BASE_ADDRESS, s_DefaultTimeout, true);
#else
    public static readonly IWebClient s_WebClient = new WebClientCore(BASE_ADDRESS, s_DefaultTimeout, true);
#endif
}
