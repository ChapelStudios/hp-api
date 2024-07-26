using DDB.HealthCycle.Models.Enums;

namespace DDB.HealthCycle.Models.DTO;

public record DamageTypeDefense(
    DamageType Type,
    DefenseType Defense
);
