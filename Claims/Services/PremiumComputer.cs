using Claims.Models;
using Claims.Services.Interfaces;
using RuleConstants = Claims.Constants.Constants.Rules;

namespace Claims.Services;

public class PremiumComputer : IPremiumComputer<CoverType>
{
    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var multiplier = coverType switch
        {
            CoverType.Yacht => RuleConstants.YachtDailyPremiumMultiplier,
            CoverType.PassengerShip => RuleConstants.PassengerShipDailyPremiumMultiplier,
            CoverType.Tanker => RuleConstants.TankerDailyPremiumMultiplier,
            _ => RuleConstants.DefaultDailyPremiumMultiplier
        };

        var insuranceLength = (endDate - startDate).TotalDays;
        var totalPremium = 0m;
        /*
         * Assuming here that the discount specified in the task is applied successively
         * So that after 30 days, the yacht has a 1.1*0.95 = 1.045 => 4.5% more expensive per day.
         * This code is easily convertable to other interpretations, and expandable with other rules.
         */
        for (var i = 0; i < insuranceLength; i++)
        {
            switch (i)
            {
                case 30:
                {
                    multiplier *= coverType switch
                    {
                        CoverType.Yacht => RuleConstants.Yacht30DayDiscount,
                        _ => RuleConstants.Default30DayDiscount
                    };
                    break;
                }
                case 180:
                {
                    multiplier *= coverType switch
                    {
                        CoverType.Yacht => RuleConstants.Yacht180DayDiscount,
                        _ => RuleConstants.Default180DayDiscount
                    };
                    break;
                }
            }
            totalPremium += Constants.Constants.Rules.PremiumBaseDayRate * multiplier;
        }

        return totalPremium;
    }
}