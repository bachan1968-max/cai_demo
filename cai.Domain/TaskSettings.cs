
namespace cai.Domain
{
    public class TaskSettings
    {
        public TaskParams [] Tasks { get; set; }
    }

    public class TaskParams
    {
        public string TaskName { get; set; }
        public string TaskSchedule { get; set; }
    }
}
