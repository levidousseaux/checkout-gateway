using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Tests.PaymentsController;

public class GetPaymentTests(ApiFactory apiFactory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = apiFactory.CreateClient();
    private readonly Random _random = new();

    [Fact]
    public async Task GetPayment_ExistingPayment_ReturnsOkPaymentResponse()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumber = "1234567890123456",
            Currency = "GBP",
            Status = PaymentStatus.Authorized,
            Cvv = _random.Next(100, 999).ToString()
        };

        var paymentsRepository = apiFactory.Services.GetRequiredService<IPaymentsRepository>();
        paymentsRepository.Add(payment);
        
        // Act
        var response = await _client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(payment.Id, paymentResponse!.Id);
        Assert.Equal(payment.ExpiryYear, paymentResponse.ExpiryYear);
        Assert.Equal(payment.ExpiryMonth, paymentResponse.ExpiryMonth);
        Assert.Equal(payment.Amount, paymentResponse.Amount);
        Assert.Equal(payment.CardNumberLastFour, paymentResponse.CardNumberLastFour);
        Assert.Equal(payment.Currency, paymentResponse.Currency);
        Assert.Equal(payment.Status, paymentResponse.Status);
    }

    [Fact]
    public async Task GetPayment_PaymentNotExists_ReturnsNotFoundResponse()
    {
        // Act
        var response = await _client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}