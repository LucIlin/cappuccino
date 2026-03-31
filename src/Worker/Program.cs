using Worker;
using Worker.Settings;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<LLMConfiguration>(
    builder.Configuration.GetSection(nameof(LLMConfiguration))
);

builder.Services.AddHostedService<Worker.Worker>();

var host = builder.Build();

host.Run();