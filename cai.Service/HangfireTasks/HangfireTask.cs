using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using cai.Domain;

namespace cai.Service.HangfireTasks
{
        public abstract class HangfireTask : ITaskInterface
        {
            protected readonly ILogger<HangfireTask> _logger;
            protected TasksHfEnumeration TaskHfEnum { get; set; }
            protected string TaskName { get; set; } = "No_Name";

            public HangfireTask(ILogger<HangfireTask> logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
            }

            public async Task DoJobAsync(TasksHfEnumeration task, PerformContext context)
            {
                TaskHfEnum = task;

                var currentTask = task.ToString();
                if (null != currentTask)
                {
                    TaskName = currentTask;
                }
                using (_logger.BeginScope(TaskName))
                {
                    try
                    {
                        if (JobIsAlreadyRunning(TaskHfEnum, context))
                        {
                            return;
                        }
                        await DoJobAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exeption in task \"{TaskName}\" ", TaskName);
                    }
                }
            }

            protected abstract Task DoJobAsync();

            private static bool JobIsAlreadyRunning(TasksHfEnumeration tn, PerformContext context)
            {
                var hMon = JobStorage.Current.GetMonitoringApi();
                var currentJobId = context.BackgroundJob.Id;

                var jobs = hMon.ProcessingJobs(0, int.MaxValue);
                var jobsUpdated = jobs.Where(x => x.Key != currentJobId).ToList();

                foreach (var j in jobsUpdated)
                {
                    var details = hMon.JobDetails(j.Key);
                    if (details?.Job?.Method?.Name != null)
                    {
                        var taskName = (TasksHfEnumeration)details.Job?.Args[0];
                        if (taskName == tn)
                            return true;
                    }
                }

                return false;
            }
        }

    }
