using System;
using System.Collections.Generic;

public class TaskModel
{
    public int TaskId { get; set; }
    public string TaskName { get; set; }
    public DateTime AssignedTime { get; set; }
    public DateTime? CompletionTime { get; set; }
    public string Duration { get; set; }
}

class Program
{
    private static List<TaskModel> tasks = new List<TaskModel>();

    static void Main()
    {
        Console.WriteLine("Task Management Console App");

        while (true)
        {
            Console.WriteLine("\n1. Assign Task");
            Console.WriteLine("2. View Task List");
            Console.WriteLine("3. Complete Task");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option (1-4): ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        AssignTask();
                        break;
                    case 2:
                        ViewTaskList();
                        break;
                    case 3:
                        CompleteTask();
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please enter a number between 1 and 4.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
    }

    private static void AssignTask()
    {
        Console.WriteLine("\nAssign Task");

        TaskModel task = new TaskModel();

        Console.Write("Enter Task Name: ");
        task.TaskName = Console.ReadLine();

        Console.Write("Enter Assigned Time (yyyy-MM-dd HH:mm:ss): ");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime assignedTime))
        {
            task.AssignedTime = assignedTime;
        }
        else
        {
            Console.WriteLine("Invalid date and time format.");
            return;
        }

        if (IsWithinWorkingHours(task.AssignedTime))
        {
            task.TaskId = tasks.Count + 1;
            tasks.Add(task);
            Console.WriteLine("Task assigned successfully.");
        }
        else
        {
            Console.WriteLine("Task can only be assigned between 9 am to 6 pm, Monday to Friday.");
        }
    }

    private static void ViewTaskList()
    {
        Console.WriteLine("\nTask List");

        // Calculate Duration for each task
        foreach (var task in tasks)
        {
            task.Duration = CalculateDuration(task);
            Console.WriteLine($"Task ID: {task.TaskId}, Task Name: {task.TaskName}, Assigned Time: {task.AssignedTime}, Duration: {task.Duration}");
        }
    }

    private static void CompleteTask()
    {
        Console.WriteLine("\nComplete Task");

        Console.Write("Enter Task ID to complete: ");
        if (int.TryParse(Console.ReadLine(), out int taskId))
        {
            var task = tasks.Find(t => t.TaskId == taskId);

            if (task != null)
            {
                Console.Write("Enter Completion Time (yyyy-MM-dd HH:mm:ss): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime completionTime))
                {
                    task.CompletionTime = completionTime;
                    Console.WriteLine("Task completed successfully.");
                }
                else
                {
                    Console.WriteLine("Invalid date and time format.");
                }
            }
            else
            {
                Console.WriteLine($"Task with ID {taskId} not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a number.");
        }
    }

    private static bool IsWithinWorkingHours(DateTime dateTime)
    {
        return dateTime.DayOfWeek >= DayOfWeek.Monday && dateTime.DayOfWeek <= DayOfWeek.Friday
               && dateTime.Hour >= 9 && dateTime.Hour <= 18;
    }

    private static string CalculateDuration(TaskModel task)
    {
        TimeSpan totalDuration = TimeSpan.Zero;

        DateTime currentDay = task.AssignedTime;
        while (currentDay <= task.CompletionTime)
        {
            if (IsWithinWorkingHours(currentDay))
            {
                DateTime overlapStart = currentDay >= task.AssignedTime
                    ? currentDay
                    : task.AssignedTime;

                DateTime overlapEnd = currentDay.Date == task.CompletionTime.Value.Date
                    ? (task.CompletionTime.Value <= new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 18, 0, 0)
                        ? task.CompletionTime.Value
                        : new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 18, 0, 0))
                    : new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 18, 0, 0);

                if (overlapStart < overlapEnd)
                {
                    totalDuration += overlapEnd - overlapStart;
                }
            }

            currentDay = currentDay.AddDays(1);

            // If it's the first day, adjust the starting time to 9 AM on the next day
            if (currentDay.Date == task.AssignedTime.Date.AddDays(1).Date)
            {
                currentDay = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 9, 0, 0);
            }
        }

        // Format the total duration as hh:mm
        return $"{(int)totalDuration.TotalHours:00}:{totalDuration.Minutes:00}";
    }
}
