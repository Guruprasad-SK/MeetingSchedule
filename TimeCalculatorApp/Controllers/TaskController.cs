// TaskController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;


public class TaskController : Controller
{
    private static List<TaskModel> tasks = new List<TaskModel>();

    public ActionResult AssignTask()
    {
        return View();
    }

    [HttpPost]
    public ActionResult AssignTask(TaskModel task)
    {
        if (IsWithinWorkingHours(task.AssignedTime))
        {
            task.TaskId = tasks.Count + 1;
            tasks.Add(task);
            return RedirectToAction("TaskList");
        }
        else
        {
            ModelState.AddModelError("", "Task can only be assigned between 9 am to 6 pm, Monday to Friday.");
            return View(task);
        }
    }

    public ActionResult TaskList()
    {
        // Calculate Duration for each task
        foreach (var task in tasks)
        {
            task.Duration = CalculateDuration(task);
        }

        return View(tasks);
    }

    public ActionResult CompleteTask(int taskId)
    {
        var task = tasks.Find(t => t.TaskId == taskId);

        if (task != null)
        {
            return View(task);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost]
    public ActionResult CompleteTask(TaskModel completedTask)
    {
        var task = tasks.Find(t => t.TaskId == completedTask.TaskId);

        if (task != null)
        {
            task.CompletionTime = completedTask.CompletionTime;
            return RedirectToAction("TaskList");
        }
        else
        {
            return NotFound();
        }
    }

    private bool IsWithinWorkingHours(DateTime dateTime)
    {
        return dateTime.DayOfWeek >= DayOfWeek.Monday && dateTime.DayOfWeek <= DayOfWeek.Friday
               && dateTime.Hour >= 9 && dateTime.Hour <= 18;
    }

    private string CalculateDuration(TaskModel task)
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
