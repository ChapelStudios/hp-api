namespace DDB.HealthCycle.Models.DTO;

public record ItemModifier(
    string AffectedObject,
    string AffectedValue,
    int Value
);
