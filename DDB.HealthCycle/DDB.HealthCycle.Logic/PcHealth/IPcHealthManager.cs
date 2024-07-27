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

    /// <summary>
    /// Gives a player character Temp HP.
    /// </summary>
    /// <param name="playerToAffect">ID of player that should gain the Temp HP.</param>
    /// <param name="amount">Amount of Temp HP to gain.</param>
    /// <returns>An updated <see cref="PlayerCharacterHealthStats"/> after the Temp HP has been applied. If the upsert fails, null is returned instead.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="playerToAffect"/> doesn't return any results.</exception>
    /// <exception cref="FormatException">Thrown if the Player data is corrupt.</exception>
    Task<PlayerCharacterHealthStats?> AddTempHpAsync(string playerToAffect, int amount);
}
