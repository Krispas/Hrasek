using System.Data;
using Terminal.Gui;

namespace Hrasek;

public class TournamentUI
{
    private readonly PlayerManager players = new PlayerManager();
    private readonly TournamentManager tournaments;
    private readonly Label summary = new Label() { X = 0, Y = 0, Width = Dim.Fill(), Height = 3 };
    private readonly TableView matches = new TableView() { X = 25, Y = 4, Width = Dim.Fill(), Height = 7, FullRowSelect = true };
    private readonly TableView standings = new TableView() { X = 25, Y = 12, Width = Dim.Fill(), Height = Dim.Fill(1), FullRowSelect = true };
    private List<Match> shownMatches = new List<Match>();

    public TournamentUI()
    {
        tournaments = new TournamentManager(players);
    }

    public void Run()
    {
        Application.Init();
        try
        {
            Window window = new Window("Správa turnaje") { Width = Dim.Fill(), Height = Dim.Fill() };
            View content = new View() { Width = Dim.Fill(), Height = Dim.Fill(), CanFocus = true };
            Label smallScreen = new Label("Zvětši terminál alespoň na 80 × 24 znaků.\nCtrl+Q: ukončit aplikaci.")
                { Width = Dim.Fill(), Height = Dim.Fill(), Visible = false };
            window.Add(content, smallScreen);
            window.LayoutComplete += _ =>
            {
                bool enoughSpace = Application.Driver.Cols >= 80 && Application.Driver.Rows >= 24;
                content.Visible = enoughSpace;
                smallScreen.Visible = !enoughSpace;
            };
            content.Add(summary);
            AddButton(content, "Vytvořit turnaj", 4, CreateTournament);
            AddButton(content, "Přidat hráče", 5, AddPlayer);
            AddButton(content, "Seznam hráčů", 6, ShowPlayers);
            AddButton(content, "Nastavit počet kol", 7, SetRounds);
            AddButton(content, "Spustit turnaj", 8, tournaments.startTournament);
            AddButton(content, "Zadat výsledek", 10, EnterResult);
            AddButton(content, "Uzavřít kolo", 11, tournaments.closeRound);
            AddButton(content, "Další kolo", 12, tournaments.NewRound);
            AddButton(content, "Konečné pořadí", 14, FinalStandings);
            AddButton(content, "Ukončit", 16, () => Application.RequestStop());
            content.Add(new Label("Zápasy aktuálního kola (výběr šipkami):") { X = 25, Y = 3 });
            content.Add(new Label("Pořadí | pomocná kritéria: soupeři, výhry") { X = 25, Y = 11 });
            content.Add(matches, standings);
            content.Add(new Label("Tab: přechod • Šipky: výběr • Enter: tlačítko • Ctrl+Q: konec")
                { X = 0, Y = Pos.AnchorEnd(1), Width = Dim.Fill() });
            Application.Top.Add(window);
            Refresh();
            Application.Run();
        }
        finally { Application.Shutdown(); }
    }

    private void AddButton(View view, string text, int y, Action action)
    {
        Button button = new Button(text) { X = 0, Y = y };
        button.Clicked += () => Execute(action);
        view.Add(button);
    }

    private void Execute(Action action)
    {
        try { action(); }
        catch (PlayerExeptions e) { MessageBox.ErrorQuery("Chyba", e.Message, "OK"); }
        catch (TournamentExeptions e) { MessageBox.ErrorQuery("Chyba", e.Message, "OK"); }
        catch (Exception) { MessageBox.ErrorQuery("Chyba", "Operaci se nepodařilo provést. Zkontroluj stav turnaje.", "OK"); }
        Refresh();
    }

    private Tournament RequireTournament()
    {
        if (tournaments.activeTournament == null) throw new TournamentNotExistingExeption();
        return tournaments.activeTournament;
    }

    private void CreateTournament()
    {
        if (tournaments.activeTournament != null) throw new TournamentAlreadyExistsExeption();
        Form("Nový turnaj", new[] { "Název", "Disciplína", "Počet kol (1–99)" }, values =>
        {
            if (!InputValidator.Text(values[0], "Název", out string error) ||
                !InputValidator.Text(values[1], "Disciplína", out error)) return error;
            if (!InputValidator.Number(values[2], 1, 99, out int rounds, out error)) return error;
            tournaments.newTournament(values[0], values[1]);
            tournaments.setNumberOfRounds(rounds);
            return null;
        });
    }

    private void AddPlayer()
    {
        Tournament t = RequireTournament();
        if (t.tournamentStarted) throw new TournamentExeptions("Do rozehraného turnaje již nelze přidávat hráče.");
        Form("Přidat hráče", new[] { "Jméno", "Jedinečná přezdívka" }, values =>
        {
            if (!InputValidator.Text(values[0], "Jméno hráče", out string error) ||
                !InputValidator.Text(values[1], "Přezdívka", out error)) return error;
            Player? player = players.getPlayer(values[1]);
            if (player != null && t.tournamentPlayers.Contains(player)) return "Tato přezdívka již je v turnaji.";
            if (player == null) players.newPlayer(values[0], values[1]);
            tournaments.addPlayerToTournament(values[1]);
            return null;
        });
    }

    private void SetRounds()
    {
        Tournament t = RequireTournament();
        if (t.tournamentStarted) throw new TournamentExeptions("Po zahájení nelze měnit počet kol.");
        Form("Počet kol", new[] { "Počet kol (1–99)" }, values =>
        {
            if (!InputValidator.Number(values[0], 1, 99, out int rounds, out string error)) return error;
            tournaments.setNumberOfRounds(rounds);
            return null;
        });
    }

    private void ShowPlayers()
    {
        Tournament t = RequireTournament();
        DataTable table = new DataTable();
        table.Columns.Add("Jméno");
        table.Columns.Add("Přezdívka (ID)");
        foreach (Player p in t.tournamentPlayers) table.Rows.Add(p.Name, p.nickname);
        ShowTable("Hráči turnaje", table);
    }

    private void EnterResult()
    {
        Tournament t = RequireTournament();
        int index = matches.SelectedRow;
        if (index < 0 || index >= shownMatches.Count) throw new TournamentExeptions("Nejdříve vyber zápas v tabulce pomocí šipek.");
        Match match = shownMatches[index];
        if (t.roundClosed) throw new TournamentExeptions("Do uzavřeného kola nelze zapisovat.");
        if (match.Player2 == null) throw new TournamentExeptions("Volný los má výsledek automaticky.");
        Form($"{match.Player1?.nickname} × {match.Player2.nickname}", new[] { "1 = první, 2 = druhý, 0 = remíza" }, values =>
        {
            if (!InputValidator.Number(values[0], 0, 2, out int result, out string error)) return error;
            MatchManager manager = new MatchManager(t, players);
            if (result == 0) manager.setMatchDraw(match);
            else if (result == 1) manager.setMatchWinner(match, match.Player1!);
            else manager.setMatchWinner(match, match.Player2);
            return null;
        });
    }

    private void FinalStandings()
    {
        Tournament t = RequireTournament();
        if (!t.tournamentFinished) throw new TournamentNotFinishedExeption();
        ShowTable("Konečné pořadí", StandingsTable(t));
    }

    private void Form(string title, string[] labels, Func<string[], string?> submit)
    {
        using Dialog dialog = new Dialog(title, 68, labels.Length * 2 + 5);
        List<TextField> fields = new List<TextField>();
        for (int i = 0; i < labels.Length; i++)
        {
            dialog.Add(new Label(labels[i]) { X = 1, Y = i * 2 });
            TextField field = new TextField("") { X = 1, Y = i * 2 + 1, Width = Dim.Fill(2) };
            fields.Add(field);
            dialog.Add(field);
        }
        Button ok = new Button("Potvrdit") { IsDefault = true };
        ok.Clicked += () =>
        {
            try
            {
                string[] values = new string[fields.Count];
                for (int i = 0; i < fields.Count; i++) values[i] = (fields[i].Text.ToString() ?? "").Trim();
                string? error = submit(values);
                if (error != null) MessageBox.ErrorQuery("Chyba", error, "OK");
                else Application.RequestStop(dialog);
            }
            catch (PlayerExeptions e) { MessageBox.ErrorQuery("Chyba", e.Message, "OK"); }
            catch (TournamentExeptions e) { MessageBox.ErrorQuery("Chyba", e.Message, "OK"); }
            catch (Exception) { MessageBox.ErrorQuery("Chyba", "Operaci se nepodařilo provést.", "OK"); }
        };
        Button cancel = new Button("Zrušit");
        cancel.Clicked += () => Application.RequestStop(dialog);
        dialog.AddButton(ok);
        dialog.AddButton(cancel);
        fields[0].SetFocus();
        Application.Run(dialog);
    }

    private void ShowTable(string title, DataTable table)
    {
        using Dialog dialog = new Dialog(title) { Width = Dim.Percent(95), Height = Dim.Percent(90) };
        dialog.Add(new TableView(table) { Width = Dim.Fill(), Height = Dim.Fill(2), FullRowSelect = true });
        Button close = new Button("Zavřít") { IsDefault = true };
        close.Clicked += () => Application.RequestStop(dialog);
        dialog.AddButton(close);
        Application.Run(dialog);
    }

    private DataTable StandingsTable(Tournament t)
    {
        DataTable table = new DataTable();
        table.Columns.Add("Pořadí", typeof(int));
        table.Columns.Add("Hráč");
        table.Columns.Add("Body", typeof(int));
        table.Columns.Add("Odehrané zápasy", typeof(int));
        table.Columns.Add("Body soupeřů", typeof(int));
        table.Columns.Add("Výhry", typeof(int));
        int rank = 1;
        foreach (LeaderboardRow row in Leaderboard.Calculate(t))
            table.Rows.Add(rank++, row.Player.Name + " (" + row.Player.nickname + ")", row.Points, row.Played, row.OpponentPoints, row.Wins);
        return table;
    }

    private void Refresh()
    {
        Tournament? t = tournaments.activeTournament;
        shownMatches = new List<Match>();
        DataTable table = new DataTable();
        table.Columns.Add("První hráč");
        table.Columns.Add("Druhý hráč");
        table.Columns.Add("Výsledek");
        if (t == null) summary.Text = "Vítej! Vytvoř turnaj a přidej hráče.\nPotom spusť turnaj a zadávej výsledky.";
        else
        {
            string state = "Kolo otevřeno";
            if (!t.tournamentStarted) state = "Registrace";
            else if (t.tournamentFinished) state = "Dokončen";
            else if (t.roundClosed) state = "Kolo uzavřeno";
            summary.Text = $"{t.Name} • {t.Discipline} • Hráči: {t.tournamentPlayers.Count}\n{state} • Kolo {t.currentRound}/{t.numberOfRounds}";
            foreach (Match match in t.matches)
            {
                if (match.Round != t.currentRound) continue;
                shownMatches.Add(match);
                string result = match.matchFinished ? $"{match.scoreP1}:{match.scoreP2}" : "Čeká na výsledek";
                table.Rows.Add(match.Player1?.nickname, match.Player2?.nickname ?? "Volný los", result);
            }
        }
        matches.Table = table;
        standings.Table = t == null ? new DataTable() : StandingsTable(t);
    }
}
