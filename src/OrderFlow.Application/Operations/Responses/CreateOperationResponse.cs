using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Operations.Responses;

public sealed record CreateOperationResponse(
    Guid Id,
    OperationStatus Status,
    DateTime CreatedAtUtc);
