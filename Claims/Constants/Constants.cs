namespace Claims.Constants;

public static class Constants
{
    public static class Rules
    {
        public const int ClaimsMaxDamagecost = 100000;

        public const int PremiumBaseDayRate = 1250;
        public const decimal DefaultDailyPremiumMultiplier = 1.3m;
        public const decimal YachtDailyPremiumMultiplier = 1.1m;
        public const decimal PassengerShipDailyPremiumMultiplier = 1.2m;
        public const decimal TankerDailyPremiumMultiplier = 1.5m;

        public const decimal Yacht30DayDiscount = 0.95m;
        public const decimal Yacht180DayDiscount = 0.97m;
        
        public const decimal Default30DayDiscount = 0.98m;
        public const decimal Default180DayDiscount = 0.99m;
        
        
    }
}