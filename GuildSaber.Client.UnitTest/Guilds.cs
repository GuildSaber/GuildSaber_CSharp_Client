using GuildSaber.ApiStruct;
using GuildSaber.Enums;
using Xunit;

namespace GuildSaber.Client.UnitTest;

public class Guilds
{
    [Fact]
    public async Task GetAllAsyncReturnsNonEmptyList()
    {
        var tcs = new TaskCompletionSource<bool>();
        GSClient.Guilds.GetAllAsync(1, 8, true, true, EGuildSorters.Name, EOrder.Asc,
            callback: result =>
            {
                Assert.True(result.TryPickT0(out var pagedList, out _), "Result is not a PagedList<Guild>");
                Assert.NotEmpty(pagedList.Data);
                tcs.SetResult(true);
            });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAsyncReturnsNonNullGuild()
    {
        var tcs = new TaskCompletionSource<bool>();
        GSClient.Guilds.GetAsync(1, true, true,
            result =>
            {
                Assert.True(result.TryPickT0(out var guild, out _), "Result is not a Guild");
                Assert.NotNull(guild);
                tcs.SetResult(true);
            });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT)); // 1 second timeout
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetStatsAsyncReturnsNonNullStats()
    {
        var tcs = new TaskCompletionSource<GuildStats?>();
        GSClient.Guilds.GetStatsAsync(1, result =>
        {
            Assert.True(result.TryPickT0(out var stats, out _), "Result is not a GuildStats");
            tcs.SetResult(stats);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT)); // 1 second timeout
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }
}
