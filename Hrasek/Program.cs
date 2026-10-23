using Hrasek;
//Jen for testing purposes.
PlayerManager playerManager = new PlayerManager();
TournamentManager tournamentManager = new TournamentManager(playerManager);

try
{
    tournamentManager.addPlayer("test");
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