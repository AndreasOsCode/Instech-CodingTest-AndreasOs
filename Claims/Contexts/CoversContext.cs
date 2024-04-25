using Claims.Contexts.Interfaces;
using Claims.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Contexts;

public class CoversContext : DbContext, IGenericContext<Cover>
{
    private DbSet<Claim> Covers { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cover>().ToCollection("covers");
    }


    public Task<IEnumerable<Cover>> GetItemsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Cover?> GetItemAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task AddItemAsync(Cover item)
    {
        throw new NotImplementedException();
    }

    public Task DeleteItemAsync(string id)
    {
        throw new NotImplementedException();
    }
}