namespace OrderFlow.Application.Operations.Requests;

public sealed record CreateOperationRequest(
    string ExternalReference,
    string CustomerName,
    string CustomerEmail,
    decimal TotalAmount,
    string Currency);
