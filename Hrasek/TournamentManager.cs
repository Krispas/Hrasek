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

        activeTournament = null;
    }
}