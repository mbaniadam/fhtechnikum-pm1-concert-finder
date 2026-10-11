var today = new DateTime(2026, 10, 11);
var concerts = new[]
{
    new Concert("No date", null),
    new Concert("Past", new DateTime(2026, 10, 3)),
    new Concert("Too late", new DateTime(2027, 1, 10)),
    new Concert("Muse", new DateTime(2026, 12, 10))
};

var result = ConcertFinder.FindUpcoming(concerts, today);

if (result?.Name != "Muse" || result.Date != new DateTime(2026, 12, 10))
{
    throw new Exception("FindUpcoming did not return the expected concert.");
}

var noUpcomingConcert = ConcertFinder.FindUpcoming(
    new[] { new Concert("Too late", new DateTime(2027, 1, 10)) },
    today);

if (noUpcomingConcert is not null)
{
    throw new Exception("FindUpcoming should return null when no concert is within two months.");
}

if (ConcertFinder.NormalizeCity("Wien") != "Vienna")
{
    throw new Exception("NormalizeCity should convert Wien to Vienna.");
}

if (ConcertFinder.NormalizeCity("Graz") != "Graz")
{
    throw new Exception("NormalizeCity should leave other city names unchanged.");
}

Console.WriteLine("Tests passed.");