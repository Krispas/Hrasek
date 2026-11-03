using Hrasek;
using Xunit;

public class TournamentTests
{
    [Theory]
    [InlineData("text")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("-1")]
    [InlineData("100")]
    [InlineData("2147483648")]
    public void InvalidNumber(string text)
    {
        Assert.False(InputValidator.Number(text, 1, 99, out _, out string error));
        Assert.NotEmpty(error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("a\nb")]
    public void InvalidName(string name)
    {
        Assert.False(InputValidator.Text(name, "Jméno hráče", out _));
        Assert.Throws<PlayerExeptions>(() => new PlayerManager().newPlayer(name, "id"));
    }

    private static (PlayerManager, TournamentManager, MatchManager) Setup(int count = 4, int rounds = 1)
    {
        PlayerManager players = new();
        TournamentManager manager = new(players);
        manager.newTournament("Školní turnaj", "Šachy");
        manager.setNumberOfRounds(rounds);
        for (int i = 0; i < count; i++)
        {
            players.newPlayer("Hráč " + i, "p" + i);
            manager.addPlayerToTournament("p" + i);
        }
        manager.startTournament();
        return (players, manager, new MatchManager(manager.activeTournament, players));
    }

    [Fact]
    public void InvalidOperationsDoNotChangeState()
    {
        var (players, manager, matches) = Setup();
        Tournament t = manager.activeTournament!;
        Match match = t.matches[0];
        Assert.Throws<TournamentExeptions>(() => matches.setMatchDraw(new Match(match.Player1!, match.Player2, 1)));
        Assert.Throws<PlayerNotFoundExeption>(() => matches.setMatchWinner(match, "není"));
        Assert.Throws<PlayerNotInThisMatchExeption>(() => matches.setMatchWinner(match, new Player("Cizí", "cizí")));
        Assert.Throws<TournamentExeptions>(() => manager.addPlayerToTournament("p0"));
        Assert.Throws<TournamentExeptions>(() => manager.startTournament());
        Assert.Throws<TournamentExeptions>(() => manager.NewRound());
        Assert.Throws<TournamentExeptions>(() => manager.closeRound());
        Assert.False(t.roundClosed);
        Assert.Equal(1, players.getPlayer("p0")!.tournamentsPlayed);
        Assert.False(match.matchFinished);
    }

    [Fact]
    public void ResultsCanBeCorrectedWithoutDuplicatingPoints()
    {
        var (_, manager, matches) = Setup(2);
        Tournament t = manager.activeTournament!;
        Match m = t.matches[0];
        matches.setMatchWinner(m, m.Player1!);
        matches.setMatchWinner(m, m.Player1!);
        var rows = Leaderboard.Calculate(t);
        Assert.Equal(3, rows[0].Points);
        Assert.Equal(1, rows[0].Played);
        Assert.Equal(1, rows[0].Wins);
        Assert.Equal(0, rows[1].Points);
        matches.setMatchDraw(m);
        foreach (var row in Leaderboard.Calculate(t))
        {
            Assert.Equal(1, row.Points);
            Assert.Equal(1, row.Played);
            Assert.Equal(0, row.Wins);
        }
        manager.closeRound();
        Assert.True(t.tournamentFinished);
        Assert.Throws<TournamentExeptions>(() => matches.setMatchDraw(m));
        Assert.Throws<TournamentExeptions>(() => manager.closeRound());
        Assert.Throws<TournamentAllRoundsFinishedExeption>(() => manager.NewRound());
    }

    [Fact]
    public void ByeDoesNotBlockClosingOrCountAsPlayed()
    {
        var (_, manager, matches) = Setup(3);
        Tournament t = manager.activeTournament!;
        Match bye = t.matches.Find(m => m.Player2 == null)!;
        Assert.True(bye.matchFinished);
        Assert.Throws<TournamentExeptions>(() => matches.setMatchDraw(bye));
        foreach (Match m in t.matches)
            if (m.Player2 != null) matches.setMatchDraw(m);
        manager.closeRound();
        var row = Leaderboard.Calculate(t).Find(r => r.Player == bye.Player1)!;
        Assert.Equal(Scoring.Bye, row.Points);
        Assert.Equal(0, row.Played);
        Assert.Equal(0, row.Wins);
        Assert.Equal(0, row.OpponentPoints);
        Assert.True(t.tournamentFinished);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void PairingHasNoRepeatedOpponentsOrByes(int count)
    {
        int rounds = count - 1 + count % 2;
        var (_, manager, matches) = Setup(count, rounds);
        Tournament t = manager.activeTournament!;
        for (int round = 1; round <= rounds; round++)
        {
            foreach (Match m in manager.getCurentRoundMatches())
                if (m.Player2 != null) matches.setMatchDraw(m);
            manager.closeRound();
            if (round < rounds) manager.NewRound();
        }
        foreach (Player player in t.tournamentPlayers)
        {
            List<Player> opponents = new();
            int byes = 0;
            foreach (Match m in t.matches)
            {
                if (m.Player1 == player && m.Player2 == null) byes++;
                else if (m.Player1 == player) opponents.Add(m.Player2!);
                else if (m.Player2 == player) opponents.Add(m.Player1!);
            }
            Assert.Equal(count - 1, opponents.Count);
            Assert.Equal(count - 1, opponents.Distinct().Count());
            Assert.Equal(count % 2, byes);
        }
    }

    [Fact]
    public void BuchholzUsesActualOpponentsPoints()
    {
        var (_, manager, matches) = Setup(4);
        Tournament t = manager.activeTournament!;
        matches.setMatchWinner(t.matches[0], t.matches[0].Player1!);
        matches.setMatchDraw(t.matches[1]);
        var rows = Leaderboard.Calculate(t);
        foreach (var row in rows)
        {
            Match match = t.matches.Find(m => m.Player1 == row.Player || m.Player2 == row.Player)!;
            Player opponent = match.Player1 == row.Player ? match.Player2! : match.Player1!;
            Assert.Equal(rows.Find(r => r.Player == opponent)!.Points, row.OpponentPoints);
        }
    }

    [Fact]
    public void TieBreaksFollowSpecifiedOrder()
    {
        LeaderboardRow a = new(new Player("Adam", "a")) { Points = 3, OpponentPoints = 5, Wins = 0 };
        LeaderboardRow b = new(new Player("Boris", "b")) { Points = 3, OpponentPoints = 4, Wins = 1 };
        Assert.True(Leaderboard.Compare(a, b) < 0);
        b.OpponentPoints = 5;
        Assert.True(Leaderboard.Compare(b, a) < 0);
        a.Wins = 1;
        Assert.True(Leaderboard.Compare(a, b) < 0);
        b.Player.Name = "Adam";
        Assert.True(Leaderboard.Compare(a, b) < 0);
        Assert.Equal(0, Leaderboard.Compare(a, a));
    }

    [Fact]
    public void TournamentsHaveSeparateStandings()
    {
        var (_, manager, matches) = Setup(2);
        matches.setMatchWinner(manager.activeTournament!.matches[0], "p0");
        var (_, other, _) = Setup(2);
        Assert.Equal(0, Leaderboard.Calculate(other.activeTournament!)[0].Points);
    }

    [Fact]
    public void ResultsBeforeStartAreRejected()
    {
        PlayerManager players = new PlayerManager();
        TournamentManager manager = new TournamentManager(players);
        manager.newTournament("Turnaj", "Šachy");
        players.newPlayer("Adam", "a");
        players.newPlayer("Boris", "b");
        manager.addPlayerToTournament("a");
        manager.addPlayerToTournament("b");
        Match match = new Match(players.getPlayer("a")!, players.getPlayer("b"), 1);
        MatchManager matches = new MatchManager(manager.activeTournament, players);

        Assert.Throws<TournamentNotStartedExeption>(() => matches.setMatchDraw(match));
        Assert.Throws<TournamentNotStartedExeption>(() => matches.setMatchWinner(match, "a"));
        Assert.False(match.matchFinished);
        Assert.Equal(0, match.scoreP1);
        Assert.Equal(0, match.scoreP2);
    }

    [Fact]
    public void EqualRowsAreSortedByNameThenNickname()
    {
        Tournament tournament = new Tournament("Turnaj", "Šachy");
        tournament.tournamentPlayers.Add(new Player("Boris", "c"));
        tournament.tournamentPlayers.Add(new Player("Adam", "b"));
        tournament.tournamentPlayers.Add(new Player("Adam", "a"));

        List<LeaderboardRow> rows = Leaderboard.Calculate(tournament);

        Assert.Equal("a", rows[0].Player.nickname);
        Assert.Equal("b", rows[1].Player.nickname);
        Assert.Equal("c", rows[2].Player.nickname);
    }
}
