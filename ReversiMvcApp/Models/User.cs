namespace ReversiMvcApp.Models;

public class User
{
    public string Guuid { get; set; }
    public string Naam { get; set; }
    public List<String> Rollen { get; set; }
}