using GuildSaber.Enums;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace GuildSaber.Client.UnitTest;

public class Players
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Players(ITestOutputHelper testOutputHelper)
        => _testOutputHelper = testOutputHelper;

    [Fact]
    public async Task GetAsyncReturnsNonNullPlayerWithUserProperties()
    {
        var tcs = new TaskCompletionSource<bool>();
        GSClient.Players.GetAsync(1, EIncludeFlags.Users, response =>
        {
            Assert.True(response.TryPickT0(out var playerResponseStruct, out _), "Result is not a PlayerResponseStruct");
            Assert.NotNull(playerResponseStruct.Player.User_AvatarUrl);
            Assert.NotEmpty(playerResponseStruct.Player.User_AvatarUrl);
            tcs.SetResult(true);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAsyncReturnsNonNullPlayerWithGuildProperties()
    {
        var tcs = new TaskCompletionSource<bool>();
        GSClient.Players.GetAsync(1, EIncludeFlags.Guilds, response =>
        {
            Assert.True(response.TryPickT0(out var playerResponseStruct, out _), "Result is not a PlayerResponseStruct");
            Assert.NotNull(playerResponseStruct.Player);
            Assert.NotNull(playerResponseStruct.Guilds);
            Assert.NotEmpty(playerResponseStruct.Guilds);
            tcs.SetResult(true);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAtMeReturnsNullPlayerAtMeStruct()
    {
        var tcs = new TaskCompletionSource<bool>();
        GSClient.Players.GetAtMe(response =>
        {
            Assert.False(response.TryPickT0(out _, out var problemDetailsLite), "Result is not a ProblemDetail");
            _testOutputHelper.WriteLine($"Status code is {problemDetailsLite.Status}");
            tcs.SetResult(true);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.LONG_TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAtMeReturnsNonNullPlayerAtMeStruct()
    {
        var tokenFromFile = await File.ReadAllBytesAsync("token.txt");
        var utf8Token     = Encoding.UTF8.GetString(tokenFromFile);
        GSClient.SetToken(utf8Token);

        var tcs = new TaskCompletionSource<bool>();
        GSClient.Players.GetAtMe(response =>
        {
            Assert.True(response.TryPickT0(out var playerAtMeStruct, out _), "Result is not a PlayerAtMeStruct");
            Assert.NotEmpty(playerAtMeStruct.Player.Name);

            tcs.SetResult(true);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetStats()
    {
        var tcs = new TaskCompletionSource<bool>();
        GSClient.Players.GetStats(1, 1, result =>
        {
            Assert.True(result.TryPickT0(out var stats, out _), "Result is not a PlayerPointStatsStruct");
            tcs.SetResult(true);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }
}
