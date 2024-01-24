using CP_SDK;
using GuildSaber.ApiStruct;

namespace GuildSaber.Client.UnitTest;

class Program
{
    static void Main()
    {
        GSClient.Guilds.GetAllAsync(1, 8, true, true, EGuildSorters.Name, Enums.EOrder.Asc,
            p_Callback: p_List =>
            {
                foreach (var l_Guild in p_List.Data)
                {
                    ChatPlexSDK.Logger.Info(l_Guild.Name);
                }
            });

        GSClient.Guilds.GetAsync(1, true, true,
            p_Callback: p_Guild =>
            {
                ChatPlexSDK.Logger.Info(p_Guild.Name);
            });

        while (true)
            Thread.Sleep(50);
        // ReSharper disable once FunctionNeverReturns
    }
}
