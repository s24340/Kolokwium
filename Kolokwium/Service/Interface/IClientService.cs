using Kolokwium.Dto;

namespace Kolokwium.Service;

public interface IClientService
{
    Task<ClientSubscriptionsDto?> GetClientWithSubscriptionsAsync(int id);
}