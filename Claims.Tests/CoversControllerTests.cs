using System.Net;
using System.Net.Http.Json;
using Claims.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Claims.Tests;

public class CoversControllerTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CoversControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Get_Covers()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });
        var client = application.CreateClient();

        var response = await client.GetAsync("/Covers");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var covers = JsonConvert.DeserializeObject<IList<Cover>>(content);
        Assert.True(covers != null);
    }

    [Fact]
    public async Task Insert_And_Get_And_Delete_And_Get()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });
        var client = application.CreateClient();

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

        var getResponse = await client.GetAsync($"Covers/{insertedCover.Id}");
        getResponse.EnsureSuccessStatusCode();
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var gottenCover = JsonConvert.DeserializeObject<Cover>(getContent);
        Assert.True(gottenCover != null);
        
        Assert.True(gottenCover.Id == insertedCover.Id);

        var deleteResponse = await client.DeleteAsync($"Covers/{insertedCover.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        var afterDeleteGetResponse = await client.GetAsync($"Covers/{insertedCover.Id}");
        Assert.True(afterDeleteGetResponse.StatusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Bad_Insert_Should_Fail()
    {
        var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ => { });
        var client = application.CreateClient();

        var pastCover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(120),
            Type = CoverType.Yacht,
            Premium = 0
        };
        var pastCoverResponse = await client.PostAsync("/Covers", JsonContent.Create(pastCover));
        Assert.True(pastCoverResponse.StatusCode == HttpStatusCode.BadRequest);
        
        var tooBigCover = new Cover
        {
            Id = Guid.NewGuid().ToString(),
            StartDate = DateTime.Now.AddDays(1),
            EndDate = DateTime.Now.AddDays(720),
            Type = CoverType.Yacht,
            Premium = 0
        };
        var tooBigCoverResponse = await client.PostAsync("/Covers", JsonContent.Create(tooBigCover));
        Assert.True(tooBigCoverResponse.StatusCode == HttpStatusCode.BadRequest);
    }
}