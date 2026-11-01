using Hrasek;
//Jen for testing purposes.
PlayerManager playerManager = new PlayerManager();
TournamentManager tournamentManager = new TournamentManager(playerManager);
MatchManager matchManager = new MatchManager(tournamentManager.activeTournament, playerManager);

try
{
    tournamentManager.addPlayerToTournament("test");
}
catch (PlayerExeptions e)
{
    Console.WriteLine(e.Message);
}
catch (TournamentExeptions e)
{
    Console.WriteLine(e.Message);
}
catch (Exception)
{
    Console.WriteLine("Neočekávaná chyba");
}