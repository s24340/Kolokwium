namespace Kolokwium.Service;

public interface IPaymentService
{
    Task<int?> AddPaymentAsync(int idClient, int idSubscription, decimal paymentAmount);
}