namespace Hrasek;

public static class Scoring
{
    public const int Win = 3;
    public const int Draw = 1;
    public const int Loss = 0;
    // Specifikace body za volný los neurčuje; tým musí tuto hodnotu schválit.
    public const int Bye = 3;
}

public class LeaderboardRow
{
    public Player Player { get; }
    public int Points { get; set; }
    public int Played { get; set; }
    public int OpponentPoints { get; set; }
    public int Wins { get; set; }

    public LeaderboardRow(Player player)
    {
        Player = player;
    }
}

public static class Leaderboard
{
    public static List<LeaderboardRow> Calculate(Tournament tournament)
    {
        List<LeaderboardRow> rows = new List<LeaderboardRow>();
        foreach (Player player in tournament.tournamentPlayers) rows.Add(new LeaderboardRow(player));
        foreach (Match match in tournament.matches)
        {
            if (!match.matchFinished) continue;
            LeaderboardRow? first = FindRow(rows, match.Player1);
            LeaderboardRow? second = FindRow(rows, match.Player2);
            if (first == null) continue;
            if (match.Player2 == null)
            {
                first.Points += Scoring.Bye;
                continue;
            }
            if (second == null) continue;
            first.Points += match.scoreP1;
            second.Points += match.scoreP2;
            first.Played++;
            second.Played++;
            if (match.winner == first.Player) first.Wins++;
            if (match.winner == second.Player) second.Wins++;
        }
        foreach (Match match in tournament.matches)
        {
            if (!match.matchFinished || match.Player2 == null) continue;
            LeaderboardRow? first = FindRow(rows, match.Player1);
            LeaderboardRow? second = FindRow(rows, match.Player2);
            if (first == null || second == null) continue;
            first.OpponentPoints += second.Points;
            second.OpponentPoints += first.Points;
        }
        rows.Sort(Compare);
        return rows;
    }

    private static LeaderboardRow? FindRow(List<LeaderboardRow> rows, Player? player)
    {
        foreach (LeaderboardRow row in rows)
        {
            if (row.Player == player) return row;
        }
        return null;
    }

    public static int Compare(LeaderboardRow a, LeaderboardRow b)
    {
        int result = b.Points.CompareTo(a.Points);
        if (result == 0) result = b.OpponentPoints.CompareTo(a.OpponentPoints);
        if (result == 0) result = b.Wins.CompareTo(a.Wins);
        if (result == 0) result = string.Compare(a.Player.Name, b.Player.Name, StringComparison.CurrentCulture);
        if (result == 0) result = string.Compare(a.Player.nickname, b.Player.nickname, StringComparison.Ordinal);
        return result;
    }
}
