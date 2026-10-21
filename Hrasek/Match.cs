/// <summary>
/// Match class for storing information about a match. It holds 2 players thier final score, and who in this round and the round number.
/// </summary>
/// <param name="p1">Player 1</param>
/// <param name="p2">Player 2</param>
/// <param name="round">The number of  the round</param>
public class Match(Player p1, Player p2, int round)
{
    public Player Player1 { get; set; } = p1;
    public Player Player2 { get; set; } = p2;
    public int Round { get; set; } = round;
    public int scoreP1 = 0;
    public int scoreP2 = 0;
    public Player winner = null;
}