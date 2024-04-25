using Claims.Auditing;
using Claims.Contexts.Interfaces;
using Claims.Models;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(
        ILogger<ClaimsController> logger,
        IGenericDbService<Claim> claimsDbService,
        AuditContext auditContext) //This should have been dependency injected, but ran into issues implementing it.
        : ControllerBase
    {
        private readonly Auditer _auditer = new(auditContext);
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await claimsDbService.GetItemsAsync();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("{id}")]
        public async Task<Claim?> GetAsync(string id)
        {
            return await claimsDbService.GetItemAsync(id);
        }
        
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            try
            {
                await claimsDbService.AddItemAsync(claim);
            }
            catch (ArgumentException ex)
            {
                logger.LogError(ex,"Bad request received");
                return BadRequest(ex.Message);
            }
            await _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
        }
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            await _auditer.AuditClaim(id, "DELETE");
            await claimsDbService.DeleteItemAsync(id);
        }
    }
}
