using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Gateways;

public interface IBankGateway
{
    Task<ProcessBankPaymentResponse> ProcessPayment(Payment payment, CancellationToken token);
}