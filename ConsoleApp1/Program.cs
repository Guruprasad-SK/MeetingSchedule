using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Graph.Authentication;


public class Meeting
{
    public string Title { get; set; }
    public List<string> Emails { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string TeamsMeetingUrl { get; set; }
}

class Program
{
    static List<Meeting> meetings = new List<Meeting>();
    static IConfidentialClientApplication cca;
    static GraphServiceClient graphServiceClient;

    static async Task Main()
    {
        InitializeGraphClient();

        while (true)
        {
            Console.WriteLine("1. Schedule Meeting");
            Console.WriteLine("2. View Meetings");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ScheduleMeetingAsync();
                    break;
                case "2":
                    ViewMeetings();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void InitializeGraphClient()
    {
        cca = ConfidentialClientApplicationBuilder
            .Create("YourAppClientId")
            .WithClientSecret("YourAppClientSecret")
            .WithAuthority(new Uri("https://login.microsoftonline.com/YourTenantId"))
            .Build();

        graphServiceClient = new GraphServiceClient(new Microsoft.Graph.A.InteractiveBrowserProvider(cca, new[] { "User.Read", "OnlineMeetings.ReadWrite" }));
    }

    static async Task ScheduleMeetingAsync()
    {
        Console.Write("Enter title: ");
        string title = Console.ReadLine();

        Console.Write("Enter comma-separated emails: ");
        string emailInput = Console.ReadLine();
        List<string> emails = emailInput.Split(',').Select(e => e.Trim()).ToList();

        Console.Write("Enter date (yyyy-mm-dd): ");
        DateTime date = DateTime.Parse(Console.ReadLine());

        Console.Write("Enter start time (hh:mm): ");
        TimeSpan startTime = TimeSpan.Parse(Console.ReadLine());

        Console.Write("Enter end time (hh:mm): ");
        TimeSpan endTime = TimeSpan.Parse(Console.ReadLine());

        // Create a Teams meeting and get the meeting URL
        string teamsMeetingUrl = await CreateTeamsMeeting(title, date, startTime, endTime);

        Meeting meeting = new Meeting
        {
            Title = title,
            Emails = emails,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            TeamsMeetingUrl = teamsMeetingUrl
        };

        meetings.Add(meeting);

        foreach (var email in meeting.Emails)
        {
            SendNotificationEmail(email, meeting);
            AddToCalendar(email, meeting);
        }

        Console.WriteLine("Meeting scheduled successfully!");
    }

    static void ViewMeetings()
    {
        Console.WriteLine("Scheduled Meetings:");
        foreach (var meeting in meetings)
        {
            Console.WriteLine($"Title: {meeting.Title}, Emails: {string.Join(", ", meeting.Emails)}, " +
                              $"Date: {meeting.Date.ToShortDateString()}, Time: {meeting.StartTime} - {meeting.EndTime}, " +
                              $"Teams Meeting URL: {meeting.TeamsMeetingUrl}");
        }
    }

    static async Task<string> CreateTeamsMeeting(string title, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        var onlineMeeting = new OnlineMeeting
        {
            StartDateTime = date.Add(startTime),
            EndDateTime = date.Add(endTime),
            Subject = title,
        };

        var createdMeeting = await graphServiceClient.Me.OnlineMeetings
            .Request()
            .AddAsync(onlineMeeting);

        return createdMeeting.JoinWebUrl;
    }

    static void SendNotificationEmail(string toEmail, Meeting meeting)
    {
        // Implement your email sending logic here
        Console.WriteLine($"Email sent to {toEmail}: You have a meeting scheduled on {meeting.Date.ToShortDateString()} from {meeting.StartTime} to {meeting.EndTime}.\nJoin Microsoft Teams Meeting: {meeting.TeamsMeetingUrl}");
    }

    static void AddToCalendar(string toEmail, Meeting meeting)
    {
        // Implement your calendar event creation logic here
        Console.WriteLine($"Calendar event added for {toEmail} - Meeting: {meeting.Title}, Date: {meeting.Date.ToShortDateString()}, Time: {meeting.StartTime} - {meeting.EndTime}, Teams Meeting URL: {meeting.TeamsMeetingUrl}");
    }
}
