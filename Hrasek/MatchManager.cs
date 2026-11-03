namespace Hrasek;

public class MatchManager
{
    private readonly Tournament? activeTournament;
    private readonly PlayerManager playerManager;

    public MatchManager(Tournament? activeTournament, PlayerManager playerManager)
    {
        this.activeTournament = activeTournament;
        this.playerManager = playerManager;
    }

    private Tournament requireOpenRound()
    {
        if (activeTournament == null) throw new TournamentNotExistingExeption();
        if (!activeTournament.tournamentStarted) throw new TournamentNotStartedExeption();
        if (activeTournament.roundClosed || activeTournament.tournamentFinished)
            throw new TournamentExeptions("Do uzavřeného kola nelze zapisovat.");
        return activeTournament;
    }

    public void addMatch(Match match)
    {
        Tournament t = requireOpenRound();
        if (match.Round != t.currentRound) throw new TournamentExeptions("Zápas patří do jiného kola.");
        if (match.Player1 == null || !t.tournamentPlayers.Contains(match.Player1) ||
            (match.Player2 != null && !t.tournamentPlayers.Contains(match.Player2)))
            throw new PlayerExeptions("Hráč není přihlášen do turnaje.");
        if (match.Player1 == match.Player2) throw new TournamentExeptions("Hráč nemůže hrát sám se sebou.");
        int byes = 0;
        foreach (Match m in t.matches)
        {
            if (m.Round == t.currentRound)
            {
                if (m.Player2 == null) byes++;
                if (m.Player1 == match.Player1 || m.Player2 == match.Player1 ||
                    (match.Player2 != null && (m.Player1 == match.Player2 || m.Player2 == match.Player2)))
                    throw new TournamentExeptions("Hráč již má zápas v tomto kole.");
            }
            else if (match.Player2 != null &&
                ((m.Player1 == match.Player1 && m.Player2 == match.Player2) ||
                 (m.Player1 == match.Player2 && m.Player2 == match.Player1)))
                throw new TournamentExeptions("Tito hráči již spolu hráli.");
            if (match.Player2 == null && m.Player2 == null && m.Player1 == match.Player1)
                throw new TournamentExeptions("Hráč již měl volný los.");
        }
        if (match.Player2 == null)
        {
            if (t.tournamentPlayers.Count % 2 == 0 || byes > 0)
                throw new TournamentExeptions("Volný los není v tomto kole povolen.");
            match.scoreP1 = Scoring.Bye;
            match.scoreP2 = 0;
            match.winner = match.Player1;
            match.matchFinished = true;
        }
        else if (match.matchFinished || match.scoreP1 != 0 || match.scoreP2 != 0 || match.winner != null)
            throw new TournamentExeptions("Nový zápas musí být bez výsledku.");
        t.matches.Add(match);
    }

    private void validateMatch(Match match)
    {
        Tournament t = requireOpenRound();
        if (match == null || !t.matches.Contains(match)) throw new TournamentExeptions("Vybraný zápas neexistuje.");
        if (match.Round != t.currentRound) throw new TournamentExeptions("Do minulého kola nelze zapisovat.");
        if (match.Player1 == null || !t.tournamentPlayers.Contains(match.Player1) ||
            (match.Player2 != null && !t.tournamentPlayers.Contains(match.Player2)))
            throw new PlayerExeptions("Hráč není přihlášen do turnaje.");
        if (match.Player2 == null) throw new TournamentExeptions("Volný los má výsledek automaticky.");
    }

    public void setMatchWinner(Match match, Player winner)
    {
        validateMatch(match);
        if (winner == null || (winner != match.Player1 && winner != match.Player2))
            throw new PlayerNotInThisMatchExeption();
        match.scoreP1 = winner == match.Player1 ? Scoring.Win : Scoring.Loss;
        match.scoreP2 = winner == match.Player2 ? Scoring.Win : Scoring.Loss;
        match.winner = winner;
        match.matchFinished = true;
    }

    public void setMatchWinner(Match match, string nickname)
    {
        Player? winner = playerManager.getPlayer(nickname);
        if (winner == null) throw new PlayerNotFoundExeption(nickname);
        setMatchWinner(match, winner);
    }

    public void setMatchDraw(Match match)
    {
        validateMatch(match);
        match.scoreP1 = Scoring.Draw;
        match.scoreP2 = Scoring.Draw;
        match.winner = null;
        match.matchFinished = true;
    }

    public List<Match> getMatches()
    {
        if (activeTournament == null) throw new TournamentNotExistingExeption();
        if (!activeTournament.tournamentStarted) throw new TournamentNotStartedExeption();
        return activeTournament.matches;
    }
}
