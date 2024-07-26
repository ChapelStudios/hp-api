using DDB.HealthCycle.Data;
using DDB.HealthCycle.DataAccess.DateTimeProvider;
using DDB.HealthCycle.Models.DataModels;
using DDB.HealthCycle.Models.DTO;
using Microsoft.Extensions.Logging;

namespace DDB.HealthCycle.DataAccess.PlayerCharacters;

public class PlayerCharacterRepo(
    PlayerCharacterContext _pcContext,
    IDateTimeProvider _dateTimeProvider,
    ILogger<PlayerCharacterRepo> _logger
) : IPlayerCharacterRepo
{
    /// <summary>
    /// Gather a Player Character's record by their ID
    /// </summary>
    /// <param name="id">Id of the PlayerCharacter</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the ID doesn't return any results.</exception>
    public PlayerCharacterRecord GetCharacterRecordById(string id)
    {
        return _pcContext.PlayerCharacterRecords
            .First(c => c.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id), "Character not found");
    }

    /// <summary>
    /// Gather a Player Character's data by their ID
    /// </summary>
    /// <param name="id">Id of the PlayerCharacter</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the ID doesn't return any results.</exception>
    /// <exception cref="FormatException">Thrown if the JSON data cannot be deserialized.</exception>
    public PlayerCharacter GetCharacterById(string id)
    {
        var record = GetCharacterRecordById(id);
        return record.Unpack() ?? throw new FormatException("Character json corrupted");
    }

    /// <summary>
    /// Either adds or updates a PlayerCharacter as appropriate.
    /// </summary>
    /// <param name="playerCharacter">Character data to update.</param>
    /// <returns>A bool letting you know if the data was saved correctly.</returns>
    public bool UpsertPlayerCharacter(PlayerCharacter playerCharacter)
    {
        try
        {
            bool isSuccess = false;
            try
            {
                var existing = GetCharacterRecordById(playerCharacter.Id);
                existing.Pack(playerCharacter);
                existing.Updated = _dateTimeProvider.UtcNow;
            }
            catch (ArgumentOutOfRangeException)
            {
                var newPC = new PlayerCharacterRecord(playerCharacter)
                {
                    Updated = _dateTimeProvider.UtcNow
                };
                _pcContext.Add(newPC);
            }
            finally
            {
                isSuccess = _pcContext.SaveChanges() > 0;
            }
            return isSuccess;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
