public static class Losovani
{
    public const string VolnyLos = "VOLNY_LOS";

    public static List<(string, string)> VylosujKolo(List<string> hraci, int kolo)
    {
        List<(string, string)> dvojice = new List<(string, string)>();
        if (hraci.Count == 0) return dvojice;

        List<string> poradi = new List<string>(hraci);

        if (poradi.Count % 2 == 1)
            poradi.Add(VolnyLos);

        int n = poradi.Count;   

        if (kolo < 0 || kolo >= n - 1)
            throw new ArgumentOutOfRangeException(nameof(kolo), "Kolo je mimo rozsah.");

        for (int i = 0; i < kolo; i++)
        {
            string posledni = poradi[n - 1];
            poradi.RemoveAt(n - 1);
            poradi.Insert(1, posledni);
        }

        for (int i = 0; i < n / 2; i++)
        {
            string a = poradi[i];
            string b = poradi[n - 1 - i];

            if (a == VolnyLos)
                dvojice.Add((b, a));
            else
                dvojice.Add((a, b));
        }

        return dvojice;
    }
}
