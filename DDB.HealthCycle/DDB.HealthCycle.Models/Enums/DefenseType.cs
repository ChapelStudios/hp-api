using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DDB.HealthCycle.Models.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum DefenseType
{
    None,
    Resistance,
    Immunity,
}
