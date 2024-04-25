using Claims.Contexts.Interfaces;
using Claims.Models;
using Claims.Services.Interfaces;

namespace Claims.Services;
/**
 * Service for adding business logic and validation
 * to interactions with the claims database.
 */
public class ClaimsDbService(
    IGenericDbContext<Claim> claimsDbContext,
    IGenericDbService<Cover> coversDbService) : IGenericDbService<Claim>
{
    public async Task<IEnumerable<Claim>> GetItemsAsync()
    {
        return await claimsDbContext.GetItemsAsync();
    }
    public async Task<Claim?> GetItemAsync(string id)
    {
        return await claimsDbContext.GetItemAsync(id);
    }
    public async Task<Claim> AddItemAsync(Claim claim)
    {
        if (claim.DamageCost > Constants.Constants.Rules.ClaimsMaxDamagecost)
        {
            throw new ArgumentException($"Damage cost of claim exceeds maximum of {Constants.Constants.Rules.ClaimsMaxDamagecost}");
        }

        var cover = await coversDbService.GetItemAsync(claim.CoverId);
        if (cover == null)
        {
            throw new ArgumentException($"Associated cover does not exist.");
        }

        if (claim.Created < cover.StartDate || cover.EndDate < claim.Created)
        {
            throw new ArgumentException("Claim falls outside of cover's date range.");
        }
        await claimsDbContext.AddItemAsync(claim);
        return claim;
    }
    public async Task DeleteItemAsync(string id)
    {
        await claimsDbContext.DeleteItemAsync(id);
    }
    
}