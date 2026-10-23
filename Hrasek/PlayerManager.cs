public class PlayerManager
{
    private List<Player> players = new List<Player>();

    public void newPlayer(string name, string nickname)
    {
        foreach (Player p in players)
        {
            if (p.nickname.ToLower() == nickname.ToLower())
            {
                throw new PlayerNickanemeAreadyTakenExeption(nickname);
            }
        }

        players.Add(new Player(name, nickname));
    }
    public void removePlayer(string nickname)
    {
        foreach (Player p in players)
        {
            if (p.nickname.ToLower() == nickname.ToLower() && p.tournamentsPlayed != 0)
            {
                throw new PlayerPlayerTournamentExeption(nickname);
            }
            if (p.nickname.ToLower() == nickname.ToLower())
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
            nicknames.Add(p.nickname);
        }
        if (nicknames.Count == 0)
        {
            throw new PlayersNotFoundExeption();
        }
        return nicknames;
    }

    public Player? getPlayer(string nickname)
    {
        Player? player = players.Find(p => p.nickname == nickname);
        return player;
    }
}