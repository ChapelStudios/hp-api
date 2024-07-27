using DDB.HealthCycle.Logic.PcHealth;
using DDB.HealthCycle.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DDB.HealthCycle.Server.Controllers;
[ApiController]
[Route("[controller]")]
public class PlayerCharacterController(ILogger<PlayerCharacterController> _logger, IPcHealthManager _pcHealthManager) : ControllerBase
{
    /// <summary>
    /// Gets the Json data for a Player Character.
    /// </summary>
    /// <param name="id">The id of the player character.</param>
    /// <response code="200">Payload of <see cref="PlayerCharacter"/> with spacific id.</response>
    /// <response code="204"><see cref="PlayerCharacter"/> not found.</response>
    /// <response code="500">Internal error.</response>
    [HttpGet(Name = "{id}")]
    [ProducesResponseType(typeof(PlayerCharacter), 200)]
    [ProducesResponseType(204)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            // This should theoretically check and fail out of the requesting user dosn't have access to the PC
            // but there is no user data for this expirement.

            var pc = await _pcHealthManager.GetPlayerCharacterAsync(id);
            return pc == null
                ? StatusCode(500, "Unable to save updated character data, please try again")
                : Ok(pc);
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
    /// <param name="amount">Number of hit points to heal. Must be a positive number.</param>
    /// <returns>The updated <see cref="PlayerCharacterHealthStats"/> after applying the healing.</returns>
    /// <response code="200">Payload of resulting <see cref="PlayerCharacterHealthStats"/> after applying the healing.</response>
    /// <response code="204">PlayerCharacter not found.</response>
    /// <response code="400"><paramref name="amount"/> is too low.</response>
    /// <response code="500">Internal error.</response>
    [HttpGet(Name = "{id}/heal")]
    [ProducesResponseType(typeof(PlayerCharacter), 200)]
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

            return Ok(result);
        }
        catch (ArgumentOutOfRangeException)
        {
            return StatusCode(204);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while attempting to heal PlayerCharacter data for Id {id}", id);
            return StatusCode(500, "Unable to save updated character data, please try again");
        }
    }
}
