using DDB.HealthCycle.Models.Enums;

namespace DDB.HealthCycle.Models.DTO.Actor;

public record PlayerCharacterClassData(
    string Name,
    Die HitDiceValue,
    int ClassLevel
);
