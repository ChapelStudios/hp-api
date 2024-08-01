using DDB.HealthCycle.Models.DataModels;
using DDB.HealthCycle.Models.DTO.Actor;

namespace DDB.HealthCycle.DataAccess.PlayerCharacters;
public interface IPlayerCharacterRepo
{
    /// <summary>
    /// Gather a Player Character's record by their ID
    /// </summary>
    /// <param name="id">Id of the PlayerCharacter</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the ID doesn't return any results.</exception>
    Task<PlayerCharacterRecord> GetCharacterRecordByIdAsync(string id);

    /// <summary>
    /// Gather a Player Character's data by their ID
    /// </summary>
    /// <param name="id">Id of the PlayerCharacter</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the ID doesn't return any results.</exception>
    /// <exception cref="FormatException">Thrown if the JSON data cannot be deserialized.</exception>
    Task<PlayerCharacter> GetCharacterByIdAsync(string id);

    /// <summary>
    /// Either adds or updates a PlayerCharacter as appropriate.
    /// </summary>
    /// <param name="playerCharacter">Character data to update.</param>
    /// <returns>A bool letting you know if the data was saved correctly.</returns>
    Task<bool> UpsertPlayerCharacterAsync(PlayerCharacter playerCharacter);
}
