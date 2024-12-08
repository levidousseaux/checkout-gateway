using PaymentGateway.Api.Contracts.Requests;
using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Gateways;

public class BankGateway(HttpClient client, ILogger<BankGateway> logger) : IBankGateway
{
    public async Task<ProcessBankPaymentResponse> ProcessPayment(Payment payment, CancellationToken token)
    {
        var request = new ProcessBankPaymentRequest
        {
            CardNumber = payment.CardNumber,
            ExpiryDate = $"{payment.ExpiryMonth.ToString().PadLeft(2, '0')}/{payment.ExpiryYear}",
            Currency = payment.Currency,
            Amount = payment.Amount,
            Cvv = payment.Cvv,
        };

        var response = await client.PostAsJsonAsync("payments", request, cancellationToken: token);

        if (!response.IsSuccessStatusCode)
        {
            return new ProcessBankPaymentResponse { AuthorizationCode = string.Empty, Authorized = false };
        }

        var content = await response.Content.ReadFromJsonAsync<ProcessBankPaymentResponse>(token);

        return content ?? new ProcessBankPaymentResponse { AuthorizationCode = string.Empty, Authorized = false };
    }
}