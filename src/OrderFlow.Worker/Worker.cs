using OrderFlow.Application.Operations.Interfaces;
using OrderFlow.Application.Operations.ProcessOperation;

namespace OrderFlow.Worker;

public class Worker(
    ILogger<Worker> logger,
    IOperationQueueConsumer operationQueueConsumer,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            IOperationQueueMessage? message;

            try
            {
                message = await operationQueueConsumer.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Operation message consumption failed unexpectedly.");
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            if (message is null)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            var operationId = message.OperationId;

            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<ProcessOperationHandler>();

                logger.LogInformation("Operation processing started: {OperationId}", operationId);

                var processed = await handler.HandleAsync(operationId, stoppingToken);

                if (processed)
                {
                    await message.CompleteAsync(stoppingToken);

                    logger.LogInformation("Operation processing finished: {OperationId}", operationId);
                }
                else
                {
                    await message.DeadLetterAsync("OperationNotFound", stoppingToken);

                    logger.LogWarning("Operation message ignored because operation was not found: {OperationId}", operationId);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Operation processing failed unexpectedly: {OperationId}", operationId);

                try
                {
                    await message.AbandonAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception abandonException)
                {
                    logger.LogError(
                        abandonException,
                        "Operation message could not be abandoned after processing failure: {OperationId}",
                        operationId);
                }
            }
        }
    }
}
