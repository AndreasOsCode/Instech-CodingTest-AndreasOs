using System.Net;
using System.Net.Http.Json;
using Claims.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Bson;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Claims.Tests
{
    /**
    * These tests connect to the same databases as the main program does.
    * They serve as integration tests right now, for unit testing I would set up a Moq
    * 
    */
    public class ClaimsControllerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        
        public ClaimsControllerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Get_Claims()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                {});
            var client = application.CreateClient();

            var response = await client.GetAsync("/Claims");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var claims = JsonConvert.DeserializeObject<IList<Claim>>(content);
            Assert.True(claims != null);
        }
        [Fact]
        public async Task Insert_And_Get_And_Delete_And_Get()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ =>
                    {});
            var client = application.CreateClient();
            //Need to have a valid cover to test claims.
            var cover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(120),
                Type = CoverType.Yacht,
                Premium = 0
            };
            var coverResponse = await client.PostAsync("/Covers", JsonContent.Create(cover));
            coverResponse.EnsureSuccessStatusCode();
            var coverContent = await coverResponse.Content.ReadAsStringAsync();
            var insertedCover = JsonConvert.DeserializeObject<Cover>(coverContent);
            Assert.True(insertedCover != null);
            
            var claim = new Claim
            {
                Id = Guid.NewGuid().ToString(),
                CoverId = insertedCover.Id,
                Created = DateTime.Now.AddDays(1),
                Name = "TestCollision",
                Type = ClaimType.Collision,
                DamageCost = 2500
            };
            
            var insertResponse = await client.PostAsync("/Claims", JsonContent.Create(claim));
            insertResponse.EnsureSuccessStatusCode();
            var insertContent = await insertResponse.Content.ReadAsStringAsync();
            var insertedClaim = JsonConvert.DeserializeObject<Claim>(insertContent);
            Assert.True(insertedClaim != null);

            var getResponse = await client.GetAsync($"Claims/{insertedClaim.Id}");
            getResponse.EnsureSuccessStatusCode();
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var gottenClaim = JsonConvert.DeserializeObject<Claim>(getContent);
            Assert.True(gottenClaim != null);
            
            Assert.True(gottenClaim.Id == insertedClaim.Id);
            
            var deleteResponse = await client.DeleteAsync($"Claims/{insertedClaim.Id}");
            deleteResponse.EnsureSuccessStatusCode();
            
            var afterDeleteGetResponse = await client.GetAsync($"Claims/{insertedClaim.Id}");
            Assert.True(afterDeleteGetResponse.StatusCode == HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task Bad_Insert_Should_Fail()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ => { });
            var client = application.CreateClient();
            //Need to have a valid cover to test claims.
            var cover = new Cover
            {
                Id = Guid.NewGuid().ToString(),
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(120),
                Type = CoverType.Yacht,
                Premium = 0
            };
            var coverResponse = await client.PostAsync("/Covers", JsonContent.Create(cover));
            coverResponse.EnsureSuccessStatusCode();
            var coverContent = await coverResponse.Content.ReadAsStringAsync();
            var insertedCover = JsonConvert.DeserializeObject<Cover>(coverContent);
            Assert.True(insertedCover != null);

            var outsideCoverClaim = new Claim
            {
                Id = Guid.NewGuid().ToString(),
                CoverId = insertedCover.Id,
                Created = DateTime.Now.AddDays(200),
                Name = "TestCollision",
                Type = ClaimType.Collision,
                DamageCost = 2500
            };
            var outsideCoverClaimResponse = await client.PostAsync("/Claims", JsonContent.Create(outsideCoverClaim));
            Assert.True(outsideCoverClaimResponse.StatusCode == HttpStatusCode.BadRequest);
        
            var tooExpensiveClaim = new Claim
            {
                Id = Guid.NewGuid().ToString(),
                CoverId = insertedCover.Id,
                Created = DateTime.Now.AddDays(50),
                Name = "TestCollision",
                Type = ClaimType.Collision,
                DamageCost = 300000000
            };
            var tooExpensiveClaimResponse = await client.PostAsync("/Claims", JsonContent.Create(tooExpensiveClaim));
            Assert.True(tooExpensiveClaimResponse.StatusCode == HttpStatusCode.BadRequest);
        }
    }
}
