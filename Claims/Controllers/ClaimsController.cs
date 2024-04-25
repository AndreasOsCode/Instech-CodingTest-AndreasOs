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
        AuditContext auditContext)
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
                return BadRequest(ex.Message);
            }

            _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
        }
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            _auditer.AuditClaim(id, "DELETE");
            await claimsDbService.DeleteItemAsync(id);
        }
    }
}
