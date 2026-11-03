namespace Hrasek;

public class TournamentManager
{
    private readonly PlayerManager playerManager;
    public Tournament? activeTournament = null;

    public TournamentManager(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
    }

    public void newTournament(string name, string discipline)
    {
        if (activeTournament != null) throw new TournamentAlreadyExistsExeption();
        if (!InputValidator.Text(name, "Název", out string error) ||
            !InputValidator.Text(discipline, "Disciplína", out error))
            throw new TournamentExeptions(error);
        activeTournament = new Tournament(name.Trim(), discipline.Trim());
    }

    public void addPlayerToTournament(string nickname)
    {
        Tournament t = requireTournament();
        if (t.tournamentStarted) throw new TournamentExeptions("Do rozehraného turnaje již nelze přidávat hráče.");
        Player? player = playerManager.getPlayer(nickname);
        if (player == null) throw new PlayerNotFoundExeption(nickname);
        if (t.tournamentPlayers.Contains(player)) throw new PlayerAlreadyInTournamentExeption(nickname);
        t.tournamentPlayers.Add(player);
    }

    public void setNumberOfRounds(int rounds)
    {
        Tournament t = requireTournament();
        if (t.tournamentStarted) throw new TournamentExeptions("Po zahájení nelze měnit počet kol.");
        if (rounds < 1 || rounds > 99) throw new TournamentExeptions("Počet kol musí být 1 až 99.");
        t.numberOfRounds = rounds;
    }

    public void startTournament()
    {
        Tournament t = requireTournament();
        if (t.tournamentStarted) throw new TournamentExeptions("Turnaj již byl zahájen.");
        if (t.tournamentPlayers.Count < 2) throw new TournamentExeptions("Přidej alespoň dva hráče.");
        int max = t.tournamentPlayers.Count - 1 + t.tournamentPlayers.Count % 2;
        if (t.numberOfRounds == null || t.numberOfRounds < 1 || t.numberOfRounds > max)
            throw new TournamentExeptions($"Pro tento počet hráčů nastav 1 až {max} kol.");
        t.tournamentStarted = true;
        foreach (Player p in t.tournamentPlayers) p.tournamentsPlayed++;
        createRound();
    }

    public List<Match> getCurentRoundMatches()
    {
        Tournament t = requireStarted();
        List<Match> matches = new List<Match>();
        foreach (Match m in t.matches)
            if (m.Round == t.currentRound) matches.Add(m);
        return matches;
    }

    public bool isRoundFinished()
    {
        Tournament t = requireStarted();
        List<Match> matches = getCurentRoundMatches();
        if (matches.Count == 0) return false;
        foreach (Player player in t.tournamentPlayers)
        {
            int appearances = 0;
            foreach (Match m in matches)
            {
                if (!m.matchFinished) return false;
                if (m.Player1 == player || m.Player2 == player) appearances++;
            }
            if (appearances != 1) return false;
        }
        return true;
    }

    public void closeRound()
    {
        Tournament t = requireStarted();
        if (t.roundClosed) throw new TournamentExeptions("Kolo již bylo uzavřeno.");
        if (!isRoundFinished()) throw new TournamentExeptions("Kolo nelze uzavřít, protože některé zápasy nemají výsledek.");
        t.roundClosed = true;
        t.tournamentFinished = t.currentRound == t.numberOfRounds;
    }

    public void NewRound()
    {
        Tournament t = requireStarted();
        if (!t.roundClosed) throw new TournamentExeptions("Nejdříve uzavři aktuální kolo.");
        if (t.currentRound >= t.numberOfRounds) throw new TournamentAllRoundsFinishedExeption();
        t.currentRound++;
        t.roundClosed = false;
        createRound();
    }

    // Dočasné párování každý s každým, než tým dodá vlastní losování.
    private void createRound()
    {
        Tournament t = requireStarted();
        List<Player?> order = new List<Player?>(t.tournamentPlayers);
        if (order.Count % 2 != 0) order.Add(null);
        // První hráč zůstává na místě, ostatní se v každém kole posunou.
        for (int round = 1; round < t.currentRound; round++)
        {
            Player? last = order[order.Count - 1];
            order.RemoveAt(order.Count - 1);
            order.Insert(1, last);
        }
        MatchManager manager = new MatchManager(t, playerManager);
        for (int i = 0; i < order.Count / 2; i++)
        {
            Player? first = order[i];
            Player? second = order[order.Count - 1 - i];
            if (first == null)
            {
                first = second;
                second = null;
            }
            manager.addMatch(new Match(first!, second, t.currentRound));
        }
    }

    public void endTournament()
    {
        Tournament t = requireStarted();
        if (!t.tournamentFinished) throw new TournamentNotFinishedExeption();
        activeTournament = null;
    }

    public void endNotFinishedTournament()
    {
        requireStarted();
        activeTournament = null;
    }

    private Tournament requireTournament()
    {
        if (activeTournament == null) throw new TournamentNotExistingExeption();
        return activeTournament;
    }

    private Tournament requireStarted()
    {
        Tournament t = requireTournament();
        if (!t.tournamentStarted) throw new TournamentNotStartedExeption();
        return t;
    }
}
