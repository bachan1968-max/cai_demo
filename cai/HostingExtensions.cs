using Hangfire;
using Hangfire.Dashboard;
using cai.Service.HangfireTasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace cai
{
    public static class HostingExtensions
    {
        private class DashboardAuthFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context) => true;
        }

        public static IServiceCollection AddHangfireProcessing(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseInMemoryStorage());
            services.AddHangfireServer();

            return services;
        }

        public static IApplicationBuilder AddHangfireProcessing(this IApplicationBuilder app, TaskRunner runner)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new DashboardAuthFilter() },
                StatsPollingInterval = 0
            });
            runner.StartAllTasks();
            return app;
        }
    }
}
