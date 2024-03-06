// TaskModel.cs
using System;

public class TaskModel
{
    public int TaskId { get; set; }
    public string TaskName { get; set; }
    public DateTime AssignedTime { get; set; }
    public string EmployeeName { get; set; }
    public string ProjectDescription { get; set; }
    public DateTime? CompletionTime { get; set; }
    public string Duration { get; set; }

}
