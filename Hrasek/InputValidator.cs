namespace Hrasek;

public static class InputValidator
{
    public static bool Text(string? text, string label, out string error)
    {
        error = "";
        if (string.IsNullOrWhiteSpace(text)) error = label + " nesmí být prázdné.";
        else if (text.Trim().Length > 60) error = label + " může mít nejvýše 60 znaků.";
        else
        {
            foreach (char c in text)
                if (char.IsControl(c)) error = label + " nesmí obsahovat řídicí znaky.";
        }
        return error == "";
    }

    public static bool Number(string? text, int min, int max, out int value, out string error)
    {
        error = "";
        if (!int.TryParse(text, out value)) error = "Zadej platné celé číslo.";
        else if (value < min || value > max) error = $"Hodnota je mimo povolený rozsah ({min}–{max}).";
        return error == "";
    }
}
