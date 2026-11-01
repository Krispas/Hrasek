namespace Hrasek;
public class MatchManager(Tournament activeTournament, PlayerManager playerManager)
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
    public void setMatchWinner(Match match, Player winner)
    {
        if (match.Player1 == winner)
        {
            match.scoreP1 = 3;
            match.scoreP2 = 0;
            match.matchFinished = true;
            match.winner = match.Player1;
            return;
        }

        if (match.Player2 == winner)
        {
            match.scoreP1 = 0;
            match.scoreP2 = 3;
            match.matchFinished = true;
            match.winner = match.Player2;
            return;
        }

        throw new PlayerNotInThisMatchExeption();
    }
    public void setMatchWinner(Match match, string nickname)
        {
            Player winner = playerManager.getPlayer(nickname);
            if (match.Player1 == winner)
            {
                match.scoreP1 = 3;
                match.scoreP2 = 0;
                match.matchFinished = true;
                match.winner = match.Player1;
                return;
            }

            if (match.Player2 == winner)
            {
                match.scoreP1 = 0;
                match.scoreP2 = 3;
                match.matchFinished = true;
                match.winner = match.Player2;
                return;
            }
        }
        public void setMatchDraw(Match match)
    {
        match.scoreP1 = 1;
        match.scoreP2 = 1;
        match.matchFinished = true;
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