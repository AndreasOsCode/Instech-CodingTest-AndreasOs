using Claims.Contexts.Interfaces;
using Claims.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Contexts;

public class CoversDbContext(DbContextOptions<CoversDbContext> options) : DbContext(options), IGenericDbContext<Cover>
{
    private DbSet<Cover> Covers { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cover>().ToCollection("covers");
    }
    
    public async Task<IEnumerable<Cover>> GetItemsAsync()
    {
        return await Covers.ToListAsync();
    }

    public async Task<Cover?> GetItemAsync(string id)
    {
        return await Covers
            .Where(claim => claim.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task AddItemAsync(Cover cover)
    {
        Covers.Add(cover);
        await SaveChangesAsync();
    }

    public async Task DeleteItemAsync(string id)
    {
        var cover = await GetItemAsync(id);
        if (cover is not null)
        {
            Covers.Remove(cover);
            await SaveChangesAsync();
        }
    }
}