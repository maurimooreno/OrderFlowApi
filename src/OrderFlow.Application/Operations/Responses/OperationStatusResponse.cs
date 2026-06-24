using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Operations.Responses;

public sealed record OperationStatusResponse(
    Guid Id,
    OperationStatus Status,
    DateTime? UpdatedAtUtc,
    DateTime? ProcessedAtUtc);
