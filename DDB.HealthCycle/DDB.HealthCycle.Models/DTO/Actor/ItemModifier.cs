namespace DDB.HealthCycle.Models.DTO.Actor;

public record ItemModifier(
    string AffectedObject,
    string AffectedValue,
    int Value
);
