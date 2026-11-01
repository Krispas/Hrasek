using System.Data;

namespace Hrasek;

public class TournamentManager(PlayerManager playerManager)
{
    private Tournament? activeTournament = null;

    public void newTournament(string name, string discipline)
    {
        if (activeTournament != null)
        {
            throw new TournamentAlreadyExistsExeption();
        }

        activeTournament = new Tournament(name, discipline);
        activeTournament.tournamentStarted = false;
    }

    public void addPlayer(string nickname)
    {
        Player player = playerManager.getPlayer(nickname);
        if (player == null)
        {
            throw new PlayerNotFoundExeption(nickname);
        }

        if (activeTournament == null)
        {
            throw new TournamentNotExistingExeption();
        }

        if (activeTournament.tournamentPlayers.Contains(player))
        {
            throw new PlayerAlreadyInTournamentExeption(nickname);
        }

        activeTournament.tournamentPlayers.Add(player);
    }

    public void startTournament()
    {
        if (activeTournament == null)
        {
            throw new TournamentNotExistingExeption();
        }

        activeTournament.tournamentStarted = true;
        foreach (Player p in activeTournament.tournamentPlayers)
        {
            p.tournamentsPlayed++;
        }
        //zapnout losovani
    }

    public void endNotFinishedTournament()
    {
        if (activeTournament == null)
        {
            throw new TournamentNotExistingExeption();
        }

        if (!activeTournament.tournamentStarted)
        {
            throw new TournamentNotStartedExeption();
        }

        activeTournament = null;
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

        foreach (Match m in activeTournament.matches)
        {
            if (activeTournament.currentRound == m.Round)
            {
                curentRoundMatches.Add(m);
            }
        }
        return curentRoundMatches;
    }
    public bool isRoundFinished()
    {
        List<Match> curentRoundMatches = getCurentRoundMatches();
        foreach (Match m in curentRoundMatches)
        {
            if (!m.matchFinished)
            {
                return false;
            }
        }
        return true;
    }

    public void endTournament()
        {
            if (activeTournament == null)
            {
                throw new TournamentNotExistingExeption();
            }
            if (!activeTournament.tournamentStarted)
            {
                throw new TournamentNotStartedExeption();
            }

            if (activeTournament.currentRound != activeTournament.numberOfRounds)
            {
                throw new TournamentNotFinishedExeption();
            }
            
            activeTournament = null;
    }
    public void NewRound()
    {
        if (activeTournament == null)
        {
            throw new TournamentNotExistingExeption();
        }
        if (!activeTournament.tournamentStarted)
        {
            throw new TournamentNotStartedExeption();
        }

        if (activeTournament.currentRound == activeTournament.numberOfRounds)
        {
            throw new TournamentAllRoundsFinishedExeption();
        }

        if (isRoundFinished())
        {
            activeTournament.currentRound++;
        }
        else
        {
            throw new TournamentRoundNotFinished();
        }
    }
}