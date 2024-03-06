using System;
using System.Collections.Generic;
using MimeKit;
using MailKit.Net.Smtp;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.CalendarComponents;
using System.IO;
using System.Net.Mail;
using System.Text;
using Ical.Net.Serialization;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Security.Principal;
using Microsoft.Graph.Models;




public class Meeting
{
    public string Title { get; set; }
    public List<string> Emails { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string TeamsMeetingLink { get; set; }
}

class Program
{
    // Update these constants with your Microsoft Graph API information
    private const string ClientId = "YOUR_CLIENT_ID";
    private const string TenantId = "YOUR_TENANT_ID";
    private const string ClientSecret = "YOUR_CLIENT_SECRET";

    private static List<Meeting> meetings = new List<Meeting>();

    static void Main()
    {
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
                    ScheduleMeeting();
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

    static void ScheduleMeeting()
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

        Meeting meeting = new Meeting
        {
            Title = title,
            Emails = emails,
            Date = date,
            StartTime = startTime,
            EndTime = endTime
        };

        // Generate Teams meeting link using Microsoft Graph API
        meeting.TeamsMeetingLink = GenerateTeamsMeetingLink(meeting);

        meetings.Add(meeting);

        foreach (var email in meeting.Emails)
        {
            SendNotificationEmail(email, meeting.Title, meeting.Date, meeting.StartTime, meeting.EndTime);
            AddToCalendar(email, meeting.Title, meeting.Date, meeting.StartTime, meeting.EndTime);
        }

        Console.WriteLine("Meeting scheduled successfully!");

        // Launch Microsoft Teams with the deep link to the scheduled meeting
        LaunchTeamsMeeting(meeting.TeamsMeetingLink);
    }

    static void ViewMeetings()
    {
        Console.WriteLine("Scheduled Meetings:");
        foreach (var meeting in meetings)
        {
            Console.WriteLine($"Title: {meeting.Title}, Emails: {string.Join(", ", meeting.Emails)}, Date: {meeting.Date.ToShortDateString()}, Time: {meeting.StartTime} - {meeting.EndTime}, Teams Meeting Link: {meeting.TeamsMeetingLink}");
        }
    }

    static string GenerateTeamsMeetingLink(Meeting meeting)
    {
        var graphServiceClient = GetGraphServiceClient();

        var onlineMeeting = new Microsoft.Graph.Models.OnlineMeeting
        {
            StartDateTime = meeting.Date.Add(meeting.StartTime),
            EndDateTime = meeting.Date.Add(meeting.EndTime),
            Subject = meeting.Title,
            Participants = new MeetingParticipants
            {
                Organizers = new List<MeetingParticipantInfo>
                {
                    new MeetingParticipantInfo
                    {
                        Identity = new IdentitySet
                        {
                            User = new Identity
                            {
                                Id = "YOUR_ORGANIZER_USER_ID" // Replace with the organizer's user ID
                            }
                        }
                    }
                }
            }
        };

        var createdMeeting = graphServiceClient.Me.OnlineMeetings
            .Request()
            .AddAsync(onlineMeeting)
            .Result;

        return createdMeeting.JoinWebUrl;
    }

    static void SendNotificationEmail(string toEmail, string title, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        // Existing code for sending email notification
    }

    static void AddToCalendar(string toEmail, string title, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        // Existing code for adding to calendar
    }

    static void LaunchTeamsMeeting(string teamsMeetingLink)
    {
        // Existing code for launching Teams meeting
    }

    static GraphServiceClient GetGraphServiceClient()
    {
        var confidentialClientApplication = ConfidentialClientApplicationBuilder
            .Create(ClientId)
            .WithClientSecret(ClientSecret)
            .WithAuthority(new Uri($"https://login.microsoftonline.com/{TenantId}"))
            .Build();

        var authProvider = new ClientCredentialProvider(confidentialClientApplication);

        return new GraphServiceClient(authProvider);
    }
}
