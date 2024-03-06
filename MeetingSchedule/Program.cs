using System;
using System.Collections.Generic;
using MimeKit;
using MailKit.Net.Smtp;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.CalendarComponents;
using System.Net.Mail;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using Ical.Net.Serialization;

public class Meeting
{
    public string Title { get; set; }
    public List<string> Emails { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

class Program
{
    static List<Meeting> meetings = new List<Meeting>();
    //static string teamsLink = "https://teams.microsoft.com/l/meetup-join/19%3ameeting_OGUxOTU2ZjItMTUxMy00MDcwLWExN2EtZDgzOTkzNjcxZTE3%40thread.v2/0?context=%7b%22Tid%22%3a%22ef0ead93-a56b-4c86-8af5-94c23d9f2548%22%2c%22Oid%22%3a%2277205bfc-1ca6-4fab-bd33-ba2e6ef682ac%22%7d";

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

        meetings.Add(meeting);

        foreach (var email in meeting.Emails)
        {
            SendNotificationEmail(email, meeting.Title, meeting.Date, meeting.StartTime, meeting.EndTime);
            AddToCalendar(email, meeting.Title, meeting.Date, meeting.StartTime, meeting.EndTime);
        }

        Console.WriteLine("Meeting scheduled successfully!");
    }

    static void ViewMeetings()
    {
        Console.WriteLine("Scheduled Meetings:");
        foreach (var meeting in meetings)
        {
            Console.WriteLine($"Title: {meeting.Title}, Emails: {string.Join(", ", meeting.Emails)}, " +
                              $"Date: {meeting.Date.ToShortDateString()}, Time: {meeting.StartTime} - {meeting.EndTime}, " 
                            );
        }
    }

    static void SendNotificationEmail(string toEmail, string title, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Guru", "coe@progience.com"));
        message.To.Add(new MailboxAddress("meet", toEmail));
        message.Subject = "Meeting Notification: " + title;

        var body = new TextPart("plain")
        {
            Text = $"You have a meeting scheduled on {date.ToShortDateString()} from {startTime} to {endTime}."
        };

        var multipart = new Multipart("mixed");
        multipart.Add(body);

        message.Body = multipart;

        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            client.Connect("smtp.office365.com", 587, false);
            client.Authenticate("coe@progience.com", EncryptDecrypt.Decrypt("BuYVota9Mw7H5CHPYpy06g=="));
            client.Send(message);
            client.Disconnect(true);
        }
    }

    static void AddToCalendar(string toEmail, string title, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        var calendar = new Ical.Net.Calendar();
        var evt = new CalendarEvent
        {
            Summary = title,
            Description = $"Meeting scheduled on {date.ToShortDateString()} from {startTime} to {endTime}",
            Start = new CalDateTime(date.Add(startTime)),
            End = new CalDateTime(date.Add(endTime)),
            Location = "https://teams.microsoft.com/l/meetup-join/19%3ameeting_OGUxOTU2ZjItMTUxMy00MDcwLWExN2EtZDgzOTkzNjcxZTE3%40thread.v2/0?context=%7b%22Tid%22%3a%22ef0ead93-a56b-4c86-8af5-94c23d9f2548%22%2c%22Oid%22%3a%2277205bfc-1ca6-4fab-bd33-ba2e6ef682ac%22%7d", // Set the location to Microsoft Teams
            IsAllDay = false,
        };
        calendar.Events.Add(evt);

        var serializer = new CalendarSerializer();
        var serializedCalendar = serializer.SerializeToString(calendar);

        var calendarBytes = Encoding.UTF8.GetBytes(serializedCalendar);
        var calendarBase64 = Convert.ToBase64String(calendarBytes);

        var calendarAttachment = new MimePart("text", "calendar")
        {
            Content = new MimeContent(new MemoryStream(calendarBytes)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = "meeting.ics"
        };

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Guru", "coe@progience.com"));
        message.To.Add(new MailboxAddress(toEmail, toEmail));
        message.Subject = $"Meeting Notification: {title}";

        var body = new TextPart("plain")
        {
            Text = $"You have a meeting scheduled on {date.ToShortDateString()} from {startTime} to {endTime}"
        };

        var multipart = new Multipart("mixed");
        multipart.Add(body);
        multipart.Add(calendarAttachment);

        message.Body = multipart;

        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            client.Connect("smtp.office365.com", 587, false);
            client.Authenticate("coe@progience.com", EncryptDecrypt.Decrypt("BuYVota9Mw7H5CHPYpy06g=="));
            client.Send(message);
            client.Disconnect(true);
        }
    }

}
