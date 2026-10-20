namespace Hrasek;

public class TournamentManager(PlayerManager playerManager)
{ 
    private Tournament? activeTournament = null;
    
    public void newTournament(string name, string discipline, int numberOfRounds)
    {
        if (activeTournament != null)
        {
            throw new Exception("Tournament already exists");
        }
        activeTournament = new Tournament(name, discipline, numberOfRounds);
    }
    public void addPlayer(string nickname)
    {
        Player player = playerManager.getPlayer(nickname);
        if (player == null)
        {
            throw new Exception("Player not found");
        }
        if (activeTournament == null)
        {
            throw new Exception("No tournament exists");
        }
        activeTournament.tournamentPlayers.Add(player);
    }

    public void startTournament()
    {
    }
}