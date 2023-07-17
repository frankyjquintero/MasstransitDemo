using MassTransit;
using MasstransitDemo.Shared;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MasstransitDemo.Consumer;

public class NotificationCreatedConsumer : IConsumer<Batch<INotificationCreated>>
{
    public async Task Consume(ConsumeContext<Batch<INotificationCreated>> context)
    {
        var serializedMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { });

        Console.WriteLine($"NotificationCreated event consumed. Message: {serializedMessage}");
    }
}

public class NotificationCreated2Consumer : IConsumer<INotificationCreated2>
{
    public async Task Consume(ConsumeContext<INotificationCreated2> context)
    {
        var serializedMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { });

        Console.WriteLine($"NotificationCreated2 event consumed. Message: {serializedMessage}");
    }
}

public class NotificationCreated3Consumer : IConsumer<Batch<INotificationCreated3>>
{
    public async Task Consume(ConsumeContext<Batch<INotificationCreated3>> context)
    {
        var serializedMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { });

        Console.WriteLine($"NotificationCreated3 event consumed. Message: {serializedMessage}");
    }
}


public class SampleConsumer :
    IConsumer<ScheduleNotification>
{
    readonly ILogger<SampleConsumer> _logger;

    public SampleConsumer(ILogger<SampleConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<ScheduleNotification> context)
    {
        var serializedMessage = JsonSerializer.Serialize(context.Message, new JsonSerializerOptions { });

        Console.WriteLine($"Received scheduled message. Message: {serializedMessage}");

        return Task.CompletedTask;
    }
}