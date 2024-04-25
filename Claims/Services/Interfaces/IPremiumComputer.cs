namespace Claims.Services.Interfaces;

public interface IPremiumComputer<T>
{
    public decimal ComputePremium(DateTime startDate, DateTime endDate, T premiumType);
}