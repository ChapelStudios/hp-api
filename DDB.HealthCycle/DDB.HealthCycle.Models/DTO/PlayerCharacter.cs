using DDB.HealthCycle.Models.Enums;

namespace DDB.HealthCycle.Models.DTO;

public class PlayerCharacter(string id, int maxHP, PlayerCharacterAbilities abilities)
{
    public string Name { get; set; } = string.Empty;

    public string Id { get; set; } = id ?? Guid.NewGuid().ToString();

    // All PlayerCharacters start at level 1
    public int Level { get; set; } = 1;

    public PlayerCharacterHealthStats HitPoints { get; set; } = new(maxHP);

    public List<PlayerCharacterClassData> Classes { get; set; } = [];

    public PlayerCharacterAbilities Stats { get; set; } = abilities;

    public Dictionary<DamageType, DefenseType> Defenses { get; set; } = [];
}
