using Hrasek;
//Jen for testing purposes.
PlayerManager playerManager = new PlayerManager();
MatchManager matchManager = new MatchManager(null, playerManager);
TournamentManager tournamentManager = new TournamentManager(playerManager, matchManager);

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