using OrderFlow.Domain.Enums;

namespace OrderFlow.Application.Operations.RetryOperation;

public sealed record RetryOperationResult(
    RetryOperationStatus ResultStatus,
    Guid? Id = null,
    OperationStatus? Status = null,
    int? RetryCount = null,
    DateTime? UpdatedAtUtc = null);
