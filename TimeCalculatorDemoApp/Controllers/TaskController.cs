using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TimeCalculatorDemoApp.Models;

public class TaskController : Controller
{
    private static List<TaskModel> tasks = new List<TaskModel>();

    public IActionResult Index()
    {
        UpdateTotalTime();
        return View(tasks);
    }

    public IActionResult AssignTask()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AssignTask(TaskModel task)
    {
        task.Id = tasks.Count + 1;
        tasks.Add(task);
        return RedirectToAction("Index");
    }

    public IActionResult CompleteTask(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            return View(task);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult CompleteTask(int id, DateTime completionTime)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            task.Completed = true;
            task.CompletionTime = completionTime;
        }

        UpdateTotalTime();
        return RedirectToAction("Index");
    }

    private void UpdateTotalTime()
    {
        foreach (var task in tasks)
        {
            task.TotalTime = CalculateDuration(task);
        }
    }

    private double CalculateDuration(TaskModel task)
    {
        TimeSpan totalDuration = TimeSpan.Zero;

        DateTime currentDay = task.StartTime;
        while (currentDay <= task.CompletionTime)
        {
            if (IsWeekdayWithinOfficeHours(currentDay))
            {
                DateTime overlapStart = currentDay >= new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 9, 0, 0)
                    ? currentDay
                    : new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 9, 0, 0);

                DateTime overlapEnd = currentDay.Date == task.CompletionTime.Date
                    ? (task.CompletionTime <= new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 18, 0, 0)
                        ? task.CompletionTime
                        : new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 18, 0, 0))
                    : new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 18, 0, 0);

                if (overlapStart < overlapEnd)
                {
                    totalDuration += overlapEnd - overlapStart; 
                }
            }

            currentDay = currentDay.AddDays(1);
        }

        return totalDuration.TotalHours;
    }

    private bool IsWeekdayWithinOfficeHours(DateTime date)
    {
        // Check if the date is a weekday (Monday to Friday) and within office hours (9 AM to 6 PM)
        return date.DayOfWeek >= DayOfWeek.Monday && date.DayOfWeek <= DayOfWeek.Friday
            && date.Hour >= 9 && date.Hour < 18;
    }

    /* private double CalculateDuration(TaskModel task)
     {
         TimeSpan totalDuration = TimeSpan.Zero;

         DateTime currentDay = task.StartTime;
         while (currentDay <= task.CompletionTime)
         {
             if (IsWeekdayWithinOfficeHours(currentDay))
             {
                 DateTime overlapStart = currentDay > new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 9, 0, 0)
                     ? currentDay
                     : new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 9, 0, 0);

                 DateTime overlapEnd = currentDay.Date == task.CompletionTime.Date
                     ? (task.CompletionTime < new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 18, 0, 0)
                         ? task.CompletionTime
                         : new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 18, 0, 0))
                     : new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, 18, 0, 0);

                 if (overlapStart < overlapEnd)
                 {
                     totalDuration += overlapEnd - overlapStart;
                 }
             }

             currentDay = currentDay.AddDays(1);
         }

         return totalDuration.TotalHours;
     }

     private bool IsWeekend(DateTime date)
     {
         return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
     }

     private bool IsWeekdayWithinOfficeHours(DateTime date)
     {
         return !IsWeekend(date) && date.Hour >= 9 && date.Hour < 18;
     }*/
    public IActionResult EditTask(int id)
    {
        var task = tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            return View(task);
        }

        // Handle task not found
        return RedirectToAction("Index");
    }

   
  
}
