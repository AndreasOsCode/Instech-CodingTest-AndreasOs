using Claims.Auditing;
using Claims.Contexts.Interfaces;
using Claims.Models;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController(IGenericDbService<Cover> coversDbService, 
    AuditContext auditContext, 
    ILogger<CoversController> logger,
    IPremiumComputer<CoverType> premiumComputer)
    : ControllerBase
{
    private readonly Auditer _auditer = new(auditContext);
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("ComputeCoverPremium")]
    public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Ok(premiumComputer.ComputePremium(startDate, endDate, coverType));
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IEnumerable<Cover>> GetAsync()
    {
        return await coversDbService.GetItemsAsync();
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<Cover?> GetAsync(string id)
    {
        return await coversDbService.GetItemAsync(id);
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        cover.Id = Guid.NewGuid().ToString();
        cover.Premium = premiumComputer.ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
        try
        {
            await coversDbService.AddItemAsync(cover);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        _auditer.AuditCover(cover.Id, "POST");
        return Ok(cover);
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        _auditer.AuditCover(id, "DELETE");
        await coversDbService.DeleteItemAsync(id);
    }
}
