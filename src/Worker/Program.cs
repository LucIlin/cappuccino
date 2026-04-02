using Worker;
using Microsoft.Agents.AI;
using OpenAI;

using Worker.Settings;
using Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<LLMConfiguration>(
    builder.Configuration.GetSection(nameof(LLMConfiguration))
);

builder.Services.AddHostedService<TestAgentWorker>();

var host = builder.Build();

host.Run();