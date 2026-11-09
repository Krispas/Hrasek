public static class LosovaniOvereni
{
    public static List<string> Over(int pocet)
    {
        List<string> chyby = new List<string>();

        List<string> hraci = new List<string>();
        for (int i = 1; i <= pocet; i++)
            hraci.Add("H" + i);

        int pocetKol = pocet % 2 == 0 ? pocet - 1 : pocet;

        HashSet<string> odehrane = new HashSet<string>();
        Dictionary<string, int> volneLosy = new Dictionary<string, int>();

        for (int kolo = 0; kolo < pocetKol; kolo++)
        {
            HashSet<string> vKole = new HashSet<string>();

            foreach ((string a, string b) in Losovani.VylosujKolo(hraci, kolo))
            {
                if (!vKole.Add(a))
                    chyby.Add($"Kolo {kolo + 1}: {a} hraje dvakrát");

                if (b == Losovani.VolnyLos)
                {
                    volneLosy[a] = volneLosy.GetValueOrDefault(a) + 1;
                    continue;
                }

                if (!vKole.Add(b))
                    chyby.Add($"Kolo {kolo + 1}: {b} hraje dvakrát");

              string klic = string.CompareOrdinal(a, b) < 0 ? a + "-" + b : b + "-" + a;
                if (!odehrane.Add(klic))
                    chyby.Add($"Kolo {kolo + 1}: dvojice {klic} se opakuje");
            }
        }

        int ocekavano = pocet * (pocet - 1) / 2;
        if (odehrane.Count != ocekavano)
            chyby.Add($"Odehráno {odehrane.Count} dvojic, mělo být {ocekavano}");

        if (pocet % 2 == 1)
        {
            foreach (string hrac in hraci)
                if (volneLosy.GetValueOrDefault(hrac) != 1)
                    chyby.Add($"{hrac} má volný los {volneLosy.GetValueOrDefault(hrac)}x, má mít 1x");
        }

        return chyby;
    }
}
