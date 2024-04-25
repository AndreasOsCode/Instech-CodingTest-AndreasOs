using Claims.Models;
using Claims.Services.Interfaces;

namespace Claims.Services;

public class PremiumComputer : IPremiumComputer<CoverType>
{
    public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        var multiplier = coverType switch
        {
            CoverType.Yacht => 1.1m,
            CoverType.PassengerShip => 1.2m,
            CoverType.Tanker => 1.5m,
            _ => 1.3m
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
                        CoverType.Yacht => 0.95m,
                        _ => 0.98m
                    };
                    break;
                }
                case 180:
                {
                    multiplier *= coverType switch
                    {
                        CoverType.Yacht => 0.97m,
                        _ => 0.99m
                    };
                    break;
                }
            }
            totalPremium += Constants.Constants.Rules.PremiumBaseDayRate * multiplier;
        }

        return totalPremium;
    }
}