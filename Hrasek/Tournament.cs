namespace Hrasek;

public class Tournament(string name, string discipline)
{
    public string Name { get; set; } = name;
    public string Discipline { get; set; } = discipline;
    public List<Player> tournamentPlayers = new List<Player>();
    public bool tournamentStarted = false;
    public List<Match> matches = new();
    public int currentRound = 1;
}