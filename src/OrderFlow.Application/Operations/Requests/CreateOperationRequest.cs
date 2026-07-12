using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Application.Operations.Requests;

public sealed record CreateOperationRequest(
    [Required]
    [StringLength(100)]
    string ExternalReference,

    [Required]
    [StringLength(200)]
    string CustomerName,

    [Required]
    [EmailAddress]
    [StringLength(254)]
    string CustomerEmail,

    [Range(0.01, 9999999999999999.99)]
    decimal TotalAmount,

    [Required]
    [StringLength(3, MinimumLength = 3)]
    string Currency);
