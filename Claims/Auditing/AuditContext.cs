using Microsoft.EntityFrameworkCore;

namespace Claims.Auditing
{
    /**
     * This should've had an interface and been injected, to avoid dependency inversion.
     * This also should not be responsible for both claims and covers, because of the open-closed principle.
     */
    public class AuditContext : DbContext
    {
        public AuditContext(DbContextOptions<AuditContext> options) : base(options)
        {
        }
        public DbSet<ClaimAudit> ClaimAudits { get; set; }
        public DbSet<CoverAudit> CoverAudits { get; set; }

        public async Task Save(ClaimAudit claimAudit)
        {
            ClaimAudits.Add(claimAudit);
            await SaveChangesAsync();
        }
        
        public async Task Save(CoverAudit coverAudit)
        {
            CoverAudits.Add(coverAudit);
            await SaveChangesAsync();
        }
    }
}
