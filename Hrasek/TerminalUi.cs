using Terminal.Gui;

namespace Hrasek;

internal sealed class TerminalUi
{
    private readonly PlayerManager players = new();
    private readonly TournamentManager tournaments;
    private readonly ListView sections = new(new List<string> { "Hráči", "Turnaj", "Zápasy", "Konec" });
    private readonly ListView actions = new();
    private readonly TextView details = new() { ReadOnly = true, WordWrap = true };
    private readonly Label status = new();
    private (string Name, Action Run)[] currentActions = Array.Empty<(string, Action)>();

    private TerminalUi() => tournaments = new TournamentManager(players);

    public static void Main()
    {
        Application.Init();
        try { new TerminalUi().Run(); }
        finally { Application.Shutdown(); }
    }

    private void Run()
    {
        var window = new Window("Hrasek") { Width = Dim.Fill(), Height = Dim.Fill() };
        var navigation = new FrameView("Menu") { Width = 17, Height = Dim.Fill(4) };
        sections.Width = Dim.Fill();
        sections.Height = Dim.Fill();
        navigation.Add(sections);

        var commands = new FrameView("Akce") { X = 17, Width = 26, Height = Dim.Fill(4) };
        actions.Width = Dim.Fill();
        actions.Height = Dim.Fill();
        commands.Add(actions);

        var overview = new FrameView("Přehled") { X = 43, Width = Dim.Fill(), Height = Dim.Fill(4) };
        details.Width = Dim.Fill();
        details.Height = Dim.Fill();
        overview.Add(details);

        var save = new Button("Uložit") { X = 1, Y = Pos.AnchorEnd(3) };
        var load = new Button("Načíst") { X = Pos.Right(save) + 2, Y = Pos.Top(save) };
        status.X = Pos.Right(load) + 2;
        status.Y = Pos.Top(save);
        status.Width = Dim.Fill();

        sections.SelectedItemChanged += _ => ShowSection();
        sections.OpenSelectedItem += _ => actions.SetFocus();
        sections.KeyPress += e =>
        {
            if (e.KeyEvent.Key != Key.CursorRight) return;
            actions.SetFocus();
            e.Handled = true;
        };
        actions.KeyPress += e =>
        {
            if (e.KeyEvent.Key != Key.CursorLeft) return;
            sections.SetFocus();
            e.Handled = true;
        };
        actions.OpenSelectedItem += _ => ExecuteSelectedAction();

        window.Add(navigation, commands, overview, save, load, status);
        Application.Top.Add(window);
        ShowSection();
        ShowOverview();
        sections.SetFocus();
        Application.Run();
    }

    private void ShowSection()
    {
        currentActions = sections.SelectedItem switch
        {
            0 => new (string, Action)[]
            {
                ("Seznam hráčů", ShowPlayers), ("Přidat hráče", AddPlayer),
                ("Detail hráče", PlayerDetails), ("Odebrat hráče", RemovePlayer)
            },
            1 => new (string, Action)[]
            {
                ("Detail turnaje", ShowOverview), ("Vytvořit turnaj", CreateTournament),
                ("Přihlásit hráče", RegisterPlayer), ("Seznam účastníků", ShowParticipants),
                ("Spustit turnaj", StartTournament), ("Zkontrolovat kolo", CheckRound),
                ("Další kolo", NextRound), ("Dokončit turnaj", FinishTournament),
                ("Zrušit turnaj", AbandonTournament)
            },
            2 => new (string, Action)[]
            {
                ("Zápasy aktuálního kola", () => ShowMatches(true)),
                ("Všechny zápasy", () => ShowMatches(false)),
                ("Přidat zápas", AddMatch), ("Zadat / upravit výsledek", RecordResult)
            },
            _ => new (string, Action)[] { ("Ukončit aplikaci", Quit) }
        };
        actions.SetSource(currentActions.Select(a => a.Name).ToList());
        actions.SelectedItem = 0;
    }

    private void ExecuteSelectedAction()
    {
        if (actions.SelectedItem < 0 || actions.SelectedItem >= currentActions.Length) return;
        try { currentActions[actions.SelectedItem].Run(); }
        catch (Exception ex) { MessageBox.ErrorQuery("Akci nelze dokončit", ex.Message, "OK"); }
    }

    private List<Player> AllPlayers()
    {
        try { return players.getAllPlayerNicknames().Select(n => players.getPlayer(n)!).ToList(); }
        catch (PlayersNotFoundExeption) { return new List<Player>(); }
    }

    private void ShowPlayers() => Display("Hráči", AllPlayers().Select(p =>
        $"{p.nickname} - {p.Name}\nOdehrané turnaje: {p.tournamentsPlayed}"));

    private void AddPlayer()
    {
        var input = Form("Přidat hráče", "Jméno", "Přezdívka");
        if (input == null) return;
        players.newPlayer(input[0], input[1]);
        Changed("Hráč přidán.");
        ShowPlayers();
    }

    private void PlayerDetails()
    {
        var player = Choose("Detail hráče", AllPlayers(), PlayerText);
        if (player != null) Display("Hráč", new[]
        {
            $"Jméno: {player.Name}\nPřezdívka: {player.nickname}\nOdehrané turnaje: {player.tournamentsPlayed}"
        });
    }

    private void RemovePlayer()
    {
        var player = Choose("Odebrat hráče", AllPlayers(), PlayerText);
        if (player == null) return;
        if (tournaments.activeTournament?.tournamentPlayers.Contains(player) == true)
            throw new InvalidOperationException("Tento hráč je přihlášen do aktivního turnaje.");
        if (!Confirm("Odebrat hráče", $"Odebrat hráče {player.nickname}?")) return;
        // The existing manager modifies its list during enumeration, after removing the player.
        try { players.removePlayer(player.nickname); }
        catch (InvalidOperationException) when (players.getPlayer(player.nickname) == null) { }
        Changed("Hráč odebrán.");
        ShowPlayers();
    }

    private void CreateTournament()
    {
        if (tournaments.activeTournament != null) throw new TournamentAlreadyExistsExeption();
        var input = Form("Vytvořit turnaj", "Název", "Disciplína", "Počet kol");
        if (input == null) return;
        if (!int.TryParse(input[2], out var rounds) || rounds < 1)
            throw new InvalidOperationException("Zadejte počet kol jako kladné celé číslo.");
        tournaments.newTournament(input[0], input[1]);
        ActiveTournament().numberOfRounds = rounds;
        Changed("Turnaj vytvořen.");
    }

    private void RegisterPlayer()
    {
        var tournament = ActiveTournament();
        if (tournament.tournamentStarted) throw new InvalidOperationException("Přihlašování je uzavřeno.");
        var player = Choose("Přihlásit hráče", AllPlayers().Where(p => !tournament.tournamentPlayers.Contains(p)).ToList(), PlayerText);
        if (player == null) return;
        tournaments.addPlayerToTournament(player.nickname);
        Changed("Hráč přihlášen.");
        ShowParticipants();
    }

    private void ShowParticipants() => Display("Účastníci", ActiveTournament().tournamentPlayers.Select(PlayerText));

    private void StartTournament()
    {
        var tournament = ActiveTournament();
        if (tournament.tournamentStarted) throw new InvalidOperationException("Turnaj již začal.");
        if (tournament.tournamentPlayers.Count < 2) throw new InvalidOperationException("Nejprve přihlaste alespoň dva hráče.");
        tournaments.startTournament();
        Changed("Turnaj spuštěn. Přidejte zápasy kola.");
    }

    private void CheckRound()
    {
        RequireRoundMatches();
        MessageBox.Query("Stav kola", tournaments.isRoundFinished()
            ? "Všechny zadané zápasy tohoto kola jsou dokončeny." : "U některých zápasů ještě chybí výsledek.", "OK");
    }

    private void NextRound()
    {
        RequireRoundMatches();
        tournaments.NewRound();
        Changed("Další kolo zahájeno.");
    }

    private void FinishTournament()
    {
        RequireRoundMatches();
        if (!tournaments.isRoundFinished()) throw new TournamentRoundNotFinished();
        if (ActiveTournament().currentRound != ActiveTournament().numberOfRounds) throw new TournamentNotFinishedExeption();
        if (!Confirm("Dokončit turnaj", "Dokončit a vymazat aktivní turnaj?")) return;
        tournaments.endTournament();
        Changed("Turnaj dokončen.");
    }

    private void AbandonTournament()
    {
        StartedTournament();
        if (!Confirm("Zrušit turnaj", "Zahodit tento turnaj a jeho výsledky?")) return;
        tournaments.endNotFinishedTournament();
        Changed("Turnaj zrušen.");
    }

    private void ShowMatches(bool currentRound)
    {
        var tournament = StartedTournament();
        var matches = currentRound ? tournaments.getCurentRoundMatches() : new MatchManager(tournament, players).getMatches();
        Display(currentRound ? $"Kolo {tournament.currentRound}" : "Všechny zápasy", matches.Select(MatchText));
    }

    private void AddMatch()
    {
        var tournament = StartedTournament();
        var available = tournament.tournamentPlayers.Where(p => !tournaments.getCurentRoundMatches()
            .Any(m => m.Player1 == p || m.Player2 == p)).ToList();
        if (available.Count < 2) throw new InvalidOperationException("Pro toto kolo jsou potřeba alespoň dva hráči bez soupeře.");
        var first = Choose("První hráč", available, PlayerText);
        if (first == null) return;
        var second = Choose("Soupeř", available.Where(p => p != first && !tournament.matches.Any(m =>
            (m.Player1 == first && m.Player2 == p) || (m.Player2 == first && m.Player1 == p))).ToList(), PlayerText);
        if (second == null) return;
        new MatchManager(tournament, players).addMatch(new Match(first, second, tournament.currentRound));
        Changed("Zápas přidán.");
        ShowMatches(true);
    }

    private void RecordResult()
    {
        var tournament = StartedTournament();
        var match = Choose("Vyberte zápas aktuálního kola", tournaments.getCurentRoundMatches(), MatchText);
        if (match == null) return;
        var result = Choose("Výsledek", new List<string>
        {
            $"Výhra: {match.Player1!.nickname}", "Remíza", $"Výhra: {match.Player2!.nickname}"
        }, s => s);
        if (result == null) return;
        var manager = new MatchManager(tournament, players);
        if (result == "Remíza")
        {
            manager.setMatchDraw(match);
            match.winner = null;
        }
        else manager.setMatchWinner(match, result == $"Výhra: {match.Player1.nickname}" ? match.Player1 : match.Player2!);
        Changed("Výsledek uložen.");
        ShowMatches(true);
    }

    private Tournament ActiveTournament() => tournaments.activeTournament ?? throw new TournamentNotExistingExeption();

    private Tournament StartedTournament()
    {
        var tournament = ActiveTournament();
        if (!tournament.tournamentStarted) throw new TournamentNotStartedExeption();
        return tournament;
    }

    private void RequireRoundMatches()
    {
        StartedTournament();
        if (tournaments.getCurentRoundMatches().Count == 0)
            throw new InvalidOperationException("Nejprve přidejte zápasy do tohoto kola.");
    }

    private void Changed(string message)
    {
        status.Text = message;
        ShowOverview();
    }

    private void ShowOverview()
    {
        var tournament = tournaments.activeTournament;
        var lines = new List<string> { $"Hráči: {AllPlayers().Count}" };
        if (tournament == null) lines.Add("Žádný aktivní turnaj.\n\nPřidejte hráče, vytvořte turnaj a přihlaste účastníky.");
        else
        {
            lines.Add($"{tournament.Name}\nDisciplína: {tournament.Discipline}\nStav: {(tournament.tournamentStarted ? "Probíhá" : "Přihlašování otevřeno")}\nKolo: {tournament.currentRound} / {tournament.numberOfRounds}\nÚčastníci: {tournament.tournamentPlayers.Count}");
            if (tournament.tournamentStarted)
            {
                var matches = tournaments.getCurentRoundMatches();
                lines.Add($"Aktuální kolo:\nDokončené zápasy: {matches.Count(m => m.matchFinished)} / {matches.Count}");
                lines.Add("Ručně přidejte zápasy, zadejte výsledky a přejděte do dalšího kola.");
            }
        }
        lines.Add("Výhra: 3 body\nRemíza: 1 bod pro každého\nProhra: 0 bodů");
        Display("Přehled", lines);
    }

    private void Display(string title, IEnumerable<string> rows)
    {
        var values = rows.ToList();
        details.Text = title + "\n\n" + (values.Count == 0 ? "Žádné položky." : string.Join("\n\n", values));
        details.MoveHome();
    }

    private static string PlayerText(Player player) => $"{player.nickname} - {player.Name}";

    private static string MatchText(Match match) => $"Kolo {match.Round}: {match.Player1?.nickname} proti {match.Player2?.nickname}\n" +
        (match.matchFinished ? $"Body: {match.scoreP1} : {match.scoreP2}" : "Čeká na výsledek");

    private static T? Choose<T>(string title, List<T> options, Func<T, string> label) where T : class
    {
        if (options.Count == 0) throw new InvalidOperationException("Žádné dostupné položky.");
        T? result = null;
        var select = new Button("Vybrat") { IsDefault = true };
        var cancel = new Button("Zrušit");
        using var dialog = new Dialog(title, 68, 17, select, cancel);
        var list = new ListView(options.Select(o => label(o).Replace("\n", " | ")).ToList())
        {
            Width = Dim.Fill(), Height = Dim.Fill(2)
        };
        void Accept()
        {
            if (list.SelectedItem < 0 || list.SelectedItem >= options.Count) return;
            result = options[list.SelectedItem];
            Application.RequestStop();
        }
        select.Clicked += Accept;
        list.OpenSelectedItem += _ => Accept();
        cancel.Clicked += () => Application.RequestStop();
        dialog.Add(list);
        list.SetFocus();
        Application.Run(dialog);
        return result;
    }

    private static string[]? Form(string title, params string[] labels)
    {
        string[]? result = null;
        var submit = new Button("OK") { IsDefault = true };
        var cancel = new Button("Zrušit");
        using var dialog = new Dialog(title, 64, labels.Length * 3 + 5, submit, cancel);
        var fields = new List<TextField>();
        for (var i = 0; i < labels.Length; i++)
        {
            var field = new TextField("") { X = 1, Y = i * 3 + 1, Width = Dim.Fill(2) };
            fields.Add(field);
            dialog.Add(new Label(labels[i]) { X = 1, Y = i * 3 }, field);
        }
        submit.Clicked += () =>
        {
            var values = fields.Select(f => f.Text.ToString()!.Trim()).ToArray();
            if (values.Any(string.IsNullOrWhiteSpace))
            {
                MessageBox.ErrorQuery("Chybějící údaje", "Vyplňte všechna pole.", "OK");
                return;
            }
            result = values;
            Application.RequestStop();
        };
        cancel.Clicked += () => Application.RequestStop();
        fields[0].SetFocus();
        Application.Run(dialog);
        return result;
    }

    private static bool Confirm(string title, string message) => MessageBox.Query(title, message, "Zrušit", "Potvrdit") == 1;

    private static void Quit()
    {
        if (Confirm("Ukončit aplikaci", "Ukončit aplikaci? Všechna data v paměti budou ztracena.")) Application.RequestStop();
    }
}
