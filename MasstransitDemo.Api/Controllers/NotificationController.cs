using MassTransit;
using MassTransit.Scheduling;
using MasstransitDemo.Api.Models;
using MasstransitDemo.Shared;
using Microsoft.AspNetCore.Mvc;
using Quartz.Impl.AdoJobStore.Common;

namespace MasstransitDemo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    public readonly IPublishEndpoint publishEndpoint;
    public readonly IMessageScheduler scheduler;

    public NotificationController(IPublishEndpoint publishEndpoint, IMessageScheduler scheduler)
    {
        this.publishEndpoint = publishEndpoint;
        this.scheduler = scheduler;
    }

    [HttpPost]
    public async Task<IActionResult> Notify(NotificationDto notificationDto, CancellationToken ct)
    {
        var limit = 100;
        var numbers = Enumerable.Range(0, limit).ToList();

        ParallelOptions parallelOptions = new() {
            
        };

        ParallelOptions options = new() { 
            CancellationToken = ct,
            MaxDegreeOfParallelism = 100000
        };

        try {
            await Parallel.ForEachAsync(numbers, options, async (item, ct) =>
            {
                //...
                ct.ThrowIfCancellationRequested();
                //...
                //await publishEndpoint.Publish<INotificationCreated>(new {
                //    NotificationDate = notificationDto.NotificationDate,
                //    NotificationMessage = $"Publish {item}",
                //    NotificationType = notificationDto.NotificationType
                //});
                //...
                // Random.Shared.Next(5, 2000)
                await scheduler.SchedulePublish(TimeSpan.FromSeconds(30), new ScheduleNotification {
                    DeliveryTime = DateTime.Now,
                    EmailAddress = $"SchedulePublish{item}",
                    Body = "Sch"
                });

                ct.ThrowIfCancellationRequested();
                //...
            });

        }
        catch (OperationCanceledException ex) {
            // ...
            throw;
        }


        

        //await Parallel.ForEachAsync(numbers, parallelOptions, async (number, token) =>
        //{
        //    // Random.Shared.Next(5, 2000)
        //    var z = await scheduler.SchedulePublish(TimeSpan.FromSeconds(240), new ScheduleNotification {
        //        DeliveryTime = DateTime.Now,
        //        EmailAddress = $"SchedulePublish{number}",
        //        Body = "Sch"
        //    });
        //});

        return Ok();
    }

    [HttpPost("/notify2")]
    public async Task<IActionResult> Notify2(NotificationDto notificationDto, CancellationToken ct)
    {
        var limit = 2_000;
        var numbers = Enumerable.Range(0, limit).ToList();

        ParallelOptions parallelOptions = new() {

        };

        ParallelOptions options = new() {
            CancellationToken = ct,
            MaxDegreeOfParallelism = 100
        };

        try {
            await Parallel.ForEachAsync(numbers, options, async (item, ct) =>
            {
                //...
                ct.ThrowIfCancellationRequested();
                //...
                await publishEndpoint.Publish<INotificationCreated2>(new {
                    NotificationDate = notificationDto.NotificationDate,
                    NotificationMessage = $"Publish {item}",
                    NotificationType = notificationDto.NotificationType
                });
                //...
                ct.ThrowIfCancellationRequested();
                //...
            });

        }
        catch (OperationCanceledException ex) {
            // ...
            throw;
        }

        //await Parallel.ForEachAsync(numbers, parallelOptions, async (number, token) =>
        //{
        //    // Random.Shared.Next(5, 2000)
        //    var z = await scheduler.SchedulePublish(TimeSpan.FromSeconds(240), new ScheduleNotification {
        //        DeliveryTime = DateTime.Now,
        //        EmailAddress = $"SchedulePublish{number}",
        //        Body = "Sch"
        //    });
        //});

        return Ok();
    }


    [HttpPost("/notify3")]
    public async Task<IActionResult> Notify3(NotificationDto notificationDto, CancellationToken ct)
    {
        var limit = 200_000;
        var numbers = Enumerable.Range(0, limit).ToList();

        ParallelOptions parallelOptions = new() {

        };

        ParallelOptions options = new() {
            CancellationToken = ct,
            MaxDegreeOfParallelism = 100
        };

        try {
            await Parallel.ForEachAsync(numbers, options, async (item, ct) =>
            {
                //...
                ct.ThrowIfCancellationRequested();
                //...
                await publishEndpoint.Publish<INotificationCreated3>(new {
                    NotificationDate = notificationDto.NotificationDate,
                    NotificationMessage = $"Publish {item}",
                    NotificationType = notificationDto.NotificationType
                });
                //...
                ct.ThrowIfCancellationRequested();
                //...
            });

        }
        catch (OperationCanceledException ex) {
            // ...
            throw;
        }

        //await Parallel.ForEachAsync(numbers, parallelOptions, async (number, token) =>
        //{
        //    // Random.Shared.Next(5, 2000)
        //    var z = await scheduler.SchedulePublish(TimeSpan.FromSeconds(240), new ScheduleNotification {
        //        DeliveryTime = DateTime.Now,
        //        EmailAddress = $"SchedulePublish{number}",
        //        Body = "Sch"
        //    });
        //});

        return Ok();
    }
}