namespace DDB.HealthCycle.Models.DTO;

public class PlayerCharacter(int maxHP, PlayerCharacterAbilities abilities)
{
    public string Name { get; set; } = string.Empty;

    // All PlayerCharacters start at level 1
    public int Level { get; set; } = 1;

    public PlayerCharacterHealthStats HitPoints { get; set; } = new(maxHP);

    public List<PlayerCharacterClassData> Classes { get; set; } = [];

    public PlayerCharacterAbilities Stats { get; set; } = abilities;

    public List<DamageTypeDefense> Defenses { get; set; } = [];

    // JSON Converter requires a parameterless constructor
}
