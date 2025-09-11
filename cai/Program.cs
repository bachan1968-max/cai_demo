using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using Serilog;
using cai.Domain;
using cai.Service.HangfireTasks;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using cai.Service.B2bInteraction;
using cai.Service.EmailSender;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;
using cai.Service.ControllerService;
using cai.Service.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using cai.Service.HttpClients;

namespace cai
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                    .CreateBootstrapLogger();
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Error");
            }
            finally
            {
                Log.Information("Application finished");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .Enrich.WithProperty("Server", Environment.MachineName)
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services);

    })
                .ConfigureServices((context, services) =>
                {
                    var config = context.Configuration;
                    services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));
                    services.Configure<HttpClientSettings>(context.Configuration.GetSection("HttpClientSettings"));
                    services.Configure<TaskSettings>(context.Configuration.GetSection("TaskSettings"));
                    services.Configure<SmtpConfiguration>(context.Configuration.GetSection("SmtpConfiguration"));

                    services.AddSingleton<TaskSettings>();
                    services.AddSingleton<AppSettings>();
                    services.AddSingleton<HttpClientSettings>();
                    services.AddTransient<TaskRunner>();
                    services.AddTransient<IB2bRepository, B2bRepository>();
                    services.AddTransient<IEmailRepository, EmailRepository>();
                    services.AddTransient<IControllerService, ControllerService>();
                    services.AddTransient<IDbRepo, DbRepo>();
                    services.AddSingleton<SmtpConfiguration>();

                    services.AddSingleton<SmtpClientFactory>();

                    services.AddFluentEmail("no-reply@marvel.ru")
                            .AddRazorRenderer();
                    services.TryAdd(ServiceDescriptor.Scoped<ISender>(provider
                        => new SmtpSender(provider.GetRequiredService<SmtpClientFactory>().Create())));
                    services.AddSingleton(new CsvHelper.Factory());
                    services.AddSingleton(new CsvWriterFactory(
                    services.BuildServiceProvider().GetRequiredService<CsvHelper.Factory>()));

                    services.AddScoped(provider =>
                    {
                        var connectionString = config.GetConnectionString("DefaultDB");
                        var builder = new DbContextOptionsBuilder<CaiDbContext>();
                        var options = builder.UseSqlServer(connectionString).Options;

                        return new CaiDbContext(options);
                    }); 

                    #region B2bHttpClient
                                services.AddHttpClient<B2bHttpClient>()
                                    //5XX, 408, System.Net.Http.HttpRequestException (Network failures)
                                    .AddPolicyHandler((services, request) => HttpPolicyExtensions.HandleTransientHttpError()
                                    .OrResult(r => r.StatusCode != HttpStatusCode.OK)
                                    .WaitAndRetryAsync(
                                        services.GetService<HttpClientSettings>().RetryRequestCount,
                                        retryAttempt => TimeSpan.FromSeconds(services.GetService<HttpClientSettings>().RetryRequestInterval * retryAttempt),

                                    onRetry: (exception, timespan, retryAttempt, context) =>
                                    {
                                        services.GetService<ILogger<B2bHttpClient>>()?
                                            .LogError("Delay on {delay} secongs, then retry {retry}. Response {@Response}",
                                            timespan.TotalSeconds, retryAttempt, exception);
                                    }
                                    ));
                                #endregion

                    services.AddHangfireProcessing(config);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseWindowsService();
    }
}
