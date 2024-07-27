using DDB.HealthCycle.DataAccess.PlayerCharacters;
using DDB.HealthCycle.Models.DTO;
using Microsoft.Extensions.Logging;

namespace DDB.HealthCycle.Logic.PcHealth;

public class PcHealthManager(
    IPlayerCharacterRepo _pcRepo,
    ILogger<PcHealthManager> _logger
) : IPcHealthManager
{
    /// <summary>
    /// Returns the Player Character's current data
    /// </summary>
    /// <param name="playerId">ID of the PC to gather</param>
    /// <returns>A <see cref="PlayerCharacter"/></returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the ID doesn't return any results.</exception>
    /// <exception cref="FormatException">Thrown if the Player data is corrupt.</exception>
    public async Task<PlayerCharacter> GetPlayerCharacterAsync(string playerId)
    {
        return await _pcRepo.GetCharacterByIdAsync(playerId);
    }

    /// <summary>
    /// Heal a player character.
    /// </summary>
    /// <param name="playerToHeal">ID of player to heal</param>
    /// <param name="amount">Amount to Heal</param>
    /// <returns>An updated <see cref="PlayerCharacterHealthStats"/> after healing has been applied. If the upsert fails, null is returned instead.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="playerToHeal"/> doesn't return any results.</exception>
    /// <exception cref="FormatException">Thrown if the Player data is corrupt.</exception>
    public async Task<PlayerCharacterHealthStats?> HealAsync(string playerToHeal, int amount)
    {
        var player = await _pcRepo.GetCharacterByIdAsync(playerToHeal);

        var missingHP = player.HitPoints.Max - player.HitPoints.Current;
        var applicableHealing = Math.Min(missingHP, amount);

        _logger.LogInformation("Healed player {playerToHeal} for {applicableHealing} HP", playerToHeal, applicableHealing);
        player.HitPoints.Current = player.HitPoints.Current + applicableHealing;

        return await _pcRepo.UpsertPlayerCharacterAsync(player)
            ? player.HitPoints
            : null;
    }

    /// <summary>
    /// Gives a player character Temp HP.
    /// </summary>
    /// <param name="playerToAffect">ID of player that should gain the Temp HP.</param>
    /// <param name="amount">Amount of Temp HP to gain.</param>
    /// <returns>An updated <see cref="PlayerCharacterHealthStats"/> after the Temp HP has been applied. If the upsert fails, null is returned instead.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="playerToAffect"/> doesn't return any results.</exception>
    /// <exception cref="FormatException">Thrown if the Player data is corrupt.</exception>
    public async Task<PlayerCharacterHealthStats?> AddTempHpAsync(string playerToAffect, int amount)
    {
        var player = await _pcRepo.GetCharacterByIdAsync(playerToAffect);

        var newTempHp = Math.Max(amount, player.HitPoints.Temp);

        _logger.LogInformation("Player {playerToAffect} Temp HP set to {newTempHp}", playerToAffect, newTempHp);
        player.HitPoints.Temp = newTempHp;

        return await _pcRepo.UpsertPlayerCharacterAsync(player)
            ? player.HitPoints
            : null;
    }
}
