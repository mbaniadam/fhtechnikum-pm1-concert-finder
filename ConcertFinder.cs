public sealed record Concert(string? Name, DateTime? Date);

public static class ConcertFinder
{
    public static string NormalizeCity(string city) =>
        city.Equals("Wien", StringComparison.OrdinalIgnoreCase) ? "Vienna" : city;

    public static Concert? FindUpcoming(IEnumerable<Concert> concerts, DateTime today)
    {
        var lastDay = today.AddMonths(2);

        return concerts
            .Where(concert => concert.Date >= today && concert.Date <= lastDay)
            .OrderBy(concert => concert.Date)
            .FirstOrDefault();
    }
}