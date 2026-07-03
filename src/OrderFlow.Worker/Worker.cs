using OrderFlow.Application.Operations.Interfaces;

namespace OrderFlow.Worker;

public class Worker(
    ILogger<Worker> logger,
    IOperationQueueConsumer operationQueueConsumer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var operationId = await operationQueueConsumer.DequeueAsync(stoppingToken);

            if (operationId is null)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            logger.LogInformation("Operation message consumed: {OperationId}", operationId);
        }
    }
}
