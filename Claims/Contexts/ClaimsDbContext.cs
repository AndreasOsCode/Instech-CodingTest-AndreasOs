using Claims.Contexts.Interfaces;
using Claims.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Contexts;

public class ClaimsDbContext(DbContextOptions<ClaimsDbContext> options) : DbContext(options), IGenericDbContext<Claim>
{
    private DbSet<Claim> Claims { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Claim>().ToCollection("claims");
    }

    public async Task<IEnumerable<Claim>> GetItemsAsync()
    {
        return await Claims.ToListAsync();
    }

    public async Task<Claim?> GetItemAsync(string id)
    {
        return await Claims
            .Where(claim => claim.Id == id)
            .SingleOrDefaultAsync();
    }

    public async Task AddItemAsync(Claim claim)
    {
        Claims.Add(claim);
        await SaveChangesAsync();
    }

    public async Task DeleteItemAsync(string id)
    {
        var claim = await GetItemAsync(id);
        if (claim is not null)
        {
            Claims.Remove(claim);
            await SaveChangesAsync();
        }
    }
}