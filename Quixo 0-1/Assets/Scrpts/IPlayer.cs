using Fusion;

public interface IPlayer
{
    char piece { get; set; }
    bool won { get; set; }
    void Initialize(char playerSymbol);
}