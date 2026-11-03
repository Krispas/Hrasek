public class PlayerManager
{
    private List<Player> players = new List<Player>();

    public void newPlayer(string name, string nickname) //ui
    {
        if (!Hrasek.InputValidator.Text(name, "Jméno hráče", out string error) ||
            !Hrasek.InputValidator.Text(nickname, "Přezdívka", out error))
            throw new PlayerExeptions(error);
        name = name.Trim();
        nickname = nickname.Trim();
        foreach (Player p in players)
        {
            if (p.nickname.ToLower() == nickname.ToLower())
            {
                throw new PlayerNickanemeAreadyTakenExeption(nickname);
            }
        }

        players.Add(new Player(name, nickname));
    }
    public void removePlayer(string nickname) //ui
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
                return;
            }
        }
    }
    public List<string> getAllPlayerNicknames() //muzes pouzit v ui
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
        foreach (Player player in players)
        {
            if (string.Equals(player.nickname, nickname?.Trim(), StringComparison.OrdinalIgnoreCase))
                return player;
        }
        return null;
    }
}
