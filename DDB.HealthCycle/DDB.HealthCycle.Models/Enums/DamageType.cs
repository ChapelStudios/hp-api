using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DDB.HealthCycle.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum DamageType
{
    None,
    Bludgeoning,
    Piercing,
    Slashing,
    Fire,
    Cold,
    Acid,
    Thunder,
    Lightning,
    Poison,
    Radiant,
    Necrotic,
    Psychic,
    Force,
}
