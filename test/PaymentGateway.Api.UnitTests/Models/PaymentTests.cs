using Bogus;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.UnitTests.Models;

public class PaymentTests
{
    private readonly Faker<Payment> _paymentFaker = new Faker<Payment>()
        .RuleFor(x => x.CardNumber, f => f.Finance.CreditCardNumber())
        .RuleFor(x => x.ExpiryMonth, f => f.Random.Number(1, 12))
        .RuleFor(x => x.ExpiryYear, f => f.Date.Future().Year)
        .RuleFor(x => x.Amount, f =>  f.Random.Number(1, 10000))
        .RuleFor(x => x.Currency, f => f.Finance.Currency().Code)
        .RuleFor(x => x.Cvv, f => f.Random.Int(100, 9999).ToString());

    [Fact]
    public void Payment_CardNumberLastFour_Returns4Numbers()
    {
        // Arrange
        var payment = _paymentFaker
            .RuleFor(x => x.CardNumber, f => "2222405343248877")
            .Generate();
        
        // Assert
        Assert.Equal(8877, payment.CardNumberLastFour);
    }
    
    [Fact]
    public void Payment_UpdateStatusWithAuthorizationCode_StatusIsAuthorized()
    {
        // Arrange
        var payment = _paymentFaker.Generate();
        
        // Act
        payment.UpdateStatus(Guid.NewGuid().ToString());
        
        // Assert
        Assert.Equal(PaymentStatus.Authorized, payment.Status);
    }
    
    [Fact]
    public void Payment_UpdateStatusWithEmptyAuthorizationCode_StatusIsDeclined()
    {
        // Arrange
        var payment = _paymentFaker.Generate();
        
        // Act
        payment.UpdateStatus("");
        
        // Assert
        Assert.Equal(PaymentStatus.Declined, payment.Status);
    }
}