public class Match
{
    public Player? Player1 { get; set; }
    public Player? Player2 { get; set; }
    public int Round { get; set; }
    // Jsou to turnajové body (3/1/0), nikoli například počet gólů.
    public int scoreP1 = 0;
    public int scoreP2 = 0;
    public Player? winner = null;
    public bool matchFinished = false;

    public Match(Player p1, Player? p2, int round)
    {
        Player1 = p1;
        Player2 = p2;
        Round = round;
    }
}
