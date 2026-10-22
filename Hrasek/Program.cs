using Hrasek;
//Jen for testing purposes.
PlayerManager playerManager = new PlayerManager();
TournamentManager tournamentManager = new TournamentManager(playerManager);

try {tournamentManager.addMatch();}
catch (Exception) {Console.WriteLine("Message");}