using DDB.HealthCycle.Models.DataModels;
using DDB.HealthCycle.Models.DTO.Actor;
using Newtonsoft.Json;

namespace DDB.HealthCycle.Data.TestData;
public static class PlayerCharacterContextInitializer
{
    public static async Task Initialize(PlayerCharacterContext pcContext)
    {
        if (pcContext.PlayerCharacterRecords.Any())
        {
            // Only seed an empty db
            return;
        }

        using StreamReader file = File.OpenText(@"../DDB.HealthCycle.Data/TestData/briv.json");
        using JsonTextReader reader = new JsonTextReader(file);
        var jsonData = await file.ReadToEndAsync();

        PlayerCharacter? briv = JsonConvert.DeserializeObject<PlayerCharacter>(jsonData)
            ?? throw new Exception("Invalid json data.");

        pcContext.Add(new PlayerCharacterRecord(briv));

        pcContext.SaveChanges();
    }
}
