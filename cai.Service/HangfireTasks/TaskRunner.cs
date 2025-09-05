using System;
using Hangfire;
using cai.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cai.Service.HangfireTasks
{
    public class TaskRunner
    {
        private static IOptions<TaskSettings> _taskSettings;
        private readonly ILogger<TaskRunner> _logger;

        public TaskRunner(IOptions<TaskSettings> taskSettings, ILogger<TaskRunner> logger)
        {
            _taskSettings = taskSettings ?? throw new ArgumentNullException(nameof(taskSettings));
           _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void StartAllTasks()
        {
            foreach (var task in _taskSettings.Value.Tasks)
            {
                var taskType = Enum.Parse<TasksHfEnumeration>(task.TaskName);
                var taskCreated = true;
                var hfOptions = new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local
                };
                switch (taskType)
                {
                    case TasksHfEnumeration.GetB2bStock:
                        {
                            RecurringJob.AddOrUpdate<GetB2bStock>(task.TaskName,
                                w => w.DoJobAsync(taskType, null), task.TaskSchedule, hfOptions);
                            break;
                        }
                    default:
                        {
                            taskCreated = false;
                            _logger.LogInformation("Wrong task: {TaskName} не поддерживается!", task.TaskName);
                            break;
                        }
                }
                if (taskCreated)
                {
                    _logger.LogInformation("Task created: {TaskName}", task.TaskName);
                }
            }
        }
    }

}
