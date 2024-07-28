using DDB.HealthCycle.Logic.PcHealth;
using DDB.HealthCycle.Models.DTO;
using DDB.HealthCycle.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DDB.HealthCycle.Server.Controllers;
[ApiController]
[Route("[controller]")]
public class PlayerCharacterController(ILogger<PlayerCharacterController> _logger, IPcHealthManager _pcHealthManager) : ControllerBase
{
    private const string _unableToSaveErrorMessage = "Unable to save updated character data, please try again";

    /// <summary>
    /// Gets the Json data for a Player Character.
    /// </summary>
    /// <param name="id">The id of the player character.</param>
    /// <response code="200">Payload of <see cref="PlayerCharacter"/> with spacific id.</response>
    /// <response code="204"><see cref="PlayerCharacter"/> not found.</response>
    /// <response code="500">Internal error.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PlayerCharacter), 200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetPlayerCharacter(string id)
    {
        try
        {
            // This should theoretically check and fail out of the requesting user dosn't have access to the PC
            // but there is no user data for this expirement.

            return Ok(await _pcHealthManager.GetPlayerCharacterAsync(id));
        }
        catch (ArgumentOutOfRangeException)
        {
            return StatusCode(204);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while gathering PlayerCharacter data for Id {id}", id);
            return StatusCode(500, "Unable to gather the character data, please try again");
        }
    }

    /// <summary>
    /// Heals the specified PlayerCharacter by a given amount.
    /// </summary>
    /// <param name="id">ID of the PlayerCharacter to heal.</param>
    /// <param name="amount">Number of hit points to heal. Must be a positive number. Comes from Query String.</param>
    /// <returns>The updated <see cref="PlayerCharacterHealthStats"/> after applying the healing.</returns>
    /// <response code="200">Payload of resulting <see cref="PlayerCharacterHealthStats"/> after applying the healing.</response>
    /// <response code="204">PlayerCharacter not found.</response>
    /// <response code="400"><paramref name="amount"/> is too low.</response>
    /// <response code="500">Internal error.</response>
    [HttpPost("{id}/heal")]
    [ProducesResponseType(typeof(PlayerCharacterHealthStats), 200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Heal(string id, [FromQuery]int amount)
    {
        if (amount <= 0)
        {
            return StatusCode(400, "Healing amounts much be positive numbers");
        }

        // Again we should theoretically check and fail out of the requesting user dosn't have access to modify the PC
        // but there is no user data for this expirement.

        try
        {
            var result = await _pcHealthManager.HealAsync(id, amount);

            return result == null
                ? StatusCode(500, _unableToSaveErrorMessage)
                : Ok(result);
        }
        catch (ArgumentOutOfRangeException)
        {
            return StatusCode(204);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while attempting to heal PlayerCharacter data for Id {id}", id);
            return StatusCode(500, _unableToSaveErrorMessage);
        }
    }

    /// <summary>
    /// Applies a given amount of Temp HP to the specified PlayerCharacter.
    /// </summary>
    /// <param name="id">ID of the PlayerCharacter to apply Temp HP on.</param>
    /// <param name="amount">Number of Temp hit points to apply. Must be a positive number. Comes from Query String.</param>
    /// <returns>The updated <see cref="PlayerCharacterHealthStats"/> after applying the Temp HP.</returns>
    /// <response code="200">Payload of resulting <see cref="PlayerCharacterHealthStats"/> after applying the Temp HP.</response>
    /// <response code="204">PlayerCharacter not found.</response>
    /// <response code="400"><paramref name="amount"/> is too low.</response>
    /// <response code="500">Internal error.</response>
    [HttpPost("{id}/add-temp-hp")]
    [ProducesResponseType(typeof(PlayerCharacterHealthStats), 200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> AddTempHp(string id, [FromQuery] int amount)
    {
        if (amount <= 0)
        {
            return StatusCode(400, "Temp HP amounts much be positive numbers");
        }

        // Again we should theoretically check and fail out of the requesting user dosn't have access to modify the PC
        // but there is no user data for this expirement.

        try
        {
            var result = await _pcHealthManager.AddTempHpAsync(id, amount);

            return result == null
                ? StatusCode(500, _unableToSaveErrorMessage)
                : Ok(result);
        }
        catch (ArgumentOutOfRangeException)
        {
            return StatusCode(204);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while attempting to add Temp HP to PlayerCharacter data for Id {id}", id);
            return StatusCode(500, _unableToSaveErrorMessage);
        }
    }

    /// <summary>
    /// Applies damage of a given type to the specified PlayerCharacter taking into account any resistance defenses.
    /// </summary>
    /// <param name="id">ID of the PlayerCharacter to apply damage to.</param>
    /// <param name="amount">Base number of hit points damage to apply. Must be a positive number. Comes from Query String.</param>
    /// <param name="damageType">Base number of hit points damage to apply. Must be a positive number. Comes from Query String.</param>
    /// <returns>The updated <see cref="PlayerCharacterHealthStats"/> after applying the damage.</returns>
    /// <response code="200">Payload of resulting <see cref="PlayerCharacterHealthStats"/> after applying the damage.</response>
    /// <response code="204">PlayerCharacter not found.</response>
    /// <response code="400"><paramref name="amount"/> is too low.</response>
    /// <response code="500">Internal error.</response>
    [HttpPost("{id}/damage")]
    [ProducesResponseType(typeof(PlayerCharacterHealthStats), 200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ApplyDamage(string id, [FromQuery] int amount, [FromQuery] DamageType damageType = DamageType.None)
    {
        if (amount <= 0)
        {
            return StatusCode(400, "Damage amounts much be positive numbers");
        }

        // Again we should theoretically check and fail out of the requesting user dosn't have access to modify the PC
        // but there is no user data for this expirement.

        try
        {
            var result = await _pcHealthManager.ApplyDamageAsync(id, damageType, amount);

            return result == null
                ? StatusCode(500, _unableToSaveErrorMessage)
                : Ok(result);
        }
        catch (ArgumentOutOfRangeException)
        {
            return StatusCode(204);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while attempting to add apply damage to PlayerCharacter data for Id {id}", id);
            return StatusCode(500, _unableToSaveErrorMessage);
        }
    }
}
