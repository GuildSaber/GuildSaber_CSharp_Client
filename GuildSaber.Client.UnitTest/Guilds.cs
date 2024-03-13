using GuildSaber.ApiStruct;
using GuildSaber.Enums;
using GuildSaber.Models;
using Xunit;

namespace GuildSaber.Client.UnitTest;

public class Guilds
{
    [Fact]
    public async Task GetAllAsyncReturnsNonEmptyList()
    {
        var tcs = new TaskCompletionSource<List<Guild?>>();
        GSClient.Guilds.GetAllAsync(1, 8, true, true, EGuildSorters.Name, EOrder.Asc,
            callback: pagedList =>
            {
                // Validate the data here
                Assert.NotEmpty(pagedList.Data);
                // Set the TaskCompletionSource to a completed state
                tcs.SetResult(pagedList.Data);
            });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAsyncReturnsNonNullGuild()
    {
        var tcs = new TaskCompletionSource<Guild?>();
        GSClient.Guilds.GetAsync(1, true, true,
            guild =>
            {
                // Validate the data here
                Assert.NotNull(guild);
                // Set the TaskCompletionSource to a completed state
                tcs.SetResult(guild);
            });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT)); // 1 second timeout
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetStatsAsyncReturnsNonNullStats()
    {
        var tcs = new TaskCompletionSource<GuildStats?>();
        GSClient.Guilds.GetStatsAsync(1, stats =>
        {
            // Validate the data here
            Assert.NotNull(stats);
            // Set the TaskCompletionSource to a completed state
            tcs.SetResult(stats);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT)); // 1 second timeout
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }
}
