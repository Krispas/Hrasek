namespace Hrasek;
public class MatchManager(Tournament? activeTournament)
{
    public void addMatch(Match match) // Pres tuto metodu se budou vytvaret zapasi v losovani
    {
        if (activeTournament == null)
        {
            throw new TournamentNotExistingExeption();
        }

        if (!activeTournament.tournamentStarted)
        {
            throw new TournamentNotStartedExeption();
        }
        activeTournament.matches.Add(match);
    }

    public List<Match> getCurentRoundMatches()
    {
        List<Match> curentRoundMatches = new List<Match>();
            if (activeTournament == null)
            {
                throw new TournamentNotExistingExeption();
            }
            if (!activeTournament.tournamentStarted)
            {
                throw new TournamentNotStartedExeption();
            }
    }
    public List<Match> getMatches()  // Z teto metody se bude tvořit Tabulka, plus na ukladani se bude hodit
    {
        if (activeTournament == null)
        {
            throw new TournamentNotExistingExeption();
        }
        if (!activeTournament.tournamentStarted)
        {
            throw new TournamentNotStartedExeption();
        }
        return activeTournament.matches;
    }
}