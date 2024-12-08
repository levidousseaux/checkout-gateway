using PaymentGateway.Api.Gateways;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddHttpClient<IBankGateway, BankGateway>(client => {
    client.BaseAddress = new Uri("http://localhost:8080");
});
builder.Services.AddSingleton<CreatePaymentUseCase>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
