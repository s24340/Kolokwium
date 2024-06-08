using System.Collections;
using Microsoft.EntityFrameworkCore;
using Kolokwium.Database;
using Kolokwium.Dto;
using Kolokwium.Model;

namespace Kolokwium.Service
{
    public class ClientService : IClientService
    {
        private readonly s24340Context _dbContext;

        public ClientService(s24340Context dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ClientSubscriptionsDto?> GetClientWithSubscriptionsAsync(int clientId)
        {
            var client = await _dbContext.Clients
                .Include(c => c.Sales)
                .ThenInclude(s => s.Subscription)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.IdClient == clientId);

            return client == null ? null : MapToDto(client);
        }

        private static ClientSubscriptionsDto MapToDto(Client client)
        {
            var subscriptions = client.Sales.Select(sale => new SubscriptionDTO
            {
                IdSubscription = sale.IdSubscription,
                Name = sale.Subscription.Name,
                TotalPaidAmount = client.Payments
                    .Where(p => p.IdSubscription == sale.Subscription.IdSubscription)
                    .Sum(p => p.Subscription.Price)
            }).ToList();

            return new ClientSubscriptionsDto
            {
                firstName = client.FirstName,
                lastName = client.LastName,
                email = client.Email,
                phone = client.Phone,
                subscriptions = subscriptions
            };
        }
    }
}