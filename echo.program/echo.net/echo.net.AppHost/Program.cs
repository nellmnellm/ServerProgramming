var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.echo_net_ApiService>("apiservice");

builder.AddProject<Projects.echo_net_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
