namespace Hrasek;

public class Tournament(string name, string discipline, int numberOfRounds)
{
    public string Name { get; set; } = name;
    public string Discipline { get; set; } = discipline;
    public int NumberOfRounds { get; set; } = numberOfRounds;
    public List<Player> tournamentPlayers = new List<Player>();
    public bool tournamentStarted = false;
    public List<Match> matches = new();
}