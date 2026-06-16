using OrderFlow.Domain.Enums;

namespace OrderFlow.Domain.Entities;

public class OrderOperationHistory
{
    public Guid Id { get; private set; }
    public Guid OrderOperationId { get; private set; }
    public OperationStatus Status { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public OrderOperation? OrderOperation { get; private set; }

    private OrderOperationHistory()
    {
    }

    public OrderOperationHistory(
        Guid orderOperationId,
        OperationStatus status,
        string message)
    {
        Id = Guid.NewGuid();
        OrderOperationId = orderOperationId;
        Status = status;
        Message = message;
        CreatedAtUtc = DateTime.UtcNow;
    }
}