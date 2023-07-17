namespace MasstransitDemo.Shared;

public interface INotificationCreated
{
    DateTime NotificationDate { get; }
    string NotificationMessage { get; }
    NotificationType NotificationType { get; }
}

public interface INotificationCreated2
{
    DateTime NotificationDate { get; }
    string NotificationMessage { get; }
    NotificationType NotificationType { get; }
}

public interface INotificationCreated3
{
    DateTime NotificationDate { get; }
    string NotificationMessage { get; }
    NotificationType NotificationType { get; }
}


public record ScheduleNotification
{
    public DateTime DeliveryTime { get; init; }
    public string EmailAddress { get; init; }
    public string Body { get; init; }
}

public record SendNotification
{
    public string EmailAddress { get; init; }
    public string Body { get; init; }
}