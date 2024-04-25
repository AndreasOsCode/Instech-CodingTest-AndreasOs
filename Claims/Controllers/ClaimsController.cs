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
        IGenericContext<Claim> claimsContext,
        AuditContext auditContext)
        : ControllerBase
    {
        private readonly Auditer _auditer = new(auditContext);
        
        //TODO: Separate out context
        //TODO: Add interface to constructor

        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await claimsContext.GetItemsAsync();
        }

        [HttpGet("{id}")]
        public async Task<Claim?> GetAsync(string id)
        {
            return await claimsContext.GetItemAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            await claimsContext.AddItemAsync(claim);
            _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            _auditer.AuditClaim(id, "DELETE");
            await claimsContext.DeleteItemAsync(id);
        }
    }
}
