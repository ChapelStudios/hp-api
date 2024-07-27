using DDB.HealthCycle.DataAccess.PlayerCharacters;
using DDB.HealthCycle.Models.DTO;
using DDB.HealthCycle.Models.Enums;
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

    /// <summary>
    /// Apply damage of a given type to player character after considering defenses and temp hp.
    /// </summary>
    /// <param name="playerToAffect">ID of player that should be damaged.</param>
    /// <param name="damageType"><see cref="DamageType"/> of the attack.</param>
    /// <param name="damageAmount">Amount of base damage to deal.</param>
    /// <returns>An updated <see cref="PlayerCharacterHealthStats"/> after the damage has been applied. If the upsert fails, null is returned instead.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="playerToAffect"/> doesn't return any results.</exception>
    /// <exception cref="FormatException">Thrown if the Player data is corrupt.</exception>
    /// <exception cref="NotImplementedException">Thrown if the <paramref name="damageType"/> is invalid.</exception>
    public async Task<PlayerCharacterHealthStats?> ApplyDamageAsync(string playerToAffect, DamageType damageType, int damageAmount)
    {
        var player = await _pcRepo.GetCharacterByIdAsync(playerToAffect);

        // We don't really care if this fails because it will just use the default value of None if it doesn't exist
        _ = player.Defenses.TryGetValue(damageType, out var applicableDefense);

        // In a bigger application I might spread this logic out to a rules engine and a static dictionary of messages
        (int damageToApply, string resistanceMsg) = applicableDefense switch
        {
            DefenseType.Immunity => (0, "Immunity"),
            // This is normally rounded up in DnD
            DefenseType.Resistance => ((int)Math.Ceiling(damageAmount / 2m), "Half-Damage Resistance"),
            DefenseType.None => (damageAmount, "no defenses"),
            // In case new members are added to the enum but this statement isn't updated
            _ => throw new NotImplementedException($"DamageType {applicableDefense} has not been implemented")
        };

        _logger.LogInformation(
            "Player {playerToAffect} affected by {damageType} attack of {damageAmount} for {damageToApply} after {resistanceMsg} applied.",
            playerToAffect,
            damageType,
            damageAmount,
            damageToApply,
            resistanceMsg);

        // resolve temp hp first
        if (player.HitPoints.Temp > 0)
        {
            var applicableTempDamage =  Math.Min(damageToApply, player.HitPoints.Temp);
            damageToApply = Math.Max(0, damageToApply - applicableTempDamage);
            _logger.LogInformation("Player {playerToAffect} Temp HP decreased by {applicableTempDamage}", playerToAffect, applicableTempDamage);
            player.HitPoints.Temp -= applicableTempDamage;
        }

        var applicableDamage = Math.Min(damageToApply, player.HitPoints.Current);
        _logger.LogInformation("Player {playerToAffect} HP decreased by {applicableDamage}", playerToAffect, applicableDamage);
        player.HitPoints.Current -= applicableDamage;

        return await _pcRepo.UpsertPlayerCharacterAsync(player)
            ? player.HitPoints
            : null;
    }
}
