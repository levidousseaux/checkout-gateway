using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Contracts.Requests;
using PaymentGateway.Api.Contracts.Responses;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.UseCases;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    [HttpGet("{id:guid}")]
    public ActionResult<GetPaymentResponse?> GetPayment(Guid id, IPaymentsRepository paymentsRepository)
    {
        var payment = paymentsRepository.Get(id);
        
        return payment is null 
            ? NotFound()
            : new OkObjectResult(GetPaymentResponse.FromPayment(payment));
    }
    
    [HttpPost]
    public async Task<ActionResult<CreatePaymentResponse>> CreatePayment(
        CreatePaymentRequest request,
        CreatePaymentUseCase useCase,
        CancellationToken token)
    {
        return new OkObjectResult(await useCase.ExecuteAsync(request, token));
    }
}