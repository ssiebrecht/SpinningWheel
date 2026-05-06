namespace SpinningWheel.Services;

public sealed class QuoteService : IQuoteService
{
    private static readonly IReadOnlyList<string> Quotes = new[]
    {
        "Vierzig Stunden pro Woche, zweiundfünfzig Wochen pro Jahr – irgendwann hört die Mathematik auf, lustig zu sein.",
        "Heute hast du dein Bestes gegeben. Das war vermutlich nicht genug, aber der Gedanke zählt.",
        "Irgendwo da draußen gibt es den perfekten Job für dich. Du wirst ihn nie finden.",
        "Das Meeting hätte auch eine E-Mail sein können. Die E-Mail hätte auch nichts sein können.",
        "Du hast heute etwas bewegt. Morgen wird es jemand wieder zurückbewegen.",
        "Erfolg ist relativ. Je weniger Erfolg, desto weniger Verwandte.",
        "Deine Arbeit wird eines Tages durch eine KI ersetzt. Bis dahin kannst du so tun, als wärst du unersetzlich.",
        "Der Freitag kommt immer. Leider kommt danach immer der Montag.",
        "Du gibst jeden Tag dein Bestes. Das Beste summiert sich nicht.",
        "Manchmal ist die beste Lösung die einfachste. Manchmal findet sie trotzdem niemand.",
        "Erfahrung ist das, was man hat, wenn man es zu spät gemerkt hat.",
        "Der Workflow ist optimiert. Die Arbeit bleibt trotzdem liegen.",
        "Manche Tage laufen einfach nicht gut. Statistisch gesehen sind das die meisten.",
    };

    private readonly Random _random = new();

    public string GetRandom()
    {
        return Quotes[_random.Next(Quotes.Count)];
    }
}
