using PaymentGateway.Api.Contracts.Requests;
using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Gateways;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.UseCases;

public class CreatePaymentUseCase(IPaymentsRepository repository, IBankGateway gateway)
{
    public async Task<CreatePaymentResponse> ExecuteAsync(CreatePaymentRequest request, CancellationToken token)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Declined,
            ExpiryYear = request.ExpiryYear,
            ExpiryMonth = request.ExpiryMonth,
            Amount = request.Amount,
            CardNumber = request.CardNumber,
            Currency = request.Currency,
            Cvv = request.Cvv
        };

        var response = await gateway.ProcessPayment(payment, token);

        payment.UpdateStatus(response.AuthorizationCode);
        repository.Add(payment);

        return CreatePaymentResponse.FromPayment(payment);
    }
}