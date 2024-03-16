using System.Net;

namespace GuildSaber.Client.Types;

public struct ProblemDetailsLite
{
    public string?         Title  { get; set; }
    public HttpStatusCode? Status { get; set; }
    public string?         Detail { get; set; }
}
