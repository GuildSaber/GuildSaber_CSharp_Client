using GuildSaber.ApiStruct;
using GuildSaber.Enums;
using System.Text;
using Xunit;

namespace GuildSaber.Client.UnitTest;

public class Players
{
    [Fact]
    public async Task GetAsyncReturnsNonNullPlayerWithUserProperties()
    {
        var tcs = new TaskCompletionSource<PlayerResponseStruct?>();
        GSClient.Players.GetAsync(1, EIncludeFlags.Users, response =>
        {
            // Validate the data here
            Assert.NotNull(response);
            Assert.NotNull(response.Value.Player);
            Assert.NotNull(response.Value.Player.User_AvatarUrl);
            // Set the TaskCompletionSource to a completed state
            tcs.SetResult(response);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAsyncReturnsNonNullPlayerWithGuildProperties()
    {
        var tcs = new TaskCompletionSource<PlayerResponseStruct?>();
        GSClient.Players.GetAsync(1, EIncludeFlags.Guilds, response =>
        {
            // Validate the data here
            Assert.NotNull(response);
            Assert.NotNull(response.Value.Player);
            Assert.NotNull(response.Value.Guilds);
            // Set the TaskCompletionSource to a completed state
            tcs.SetResult(response);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAtMeReturnsNullPlayerAtMeStruct()
    {
        var tcs = new TaskCompletionSource<PlayerAtMeStruct?>();
        GSClient.Players.GetAtMe(response =>
        {
            // Validate the data here
            Assert.NotNull(response);
            // Set the TaskCompletionSource to a completed state
            tcs.SetResult(response);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAtMeReturnsNonNullPlayerAtMeStruct()
    {
        var tokenFromFile = await File.ReadAllBytesAsync("token.txt");
        var utf8Token     = Encoding.UTF8.GetString(tokenFromFile);
        GSClient.SetToken(utf8Token);

        var tcs = new TaskCompletionSource<PlayerAtMeStruct?>();
        GSClient.Players.GetAtMe(response =>
        {
            // Validate the data here
            Assert.NotNull(response);
            Assert.NotNull(response.Value.Player);
            // Set the TaskCompletionSource to a completed state
            tcs.SetResult(response);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetStats()
    {
        var tcs = new TaskCompletionSource<PlayerPointStatsStruct?>();
        GSClient.Players.GetStats(1, 1, response =>
        {
            // Validate the data here
            Assert.NotNull(response);
            // Set the TaskCompletionSource to a completed state
            tcs.SetResult(response);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }
}
