using Claims.Models;
using Claims.Services;
using Claims.Services.Interfaces;
using Xunit;
using Xunit.Abstractions;
using RuleConstants = Claims.Constants.Constants.Rules;

namespace Claims.Tests;

public class PremiumCalculationTest
{
    private readonly IPremiumComputer<CoverType> _computer;
    private readonly ITestOutputHelper _testOutputHelper;
    
    public PremiumCalculationTest( ITestOutputHelper testOutputHelper)
    {
        _computer = new PremiumComputer();
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void First_Month_One_Day_Later()
    {
        var startTime = new DateTime(2000, 01, 01);
        var endTime = new DateTime(2000, 01, 05);
        const int dayRate = RuleConstants.PremiumBaseDayRate;

        var yacht = _computer.ComputePremium(startTime, endTime, CoverType.Yacht);
        var yachtOneMore = _computer.ComputePremium(startTime, endTime.AddDays(1), CoverType.Yacht);
        
        _testOutputHelper.WriteLine(yacht.ToString());
        _testOutputHelper.WriteLine(yachtOneMore.ToString());
        Assert.True(yachtOneMore == yacht + 
            dayRate*
            RuleConstants.YachtDailyPremiumMultiplier);
    }
    
    [Fact]
    public void Second_Bracket_One_Day_Later()
    {
        var startTime = new DateTime(2000, 01, 01);
        var endTime = new DateTime(2000, 02, 05);
        const int dayRate = RuleConstants.PremiumBaseDayRate;

        var yacht = _computer.ComputePremium(startTime, endTime, CoverType.Yacht);
        var yachtOneMore = _computer.ComputePremium(startTime, endTime.AddDays(1), CoverType.Yacht);
        Assert.True(yachtOneMore == yacht + 
            dayRate*
            RuleConstants.YachtDailyPremiumMultiplier*
            RuleConstants.Yacht30DayDiscount);
        
        var tanker = _computer.ComputePremium(startTime, endTime, CoverType.Tanker);
        var tankerOneDayLater = _computer.ComputePremium(startTime, endTime.AddDays(1), CoverType.Tanker);
        Assert.True(tankerOneDayLater == tanker + 
            dayRate*
            RuleConstants.TankerDailyPremiumMultiplier*
            RuleConstants.Default30DayDiscount);
    }
    [Fact]
    public void Third_Bracket_One_Day_Later()
    {
        var startTime = new DateTime(2000, 01, 01);
        var endTime = new DateTime(2000, 08, 05);
        const int dayRate = RuleConstants.PremiumBaseDayRate;

        var yacht = _computer.ComputePremium(startTime, endTime, CoverType.Yacht);
        var yachtOneMore = _computer.ComputePremium(startTime, endTime.AddDays(1), CoverType.Yacht);
        Assert.True(yachtOneMore == yacht + 
            dayRate*
            RuleConstants.YachtDailyPremiumMultiplier*
            RuleConstants.Yacht30DayDiscount*
            RuleConstants.Yacht180DayDiscount);
        
        var tanker = _computer.ComputePremium(startTime, endTime, CoverType.Tanker);
        var tankerOneDayLater = _computer.ComputePremium(startTime, endTime.AddDays(1), CoverType.Tanker);
        Assert.True(tankerOneDayLater == tanker + 
            dayRate*
            RuleConstants.TankerDailyPremiumMultiplier*
            RuleConstants.Default30DayDiscount*
            RuleConstants.Default180DayDiscount);
    }
}