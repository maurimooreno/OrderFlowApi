using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Operations.Responses;

public sealed record OperationResponse(
    Guid Id,
    string ExternalReference,
    string CustomerName,
    string CustomerEmail,
    decimal TotalAmount,
    string Currency,
    OperationType Type,
    OperationStatus Status,
    int RetryCount,
    string? LastError,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? ProcessedAtUtc);
