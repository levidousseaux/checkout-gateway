using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Repositories;

public interface IPaymentsRepository
{
    Payment? Get(Guid id);
    void Add(Payment payment);
}