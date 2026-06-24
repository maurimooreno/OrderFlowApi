using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Operations.GetOperation;

public sealed record GetOperationResult(
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
