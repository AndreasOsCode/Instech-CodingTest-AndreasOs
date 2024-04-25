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

        var premiumPerDay = Constants.Constants.Rules.PremiumBaseDayRate * multiplier;
        var insuranceLength = (endDate - startDate).TotalDays;
        var totalPremium = 0m;

        for (var i = 0; i < insuranceLength; i++)
        {
            /*if (i < 30) {totalPremium += premiumPerDay;}
            
            if (i is > 30 and < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
            else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
            
            if (i is > 180 and < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
            else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;*/

            switch (i)
            {
                case 30:
                {
                    if (coverType == CoverType.Yacht) multiplier *= 0.95m;
                    else multiplier *= 0.98m;
                    break;
                }
                case 180:
                {
                    if (coverType == CoverType.Yacht) multiplier *= 0.97m;
                    else multiplier *= 0.99m;
                    break;
                }
            }
            totalPremium += Constants.Constants.Rules.PremiumBaseDayRate * multiplier;
        }

        return totalPremium;
    }
}