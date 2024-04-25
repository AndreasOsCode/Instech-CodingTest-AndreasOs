using Claims.Auditing;
using Claims.Contexts.Interfaces;
using Claims.Models;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController(IGenericDbContext<Cover> coversDbContext, 
    AuditContext auditContext, 
    ILogger<CoversController> logger,
    IPremiumComputer<CoverType> premiumComputer)
    : ControllerBase
{
    private readonly Auditer _auditer = new(auditContext);
    
    [HttpPost("compute")]
    public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Ok(premiumComputer.ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<IEnumerable<Cover>> GetAsync()
    {
        return await coversDbContext.GetItemsAsync();
    }

    [HttpGet("{id}")]
    public async Task<Cover?> GetAsync(string id)
    {
        return await coversDbContext.GetItemAsync(id);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = premiumComputer.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        await coversDbContext.AddItemAsync(cover);
        _auditer.AuditCover(cover.Id, "POST");
        return Ok(cover);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        _auditer.AuditCover(id, "DELETE");
        await coversDbContext.DeleteItemAsync(id);
    }
}
