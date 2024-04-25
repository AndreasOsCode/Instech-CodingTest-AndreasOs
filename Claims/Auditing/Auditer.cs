namespace Claims.Auditing
{
    /**
     * This should've had an interface and been injected, to avoid dependency inversion.
     * This also should not be responsible for both claims and covers, because of the open-closed principle.
     */
    public class Auditer
    {
        private readonly AuditContext _auditContext;
        public Auditer(AuditContext auditContext)
        {
            _auditContext = auditContext;
        }

        public async Task AuditClaim(string id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            await _auditContext.Save(claimAudit);
        }
        
        public async Task AuditCover(string id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            await _auditContext.Save(coverAudit);
        }
    }
}
