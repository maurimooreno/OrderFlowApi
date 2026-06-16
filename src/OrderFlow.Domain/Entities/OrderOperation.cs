using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities;

public class OrderOperation
{
    public Guid Id { get; private set; }
    public string ExternalReference { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public OperationType Type { get; private set; }
    public OperationStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public string? LastError { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }
    private readonly List<OrderOperationHistory> _history = new();
    public IReadOnlyCollection<OrderOperationHistory> History => _history.AsReadOnly();

    private OrderOperation()
    {
    }

    public OrderOperation(
        string externalReference,
        string customerName,
        string customerEmail,
        decimal totalAmount,
        string currency)
    {
        Id = Guid.NewGuid();
        ExternalReference = externalReference;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
        Currency = currency;
        Type = OperationType.OrderCreate;
        Status = OperationStatus.Pending;
        RetryCount = 0;
        CreatedAtUtc = DateTime.UtcNow;

        AddHistory(OperationStatus.Pending, "Operation created.");
    }

    public void MarkAsProcessing()
    {
        Status = OperationStatus.Processing;
        UpdatedAtUtc = DateTime.UtcNow;

        AddHistory(OperationStatus.Processing, "Operation processing started.");
    }

    public void MarkAsCompleted()
    {
        Status = OperationStatus.Completed;
        LastError = null;
        UpdatedAtUtc = DateTime.UtcNow;
        ProcessedAtUtc = DateTime.UtcNow;

        AddHistory(OperationStatus.Completed, "Operation completed successfully.");
    }

    public void MarkAsFailed(string error)
    {
        Status = OperationStatus.Failed;
        LastError = error;
        UpdatedAtUtc = DateTime.UtcNow;

        AddHistory(OperationStatus.Failed, error);
    }

    public void Retry()
    {
        if (Status != OperationStatus.Failed)
            throw new InvalidOperationException("Only failed operations can be retried.");

        RetryCount++;
        LastError = null;
        Status = OperationStatus.Pending;
        UpdatedAtUtc = DateTime.UtcNow;

        AddHistory(OperationStatus.Pending, "Operation retry requested.");
    }

    public void Cancel()
    {
        if (Status is OperationStatus.Completed or OperationStatus.Processing)
            throw new InvalidOperationException("Completed or processing operations cannot be cancelled.");

        Status = OperationStatus.Cancelled;
        UpdatedAtUtc = DateTime.UtcNow;

        AddHistory(OperationStatus.Cancelled, "Operation cancelled.");
    }

    private void AddHistory(OperationStatus status, string message)
    {
        _history.Add(new OrderOperationHistory(Id, status, message));
    }
}