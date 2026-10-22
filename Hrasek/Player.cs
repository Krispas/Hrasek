/// <summary>
/// Player class for storing information about a player. It holds the player's name, nickname which is their primary identificator, and how many tournaments they have played.'
/// </summary>
/// <param name="name">Players Name</param>
/// <param name="nickname">Players nickname / their identificator - must be unique</param>
public class Player(string name, string nickname)
{
    public string Name { get; set; } = name;
    public string nickname { get; set; } = nickname;
    public int tournamentsPlayed = 0;
}
