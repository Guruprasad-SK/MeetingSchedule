namespace TimeCalculatorDemoApp.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string EmployeeName { get; set; }
        public bool Completed { get; set; }
        public double TotalTime { get; set; }
        public DateTime CompletionTime { get; set; }
    }
}
