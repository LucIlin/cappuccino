using Agents.LLM;
using Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<LLMConfiguration>(
    builder.Configuration.GetSection(nameof(LLMConfiguration))
);

builder.Services.AddHostedService<LocalOllamaTestWorker>();

var host = builder.Build();

host.Run();