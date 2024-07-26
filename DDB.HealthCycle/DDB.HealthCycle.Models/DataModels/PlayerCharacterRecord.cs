using DDB.HealthCycle.Models.DTO;
using Newtonsoft.Json;

namespace DDB.HealthCycle.Models.DataModels;

public class PlayerCharacterRecord
{
    public string Id { get; set; }

    // This data is stringified as we don't need to index on it and it is generally lightweight.
    // This saves us on SQL usage as we only need 1 table, it's also less development time.
    // If we later wante to index some of this data we can easily add columns to this table at relatively low cost.
    public string CharacterData { get; set; } = string.Empty;

    public PlayerCharacterRecord(string id)
    {
        Id = id;
    }

    public PlayerCharacterRecord(string id, PlayerCharacter pc)
    {
        Id = id;
        CharacterData = JsonConvert.SerializeObject(pc);
    }
}
