using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;
using Serilog.Events;
using System.Configuration;
using System.Reflection;
using OpenTelemetry.Logs;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();


builder.Services.AddQuartz(q =>
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
            c.CheckinMisfireThreshold = TimeSpan.FromSeconds(10);
            c.CheckinInterval = TimeSpan.FromSeconds(5);
        });
    });
});

builder.Services.Configure<RabbitMqTransportOptions>(x => x.Host = "rabbitmq");


builder.Services.AddMassTransit(x =>
{
    x.AddPublishMessageScheduler();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.PrefetchCount = 32; // applies to all receive endpoints

        cfg.UsePublishMessageScheduler();

        cfg.ConfigureEndpoints(context);

        cfg.Host("rabbitmq", h =>
        {
            h.ConfigureBatchPublish(x =>
            {
                x.Enabled = true;
                x.Timeout = TimeSpan.FromMilliseconds(2);
            });
        });
    });
});

//builder.Services.AddOpenTelemetry().WithTracing(builder =>
//{

//    builder.AddJaegerExporter(o =>
//            {
//                // these are the defaults
//                o.AgentHost = "localhost";
//                o.AgentPort = 6831;

//                o.Protocol = JaegerExportProtocol.HttpBinaryThrift;
//                o.HttpClientFactory = () =>
//                {
//                    HttpClient client = new HttpClient();
//                    client.DefaultRequestHeaders.Add("X-MyCustomHeader", "value");
//                    return client;
//                };
//            });
//    builder.AddQuartzInstrumentation();

//});

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}


//app.UseHttpsRedirection();

app.UseAuthorization();
 
app.MapControllers();

app.Run();
