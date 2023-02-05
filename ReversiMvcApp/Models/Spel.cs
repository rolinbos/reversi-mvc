namespace ReversiMvcApp.Models;

public class Spel
{
    public int Id { get; set; }
    public string omschrijving { get; set; }
    public string token { get; set; }
    public int aanDeBeurt { get; set; }
    public string speler1Token { get; set; }
    public string speler2Token { get; set; }
    public bool speler1Read { get; set; }
    public bool speler2Read { get; set; }
    public Dictionary<string, int> bord { get; set; }

    public string beurt
    {
        get => aanDeBeurt == 1 ? speler1Token : speler2Token;
    }
}