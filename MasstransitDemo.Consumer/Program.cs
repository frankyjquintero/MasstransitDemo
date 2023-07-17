using MassTransit;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Quartz;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using System.Diagnostics;
using OpenTelemetry.Resources;

var builder = Host.CreateDefaultBuilder(args);


builder.ConfigureServices((hostContext, services) =>
{

    services.AddQuartz(q =>
    {
        q.SchedulerName = "MassTransit-Scheduler";
        q.SchedulerId = "AUTO";

        q.UseMicrosoftDependencyInjectionJobFactory();

        q.UseDefaultThreadPool(tp =>
        {
            tp.MaxConcurrency = 10;
        });

        q.UseTimeZoneConverter();

        q.UsePersistentStore(s =>
        {
            s.UseProperties = true;
            s.RetryInterval = TimeSpan.FromSeconds(15);

            s.UseSqlServer("Server=tcp:mssql;Database=quartznet;Persist Security Info=False;User ID=sa;Password=Quartz!DockerP4ss;Encrypt=False;TrustServerCertificate=True;");

            s.UseJsonSerializer();

            s.UseClustering(c =>
            {
                c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                c.CheckinInterval = TimeSpan.FromSeconds(10);
            });
        });
    });

    services.Configure<RabbitMqTransportOptions>(x => x.Host = "rabbitmq");

    services.AddMassTransit(x =>
    {
        var entryAssembly = Assembly.GetExecutingAssembly();

        x.AddConsumers(entryAssembly);
        x.AddQuartzConsumers();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.PrefetchCount = 120;

            cfg.UsePublishMessageScheduler();

            cfg.ConfigureEndpoints(context);
        });
    });

    //services.AddOpenTelemetry().WithTracing(builder =>
    //{

    //    builder.AddJaegerExporter(o =>
    //    {
    //        // these are the defaults
    //        o.AgentHost = "localhost";
    //        o.AgentPort = 6831;

    //        o.Protocol = JaegerExportProtocol.HttpBinaryThrift;
    //        o.HttpClientFactory = () =>
    //        {
    //            HttpClient client = new HttpClient();
    //            client.DefaultRequestHeaders.Add("X-MyCustomHeader", "value");
    //            return client;
    //        };
    //    });
    //    builder.AddSource("ConsumerResource")
    //        .ConfigureResource(resource => resource
    //            .AddService("ConsumerService"))
    //        .AddQuartzInstrumentation();

    //});
});

var app = builder.Build();

app.Run();