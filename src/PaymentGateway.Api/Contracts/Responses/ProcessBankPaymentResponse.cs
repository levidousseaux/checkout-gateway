﻿using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Contracts.Responses;

public class ProcessBankPaymentResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; } = "";
}