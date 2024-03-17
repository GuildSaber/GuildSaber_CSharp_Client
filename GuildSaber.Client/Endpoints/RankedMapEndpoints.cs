using CP_SDK.Network;
using GuildSaber.ApiStruct;
using GuildSaber.Client.Types;
using GuildSaber.Enums;
using Newtonsoft.Json;
using OneOf;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace GuildSaber.Client.Endpoints;

public class RankedMapEndpoints
{
    private readonly IWebClient _webClient;

    public RankedMapEndpoints(IWebClient webClient)
        => _webClient = webClient;

    public void GetAsync(uint id, EIncludeFlags includeFlags, Action<OneOf<RankedMapWithScoreStruct, ProblemDetailsLite>>? callback, CancellationToken? token = null)
    {
        var l_UrlBuilder = new StringBuilder();

        if ((includeFlags & ~(EIncludeFlags.RankedMapVersions | EIncludeFlags.SongDifficulties | EIncludeFlags.Songs | EIncludeFlags.GameModes | EIncludeFlags.RankedScores | EIncludeFlags.Scores | EIncludeFlags.Points)) != 0)
            throw new ArgumentException($"[{Constants.RANKED_MAP_ENDPOINT_DEBUG_NAME}.{nameof(GetAsync)}] Unsupported include flags");

        l_UrlBuilder.Append("ranked-map/by-id/");
        l_UrlBuilder.AppendFormat("{0}?include={1}", id, (int)includeFlags);

        _webClient.GetAsync(l_UrlBuilder.ToString(), token ?? CancellationToken.None,
            callback != null
                ? webResponse =>
                {
                    if (webResponse.StatusCode != HttpStatusCode.OK)
                        callback.Invoke(JsonConvert.DeserializeObject<ProblemDetailsLite>(webResponse.BodyString));
                    else
                        callback.Invoke(JsonConvert.DeserializeObject<RankedMapWithScoreStruct>(webResponse.BodyString));
                }
                : _ => EmptyDebugLog());
    }

    /// <summary>
    ///     Retrieves a paginated list of ranked maps for a specific guild asynchronously.
    /// </summary>
    /// <param name="guildID">The ID of the guild for which to retrieve the ranked maps.</param>
    /// <param name="p_Page">The page number to retrieve in the paginated result set. Default is 1.</param>
    /// <param name="p_PageSize">The number of ranked maps to include in each page of the result set. Default is 10.</param>
    /// <param name="sorter">The criteria by which to sort the returned ranked maps.</param>
    /// <param name="order">The order in which to sort the returned ranked maps. Default is descending.</param>
    /// <param name="include">Flags indicating which related entities to include in the response.</param>
    /// <param name="anyMatch">
    ///     A boolean indicating whether to return ranked maps that are in any of the categories provided.
    ///     When false, only ranked maps that match at least all of the categories provided will be returned.
    /// </param>
    /// <param name="callback">The callback function to be invoked when the asynchronous operation completes.</param>
    /// <param name="state">
    ///     The State of the Ranked Scores the user should have on the Ranked Maps. If the state provided is
    ///     AllAllowed, then only the RankedMaps with present scores that have such state will be sent. If more states are
    ///     passed, the same thing occurs, only maps with scores of the user that match at least one of the state will be
    ///     returned. If you wish to provide UnPassed maps by the user too, maybe by still wanting only the passed maps to be
    ///     filtered, provide EState.UnPassed alongside them (It's effective in EState.All too)
    /// </param>
    /// <param name="search">
    ///     A search term to filter the returned ranked maps. The search is applied on the 'Song.Name',
    ///     'Song.MapperName', 'Song.SongAuthorName', 'Song.BeatSaverKey', 'Song.Hash' fields of the ranked maps.
    /// </param>
    /// <param name="categoryIDs">
    ///     An array of category IDs to filter the returned ranked maps. If provided, only ranked maps
    ///     that have at least one of the provided categories will be returned.
    /// </param>
    /// <param name="difficultyFrom">
    ///     The minimum difficulty level of the ranked maps to return. The comparison includes a
    ///     tolerance of 0.01f to account for minor rounding errors.
    /// </param>
    /// <param name="difficultyTo">
    ///     The maximum difficulty level of the ranked maps to return. The comparison includes a
    ///     tolerance of 0.01f to account for minor rounding errors.
    /// </param>
    /// <param name="durationFrom">
    ///     The minimum length of the ranked maps to return. The comparison includes a tolerance of
    ///     0.01f to account for minor rounding errors.
    /// </param>
    /// <param name="durationTo">
    ///     The maximum length of the ranked maps to return. The comparison includes a tolerance of 0.01f
    ///     to account for minor rounding errors.
    /// </param>
    /// <param name="bpmFrom">
    ///     The minimum BPM of the ranked maps to return. The comparison includes a tolerance of 0.01f to
    ///     account for minor rounding errors.
    /// </param>
    /// <param name="bpmTo">
    ///     The maximum BPM of the ranked maps to return. The comparison includes a tolerance of 0.01f to
    ///     account for minor rounding errors.
    /// </param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    public void GetAllAsync(uint                                                                    guildID,
                            int                                                                     p_Page,
                            int                                                                     p_PageSize,
                            ESorter                                                                 sorter,
                            EOrder                                                                  order,
                            EIncludeFlags                                                           include,
                            bool                                                                    anyMatch,
                            EPassState?                                                             passState,
                            Action<OneOf<PagedList<RankedMapWithScoreStruct>, ProblemDetailsLite>>? callback,
                            string?                                                                 search         = null,
                            uint[]?                                                                 categoryIDs    = null,
                            float?                                                                  difficultyFrom = null,
                            float?                                                                  difficultyTo   = null,
                            float?                                                                  durationFrom   = null,
                            float?                                                                  durationTo     = null,
                            float?                                                                  bpmFrom        = null,
                            float?                                                                  bpmTo          = null,
                            CancellationToken?                                                      token          = null)
    {
        var l_UrlBuilder = new StringBuilder();

        l_UrlBuilder.Append("ranked-maps/");
        l_UrlBuilder.AppendFormat("{0}?sort-by={1}&order-by={2}&include={3}&page={4}&pageSize={5}",
            guildID, sorter, order, include, p_Page, p_PageSize);

        if (search != null)
            l_UrlBuilder.Append($"&search={search}");

        if (categoryIDs is { Length: > 0 })
            l_UrlBuilder.Append($"&category-ids={string.Join("&category-ids=", categoryIDs)}");

        if (!anyMatch)
            l_UrlBuilder.Append("&anyMatch=false");

        if (passState != null)
            l_UrlBuilder.Append($"&passState={passState}");

        if (difficultyFrom != null)
            l_UrlBuilder.Append($"&difficulty-from={difficultyFrom}");

        if (difficultyTo != null)
            l_UrlBuilder.Append($"&difficulty-to={difficultyTo}");

        if (durationFrom != null)
            l_UrlBuilder.Append($"&duration-from={durationFrom}");

        if (durationTo != null)
            l_UrlBuilder.Append($"&duration-to={durationTo}");

        if (bpmFrom != null)
            l_UrlBuilder.Append($"&bpm-from={bpmFrom}");

        if (bpmTo != null)
            l_UrlBuilder.Append($"&bpm-to={bpmTo}");

        _webClient.GetAsync(l_UrlBuilder.ToString(), token ?? CancellationToken.None,
            callback != null
                ? webResponse =>
                {
                    if (webResponse.StatusCode != HttpStatusCode.OK)
                        callback.Invoke(JsonConvert.DeserializeObject<ProblemDetailsLite>(webResponse.BodyString));
                    else
                        callback.Invoke(JsonConvert.DeserializeObject<PagedList<RankedMapWithScoreStruct>>(webResponse.BodyString));
                }
                : _ => EmptyDebugLog());
    }

    public static void EmptyDebugLog([CallerMemberName] string caller = "")
        => GSClient.Logger?.Debug($"[{Constants.RANKED_MAP_ENDPOINT_DEBUG_NAME}.{caller}] Executed with no callback");
}
