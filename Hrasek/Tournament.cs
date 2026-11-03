namespace Hrasek;

public class Tournament
{
    public string Name { get; set; }
    public string Discipline { get; set; }
    public List<Player> tournamentPlayers = new List<Player>();
    public bool tournamentStarted = false;
    public List<Match> matches = new List<Match>();
    public int? numberOfRounds = null;
    public int currentRound = 1;
    public bool roundClosed = false;
    public bool tournamentFinished = false;

    public Tournament(string name, string discipline)
    {
        Name = name;
        Discipline = discipline;
    }
}
