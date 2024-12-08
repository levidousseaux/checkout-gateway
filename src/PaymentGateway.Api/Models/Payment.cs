using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models;

public class Payment
{
    public required Guid Id { get; init; }
    public required string CardNumber { get; init; }
    public required int ExpiryMonth { get; init; }
    public required int ExpiryYear { get; init; }
    public required string Cvv { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
    public required PaymentStatus Status { get; set; }
    
    public Guid AuthorizationCode { get; private set; }
    
    public int CardNumberLastFour => int.Parse(CardNumber[^4..]);
    
    public void UpdateStatus(string authorizationCode)
    {
        if (string.IsNullOrWhiteSpace(authorizationCode))
        {
            Status = PaymentStatus.Declined;
            return;
        }
        
        AuthorizationCode = Guid.Parse(authorizationCode);
        Status = PaymentStatus.Authorized;
    }
}