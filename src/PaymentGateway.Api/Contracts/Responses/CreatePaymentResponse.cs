using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Contracts.Responses;

public class CreatePaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = "";
    public int Amount { get; set; }
    
    public static CreatePaymentResponse FromPayment(Payment payment)
    {
        return new CreatePaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.CardNumberLastFour,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }
}
