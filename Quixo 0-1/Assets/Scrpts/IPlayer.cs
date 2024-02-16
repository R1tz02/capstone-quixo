using Fusion;

public interface IPlayer
{
    char piece { get; set; }
    bool won { get; set; }
    int PlayerNumber { get; set; }
    void Initialize(char playerSymbol);
}