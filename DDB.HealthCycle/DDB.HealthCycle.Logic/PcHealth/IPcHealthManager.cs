using DDB.HealthCycle.Models.DTO;

namespace DDB.HealthCycle.Logic.PcHealth;
public interface IPcHealthManager
{
    /// <summary>
    /// Returns the Player Character's current data
    /// </summary>
    /// <param name="playerId">ID of the PC to gather</param>
    /// <returns>A <see cref="PlayerCharacter"/></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the ID doesn't return any results.</exception>
    /// <exception cref="FormatException">Thrown if the Player data is corrupt.</exception>
    Task<PlayerCharacter> GetPlayerCharacterAsync(string playerId);


    /// <summary>
    /// Heal a player character
    /// </summary>
    /// <param name="playerToHeal">ID of player to heal</param>
    /// <param name="amount">Amount to Heal</param>
    /// <returns>An updated <see cref="PlayerCharacterHealthStats"/> after healing has been applied.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the ID doesn't return any results.</exception>
    /// <exception cref="FormatException">Thrown if the Player data is corrupt.</exception>
    Task<PlayerCharacterHealthStats?> HealAsync(string playerToHeal, int amount);
}
