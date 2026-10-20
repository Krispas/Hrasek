public class PlayerManager
{
    private List<Player> players = new List<Player>();

    public void newPlayer(string name, string nickname)
    {
        foreach (Player p in players)
        {
            if (p.Nickname.ToLower() == nickname.ToLower())
            {
                throw new Exception("Nickname already taken");
            }
        }

        players.Add(new Player(name, nickname));
    }
    public void removePlayer(string nickname)
    {
        foreach (Player p in players)
        {
            if (p.Nickname.ToLower() == nickname.ToLower() && p.TournamentsPlayed != 0)
            {
                throw new Exception("Player has played tournaments");
            }
            if (p.Nickname.ToLower() == nickname.ToLower())
            {
                players.Remove(p);
            }
        }
    }
    public List<string> getAllPlayerNicknames()
    {
        List<string> nicknames = new List<string>();
        foreach (Player p in players)
        {
            nicknames.Add(p.Nickname);
        }
        return nicknames;
    }
}