using Claims.Contexts.Interfaces;
using Claims.Models;
using Claims.Services.Interfaces;

namespace Claims.Services;

public class CoversDbService(IGenericDbContext<Cover> coversDbContext) : IGenericDbService<Cover>
{
    public async Task<IEnumerable<Cover>> GetItemsAsync()
    {
        return await coversDbContext.GetItemsAsync();
    }
    public async Task<Cover?> GetItemAsync(string id)
    {
        return await coversDbContext.GetItemAsync(id);
    }
    public async Task<Cover> AddItemAsync(Cover cover)
    {
        if (cover.StartDate < DateTime.Today)
        {
            throw new ArgumentException("Cover cannot start in the past.");
        }

        if ((cover.EndDate > cover.StartDate.AddYears(1)))
        {
            throw new ArgumentException("Cover cannot last more than a year.");
        }
        await coversDbContext.AddItemAsync(cover);
        return cover;
    }
    public async Task DeleteItemAsync(string id)
    {
        await coversDbContext.DeleteItemAsync(id);
    }
    
}