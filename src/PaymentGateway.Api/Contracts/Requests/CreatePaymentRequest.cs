using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Contracts.Requests;

public class CreatePaymentRequest : IValidatableObject
{
    [Required]
    [Length(14, 19)]
    [RegularExpression(@"^\d+$")]
    public string CardNumber { get; set; } = null!;
    
    [Required]
    [Range(1, 12)]
    public int ExpiryMonth { get; set; }
    
    [Required]
    public int ExpiryYear { get; set; }

    [Required]
    [Length(3, 3)]
    public string Currency { get; set; } = null!;
    
    [Required]
    public int Amount { get; set; }
    
    [Required]
    [Length(3, 4)]
    [RegularExpression(@"^\d+$")]
    public string Cvv { get; set; } = null!;
    
    private static readonly string[] AcceptedCurrencies = [
        "USD",
        "GBP",
        "BRL",
    ];
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ExpiryYear < DateTime.Now.Year)
        {
            yield return new ValidationResult("Expiry year must be in the future", [nameof(ExpiryYear)]);
        }
        
        if (ExpiryYear == DateTime.Now.Year && ExpiryMonth <= DateTime.Now.Month)
        {
            yield return new ValidationResult("Expiry year and month must be in the future", [nameof(ExpiryYear)]);
            yield return new ValidationResult("Expiry year and month must be in the future", [nameof(ExpiryMonth)]);
        }
        
        if (!AcceptedCurrencies.Contains(Currency))
        {
            yield return new ValidationResult("Currency not supported", [nameof(Currency)]);
        }
    }
}