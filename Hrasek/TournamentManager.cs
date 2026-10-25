using System.Data;

namespace Hrasek;

public class TournamentManager(PlayerManager playerManager)
{ 
    private Tournament? activeTournament = null;
    
    public void newTournament(string name, string discipline, int numberOfRounds)
    {
        bool tournamentStarted = false;
        if (activeTournament != null)
        {
            throw new TournamentAlreadyExistsExeption();
        }
        activeTournament = new Tournament(name, discipline, numberOfRounds);
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
        //Pridat vsem played tournamet
        //zapnout losovani
    }
    public void addMatch()
    {
        if (activeTournament == null)
        {
            throw new TournamentNotExistingExeption();
        }

        if (!activeTournament.tournamentStarted)
        {
            throw new TournamentNotStartedExeption();
        }
    }
    public void getMatches()
    {
        if (activeTournament == null)
        {
            throw new TournamentNotExistingExeption();
        }
        if (!activeTournament.tournamentStarted)
        {
            throw new TournamentNotStartedExeption();
        }
    }
}