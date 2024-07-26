namespace DDB.HealthCycle.Models.DTO;

public class PlayerCharacterHealthStats(int max)
{
    public int Current { get; set; } = max;
    public int Max { get; set; } = max;
    public int Temp { get; set; } = 0;
    public int NonLeathal { get; set; } = 0;
}
