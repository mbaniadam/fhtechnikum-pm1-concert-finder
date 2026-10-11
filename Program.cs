using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

Console.Write("Band name: ");
var bandName = Console.ReadLine()?.Trim();

Console.Write("City: ");
var city = Console.ReadLine()?.Trim();

if (string.IsNullOrWhiteSpace(bandName) || string.IsNullOrWhiteSpace(city))
{
    Console.WriteLine("Band name and city are required.");
    return;
}

var apiKey = Environment.GetEnvironmentVariable("TICKETMASTER_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("TICKETMASTER_API_KEY is not configured.");
    return;
}
var today = DateTime.UtcNow.Date;
var lastDay = today.AddMonths(2);
var startDateTime = today.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
var endDateTime = lastDay.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
var searchCity = ConcertFinder.NormalizeCity(city);
var url = "https://app.ticketmaster.com/discovery/v2/events.json"
    + $"?apikey={Uri.EscapeDataString(apiKey)}"
    + $"&keyword={Uri.EscapeDataString(bandName)}"
    + $"&city={Uri.EscapeDataString(searchCity)}"
    + $"&startDateTime={startDateTime}&endDateTime={endDateTime}"
    + "&classificationName=music&sort=date,asc&size=20";

try
{
    using var client = new HttpClient();
    var response = await client.GetFromJsonAsync<TicketmasterResponse>(url);

    var concerts = response?.Embedded?.Events?
        .Select(item => new Concert(
                item.Name,
                DateTime.TryParseExact(
                item.Dates?.Start?.LocalDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date) ? date : null))
        ?? Enumerable.Empty<Concert>();

    var concert = ConcertFinder.FindUpcoming(concerts, today);

    Console.WriteLine(concert is null
        ? "No concert found in the next two months."
        : $"{concert.Name}: {concert.Date:yyyy-MM-dd}");
}
catch (HttpRequestException)
{
    Console.WriteLine("Could not contact the concert service.");
}
catch (System.Text.Json.JsonException)
{
    Console.WriteLine("The concert service returned an unexpected response.");
}

internal sealed record TicketmasterResponse(
    [property: JsonPropertyName("_embedded")] EmbeddedEvents? Embedded);

internal sealed record EmbeddedEvents(
    [property: JsonPropertyName("events")] List<ConcertEvent>? Events);

internal sealed record ConcertEvent(string? Name, EventDates? Dates);

internal sealed record EventDates(EventStart? Start);

internal sealed record EventStart(string? LocalDate);