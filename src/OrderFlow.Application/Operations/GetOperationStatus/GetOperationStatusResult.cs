using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Operations.GetOperationStatus;

public sealed record GetOperationStatusResult(
    Guid Id,
    OperationStatus Status,
    DateTime? UpdatedAtUtc,
    DateTime? ProcessedAtUtc);
