using Claims.Auditing;
using Claims.Contexts.Interfaces;
using Claims.Models;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(
        ILogger<ClaimsController> logger,
        IGenericDbContext<Claim> claimsDbContext,
        AuditContext auditContext)
        : ControllerBase
    {
        private readonly Auditer _auditer = new(auditContext);
        
        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await claimsDbContext.GetItemsAsync();
        }

        [HttpGet("{id}")]
        public async Task<Claim?> GetAsync(string id)
        {
            return await claimsDbContext.GetItemAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            await claimsDbContext.AddItemAsync(claim);
            _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            _auditer.AuditClaim(id, "DELETE");
            await claimsDbContext.DeleteItemAsync(id);
        }
    }
}
