using GuildSaber.Enums;
using System.Net;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace GuildSaber.Client.UnitTest;

public class RankedMaps
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RankedMaps(ITestOutputHelper testOutputHelper)
        => _testOutputHelper = testOutputHelper;

    [Fact]
    public async Task GetAsyncReturnsRankedMapWithSongName()
    {
        var tokenFromFile = await File.ReadAllBytesAsync("token.txt");
        var utf8Token     = Encoding.UTF8.GetString(tokenFromFile);
        GSClient.SetToken(utf8Token);

        var tcs = new TaskCompletionSource<bool>();
        GSClient.RankedMaps.GetAsync(1, EIncludeFlags.RankedMapVersions | EIncludeFlags.SongDifficulties | EIncludeFlags.Songs, result =>
        {
            Assert.True(result.TryPickT0(out var rankedResponse, out _), "Result is not a RankedMap");
            Assert.NotNull(rankedResponse.RankedMap.RankedMapVersions);
            Assert.NotEmpty(rankedResponse.RankedMap.RankedMapVersions);
            Assert.NotNull(rankedResponse.RankedMap.RankedMapVersions[0].SongDifficulty);
            Assert.NotNull(rankedResponse.RankedMap.RankedMapVersions[0].SongDifficulty!.Song);
            Assert.NotNull(rankedResponse.RankedMap.RankedMapVersions[0].SongDifficulty!.Song!.Name);
            tcs.SetResult(true);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAsyncReturnProblemDetailsLiteNotFound()
    {
        var tcs = new TaskCompletionSource<bool>();
        GSClient.RankedMaps.GetAsync(uint.MaxValue, EIncludeFlags.RankedMapVersions | EIncludeFlags.SongDifficulties | EIncludeFlags.Songs, result =>
        {
            Assert.True(result.TryPickT1(out var problemDetailsLite, out _), "Result is not a ProblemDetailsLite");
            Assert.Equal(HttpStatusCode.NotFound, problemDetailsLite.Status);
            tcs.SetResult(true);
        });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAllAsyncReturnsNonEmptyListWithScoreFromSpecificMap()
    {
        var tokenFromFile = await File.ReadAllBytesAsync("token.txt");
        var utf8Token     = Encoding.UTF8.GetString(tokenFromFile);
        GSClient.SetToken(utf8Token);

        var tcs = new TaskCompletionSource<bool>();
        GSClient.RankedMaps.GetAllAsync(1, 1, 8, ESorter.Difficulty, EOrder.Asc,
            EIncludeFlags.RankedMapVersions | EIncludeFlags.SongDifficulties | EIncludeFlags.Songs | EIncludeFlags.RankedScores, true, EPassState.AllAllowed,
            search: "a3c3", bpmFrom: 1, bpmTo: 300, durationFrom: 0, difficultyTo: 2, categoryIDs: new uint[] { 1, 2, 3, 4, 5, 6, 7 },
            callback: result =>
            {
                Assert.True(result.TryPickT0(out var pagedList, out _), "Result is not a PagedList<RankedMap>");
                Assert.NotEmpty(pagedList.Data);
                Assert.NotNull(pagedList.Data[0].RankedMap);
                Assert.NotNull(pagedList.Data[0].RankedScore);
                tcs.SetResult(true);
            });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAllAsyncReturnsEmptyListFromPassStateFilter()
    {
        var tokenFromFile = await File.ReadAllBytesAsync("token.txt");
        var utf8Token     = Encoding.UTF8.GetString(tokenFromFile);
        GSClient.SetToken(utf8Token);

        var tcs = new TaskCompletionSource<bool>();
        GSClient.RankedMaps.GetAllAsync(1, 1, 8, ESorter.Difficulty, EOrder.Asc,
            EIncludeFlags.RankedMapVersions | EIncludeFlags.SongDifficulties | EIncludeFlags.Songs | EIncludeFlags.RankedScores, true, EPassState.UnPassed,
            search: "a3c3", bpmFrom: 1, bpmTo: 300, durationFrom: 0, difficultyTo: 2, categoryIDs: new uint[] { 5, 6, 7 },
            callback: result =>
            {
                Assert.True(result.TryPickT0(out var pagedList, out _), "Result is not a PagedList<RankedMap>");
                Assert.Empty(pagedList.Data);
                tcs.SetResult(true);
            });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }

    [Fact]
    public async Task GetAllAsyncReturnsNonEmptyListWithoutScore()
    {
        var tcs = new TaskCompletionSource<bool>();
        GSClient.RankedMaps.GetAllAsync(1, 1, 8, ESorter.Difficulty, EOrder.Asc,
            EIncludeFlags.RankedMapVersions | EIncludeFlags.SongDifficulties | EIncludeFlags.Songs | EIncludeFlags.RankedScores, true, EPassState.All,
            search: "seven spice", bpmFrom: 1, bpmTo: 300, durationFrom: 0, difficultyTo: 2, categoryIDs: new uint[] { 1, 2, 3, 4, 5, 6, 7 },
            callback: result =>
            {
                Assert.True(result.TryPickT0(out var pagedList, out _), "Result is not a PagedList<RankedMap>");
                Assert.NotEmpty(pagedList.Data);
                Assert.NotNull(pagedList.Data[0].RankedMap);
                Assert.Null(pagedList.Data[0].RankedScore);
                tcs.SetResult(true);
            });

        var result = await Task.WhenAny(tcs.Task, Task.Delay(Constants.TIMEOUT));
        Assert.True(result == tcs.Task, "Callback was not invoked");
    }
}
