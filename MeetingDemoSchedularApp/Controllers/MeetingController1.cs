using System;
using System.Collections.Generic;
using MeetingDemoSchedularApp.Models;
using Microsoft.AspNetCore.Mvc;
using MeetingDemoSchedularApp.ViewModels;
using MimeKit;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using System.Text;

namespace MeetingDemoSchedularApp.Controllers
{
    public class MeetingController : Controller
    {
        private static List<Meeting> meetings = new List<Meeting>();

        public IActionResult Index()
        {
            var viewModel = new MeetingViewModel
            {
                Meetings = meetings,
                NewMeeting = new Meeting()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ScheduleMeeting(Meeting newMeeting)
        {
            if (ModelState.IsValid)
            {
                meetings.Add(newMeeting);

                foreach (var email in newMeeting.Emails)
                {
                    SendNotificationEmail(email, newMeeting.Title, newMeeting.Date, newMeeting.StartTime, newMeeting.EndTime);
                    AddToCalendar(email, newMeeting.Title, newMeeting.Date, newMeeting.StartTime, newMeeting.EndTime);
                }

                return RedirectToAction("Index");
            }

            // If the model state is not valid, return to the form with error messages
            var viewModel = new MeetingViewModel
            {
                Meetings = meetings,
                NewMeeting = newMeeting
            };

            return View("Index", viewModel);
        }

        public IActionResult ViewMeetings()
        {
            var viewModel = new MeetingViewModel
            {
                Meetings = meetings,
                NewMeeting = new Meeting()
            };

            return View(viewModel);
        }

        private void SendNotificationEmail(string toEmail, string title, DateTime date, TimeSpan startTime, TimeSpan endTime)
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

        private void AddToCalendar(string toEmail, string title, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            var calendar = new Ical.Net.Calendar();
            var evt = new CalendarEvent
            {
                Summary = title,
                Description = $"Meeting scheduled on {date.ToShortDateString()} from {startTime} to {endTime}.",
                Start = new CalDateTime(date.Add(startTime)),
                End = new CalDateTime(date.Add(endTime)),
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
                Text = $"You have a meeting scheduled on {date.ToShortDateString()} from {startTime} to {endTime}."
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
}
