IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<IMessageStore, MessageService>();
        services.AddSingleton(provider =>
        {
            var configuration = hostContext.Configuration.GetSection("SmtpConfiguration").Get<SmtpConfiguration>();
            var options = new SmtpServerOptionsBuilder().ServerName(configuration.ServerName).Port(configuration.Port).Build();
            return new SmtpServer.SmtpServer(options, provider.GetRequiredService<IServiceProvider>());
        });
        services.AddHttpClient();
        services.AddSingleton<HttpService>();
    })
    .Build();

await host.RunAsync();
