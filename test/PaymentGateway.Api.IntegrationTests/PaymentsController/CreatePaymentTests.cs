using System.Net;
using System.Net.Http.Json;
using Bogus;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Contracts.Requests;
using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Tests.PaymentsController;

public class CreatePaymentTests(ApiFactory apiFactory) : IClassFixture<ApiFactory>
{
    private static readonly string[] AcceptedCurrencies = [
        "USD",
        "GBP",
        "BRL",
    ];
    
    private readonly HttpClient _client = apiFactory.CreateClient();
    
    private readonly Faker<CreatePaymentRequest> _createPaymentFaker = new Faker<CreatePaymentRequest>()
        .RuleFor(x => x.CardNumber, _ => "2222405343248877")
        .RuleFor(x => x.ExpiryMonth, f => f.Random.Number(1, 12))
        .RuleFor(x => x.ExpiryYear, _ => DateTime.Now.Year + 1)
        .RuleFor(x => x.Amount, f =>  f.Random.Number(1, 10000))
        .RuleFor(
            x => x.Currency,
            f => AcceptedCurrencies[f.Random.Int(0, 2)] )
        .RuleFor(x => x.Cvv, f => f.Random.Int(100, 9999).ToString());
    
    [Fact]
    public async Task CreatePayment_ValidCard_CreateAuthorizedPayment()
    {
        // Arrange
        var request = _createPaymentFaker.Generate();
        request.CardNumber = "2222405343248877";
        request.ExpiryMonth = 04;
        request.ExpiryYear = 2025;
        request.Amount = 100;
        request.Currency = "GBP";
        request.Cvv = "123";
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", request);
        var payment = await response.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        
        // Assert
        var repository = apiFactory.Services.GetRequiredService<IPaymentsRepository>();
        var paymentFromRepository = repository.Get(payment!.Id);

        Assert.NotNull(paymentFromRepository);
        Assert.NotEqual(Guid.Empty, paymentFromRepository?.AuthorizationCode);
        Assert.Equal(PaymentStatus.Authorized, paymentFromRepository?.Status);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task CreatePayment_InvalidCard_CreateDeclinedPayment()
    {
        // Arrange
        var request = _createPaymentFaker.Generate();
        request.CardNumber = "2222405343248112";
        request.ExpiryMonth = 1;
        request.ExpiryYear = 2026;
        request.Amount = 60000;
        request.Currency = "USD";
        request.Cvv = "456";
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", request);
        var payment = await response.Content.ReadFromJsonAsync<CreatePaymentResponse>();
        
        // Assert
        var repository = apiFactory.Services.GetRequiredService<IPaymentsRepository>();
        var paymentFromRepository = repository.Get(payment!.Id);

        Assert.NotNull(paymentFromRepository);
        Assert.Equal(Guid.Empty, paymentFromRepository?.AuthorizationCode);
        Assert.Equal(PaymentStatus.Declined, paymentFromRepository?.Status);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Theory]
    [InlineData("1234567891234")] // less than 14 digits
    [InlineData("12345678912345678901")] // more than 19 digits
    [InlineData("123456789012345A")] // contains non-numeric characters
    public async Task CreatePayment_InvalidCardNumber_ReturnsBadRequestResponse(string cardNumber)
    {
        // Arrange
        var request = _createPaymentFaker.Generate();
        request.CardNumber = cardNumber;
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", request);
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("CardNumber", validationResult!.Errors.Keys.First());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public async Task CreatePayment_InvalidExpiryMonth_ReturnsBadRequestResponse(int expiryMonth)
    {
        // Arrange
        var request = _createPaymentFaker.Generate();
        request.ExpiryMonth = expiryMonth;
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", request);
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("ExpiryMonth", validationResult!.Errors.Keys.First());
    }

    [Fact]
    public async Task CreatePayment_InvalidExpiryYearAndMonth_ReturnsBadRequestResponse()
    {
        // Arrange
        var request = _createPaymentFaker.Generate();
        request.ExpiryYear = DateTime.Now.Year;
        request.ExpiryMonth = DateTime.Now.Month;
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", request);
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(validationResult!.Errors.Keys.Contains("ExpiryMonth"));
        Assert.True(validationResult.Errors.Keys.Contains("ExpiryYear"));
    }

    [Theory]
    [InlineData("ABCD")]
    [InlineData("123")]
    public async Task CreatePayment_InvalidCurrency_ReturnsBadRequestResponse(string currency)
    {
        // Arrange
        var request = _createPaymentFaker.Generate();
        request.Currency = currency;
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", request);
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Currency", validationResult!.Errors.Keys.First());
    }

    [Theory]
    [InlineData("01")]
    [InlineData("12345")]
    [InlineData("12A")]
    public async Task CreatePayment_InvalidCvv_ReturnsBadRequestResponse(string cvv)
    {
        // Arrange
        var request = _createPaymentFaker.Generate();
        request.Cvv = cvv;
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/Payments", request);
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Cvv", validationResult!.Errors.Keys.First());
    }
}