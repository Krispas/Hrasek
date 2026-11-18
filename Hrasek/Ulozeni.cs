using System;
using System.Collections.Generic;
using System.IO;

namespace Hrasek;

public class Ulozeni
{
    private string cesta = "turnaj.txt";

    public bool Ulozit(PlayerManager pm, TournamentManager tm, out string chyba)
    {
        chyba = "";
        try
        {
            using (StreamWriter sw = new StreamWriter(cesta))
            {
                sw.WriteLine("PLAYERS");
                foreach (string nick in dejnicky(pm))
                {
                    Player p = pm.getPlayer(nick);
                    if (p != null) sw.WriteLine(p.Name.Replace(";", ",") + ";" + p.nickname.Replace(";", ",") + ";" + p.tournamentsPlayed);
                }

                Tournament t = tm.activeTournament;
                if (t != null)
                {
                    sw.WriteLine("TOURNAMENT");
                    sw.WriteLine(t.Name.Replace(";", ",") + ";" + t.Discipline.Replace(";", ",") + ";" + t.tournamentStarted + ";" + t.currentRound + ";" + t.numberOfRounds);

                    sw.WriteLine("TOURNAMENT_PLAYERS");
                    foreach (Player tp in t.tournamentPlayers) sw.WriteLine(tp.nickname);

                    sw.WriteLine("MATCHES");
                    foreach (Match m in t.matches)
                    {
                        string p1 = m.Player1 != null ? m.Player1.nickname : "";
                        string p2 = m.Player2 != null ? m.Player2.nickname : "";
                        string vitez = m.winner != null ? m.winner.nickname : "";
                        sw.WriteLine(p1 + ";" + p2 + ";" + m.Round + ";" + m.scoreP1 + ";" + m.scoreP2 + ";" + m.matchFinished + ";" + vitez);
                    }
                }
            }
            return true;
        }
        catch
        {
            chyba = "Nepodařilo se uložit";
            return false;
        }
    }

    public bool Nacist(PlayerManager pm, TournamentManager tm, out string chyba)
    {
        if (!File.Exists(cesta))
        {
            chyba = "soubor neexistuje";
            return false;
        }

        chyba = "soubor je poškozený";
        try
        {
            string[] radky = File.ReadAllLines(cesta);
            if (radky.Length == 0 || radky[0].Trim() != "PLAYERS") return false;

            List<Player> hraci = new List<Player>();
            Tournament turnaj = null;
            string sekce = "";

            foreach (string radek in radky)
            {
                string r = radek.Trim();
                if (r == "") continue;
                if (r == "PLAYERS" || r == "TOURNAMENT" || r == "TOURNAMENT_PLAYERS" || r == "MATCHES")
                {
                    sekce = r;
                    continue;
                }

                string[] data = r.Split(';');
                if (sekce == "PLAYERS")
                {
                    Player novy = new Player(data[0], data[1]);
                    novy.tournamentsPlayed = int.Parse(data[2]);
                    hraci.Add(novy);
                }
                else if (sekce == "TOURNAMENT")
                {
                    turnaj = new Tournament(data[0], data[1]);
                    turnaj.tournamentStarted = bool.Parse(data[2]);
                    turnaj.currentRound = int.Parse(data[3]);
                    if (data[4] != "") turnaj.numberOfRounds = int.Parse(data[4]);
                }
                else if (sekce == "TOURNAMENT_PLAYERS")
                {
                    Player hrac = najdi(hraci, data[0]);
                    if (hrac == null) return false;
                    turnaj.tournamentPlayers.Add(hrac);
                }
                else if (sekce == "MATCHES")
                {
                    Player p1 = najdi(hraci, data[0]);
                    Player p2 = data[1] == "" ? null : najdi(hraci, data[1]);
                    if (p1 == null) return false;

                    Match zapas = new Match(p1, p2, int.Parse(data[2]));
                    zapas.scoreP1 = int.Parse(data[3]);
                    zapas.scoreP2 = int.Parse(data[4]);
                    zapas.matchFinished = bool.Parse(data[5]);
                    zapas.winner = data[6] == "" ? null : najdi(hraci, data[6]);
                    turnaj.matches.Add(zapas);
                }
            }

            List<string> stare = dejnicky(pm);
            foreach (Player h in hraci)
            {
                if (!stare.Contains(h.nickname)) pm.newPlayer(h.Name, h.nickname);
                Player p = pm.getPlayer(h.nickname);
                p.Name = h.Name;
                p.tournamentsPlayed = h.tournamentsPlayed;
            }

            tm.activeTournament = turnaj;
            chyba = "";
            return true;
        }
        catch
        {
            return false;
        }
    }

    private List<string> dejnicky(PlayerManager pm)
    {
        try { return pm.getAllPlayerNicknames(); }
        catch (PlayersNotFoundExeption) { return new List<string>(); }
    }

    private Player najdi(List<Player> seznam, string nick)
    {
        foreach (Player p in seznam) if (p.nickname == nick) return p;
        return null;
    }
}