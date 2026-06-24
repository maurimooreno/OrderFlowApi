using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Operations.CreateOperation;

public sealed record CreateOperationResult(
    Guid Id,
    OperationStatus Status,
    DateTime CreatedAtUtc);
