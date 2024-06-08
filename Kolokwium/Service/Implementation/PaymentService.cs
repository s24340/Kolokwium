using Microsoft.EntityFrameworkCore;
using Kolokwium.Database;
using Kolokwium.Model;

namespace Kolokwium.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly s24340Context _dbContext;

        public PaymentService(s24340Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int?> AddPaymentAsync(int clientId, int subscriptionId, decimal amount)
        {
            var client = await _dbContext.Clients.FindAsync(clientId);
            if (client == null)
            {
                return null;
            }

            var subscription = await _dbContext.Subscriptions.FindAsync(subscriptionId);
            if (subscription == null)
            {
                return null;
            }

            var sales = await _dbContext.Sales
                .Where(s => s.IdClient == clientId && s.IdSubscription == subscriptionId)
                .ToListAsync();

            if (!sales.Any())
            {
                return null;
            }

            var latestSale = sales.OrderByDescending(s => s.CreatedAt).First();
            var renewalDate = latestSale.CreatedAt.AddMonths(subscription.RenewalPeriod);
            if (DateTime.Now > renewalDate)
            {
                return null;
            }

            var existingPayment = await _dbContext.Payments
                .FirstOrDefaultAsync(p => p.IdClient == clientId && p.IdSubscription == subscriptionId && p.Date >= latestSale.CreatedAt && p.Date < renewalDate);

            if (existingPayment != null)
            {
                return null;
            }

            var discounts = await _dbContext.Discounts
                .Where(d => d.IdSubscription == subscriptionId && d.DateFrom <= DateTime.Now && d.DateTo >= DateTime.Now)
                .ToListAsync();

            var maxDiscount = discounts.Any() ? discounts.Max(d => d.Value) : 0;
            var finalAmount = subscription.Price * (1 - (maxDiscount / 100.0m));

            if (amount != finalAmount)
            {
                return null;
            }

            var payment = new Payment
            {
                IdClient = clientId,
                IdSubscription = subscriptionId,
                Date = DateTime.Now
            };

            _dbContext.Payments.Add(payment);
            await _dbContext.SaveChangesAsync();

            return payment.IdPayment;
        }
    }
}
