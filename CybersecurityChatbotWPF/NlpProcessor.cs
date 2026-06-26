using System;
using System.Text.RegularExpressions;

namespace CyberSecurityBot
{
    public class NlpProcessor
    {
        public bool IsTaskCommand(string input)
        {
            string text = input.ToLower();

            return text.Contains("add task")
                   || text.Contains("add a task")
                   || text.Contains("set reminder")
                   || text.Contains("set a reminder")
                   || text.Contains("remind me")
                   || text.Contains("create task")
                   || text.Contains("new task");
        }

        public bool IsViewTasksCommand(string input)
        {
            string text = input.ToLower();

            return text.Contains("show tasks")
                   || text.Contains("view tasks")
                   || text.Contains("my tasks")
                   || text.Contains("list tasks");
        }

        public bool IsActivityLogCommand(string input)
        {
            string text = input.ToLower();

            return text.Contains("show activity log")
                   || text.Contains("activity log")
                   || text.Contains("what have you done")
                   || text.Contains("recent actions");
        }

        public bool IsQuizCommand(string input)
        {
            string text = input.ToLower();

            return text.Contains("start quiz")
                   || text.Contains("open quiz")
                   || text.Contains("mini game")
                   || text.Contains("cybersecurity quiz")
                   || text.Contains("play quiz");
        }

        public string ExtractTaskTitle(string input)
        {
            string title = input.Trim();

            string[] patterns =
            {
                @"(?i)can you\s+",
                @"(?i)please\s+",
                @"(?i)add\s+(a\s+)?task\s*[-:]?\s*",
                @"(?i)create\s+(a\s+)?task\s*[-:]?\s*",
                @"(?i)new\s+task\s*[-:]?\s*",
                @"(?i)set\s+(a\s+)?reminder\s+(to|for)?\s*",
                @"(?i)remind\s+me\s+(to|about)?\s*"
            };

            foreach (string pattern in patterns)
            {
                title = Regex.Replace(title, pattern, "");
            }

            title = Regex.Replace(title, @"(?i)\b(in\s+\d+\s+(day|days|week|weeks)|tomorrow|today|next week)\b", "");
            title = Regex.Replace(title, @"(?i)\bon\s+\d{4}-\d{1,2}-\d{1,2}\b", "");

            title = title.Trim(' ', '.', ',', '-', ':');

            if (string.IsNullOrWhiteSpace(title))
            {
                title = "Cybersecurity task";
            }

            return char.ToUpper(title[0]) + title.Substring(1);
        }

        public DateTime? ExtractReminderDate(string input)
        {
            string text = input.ToLower();

            if (text.Contains("tomorrow"))
            {
                return DateTime.Today.AddDays(1).AddHours(9);
            }

            if (text.Contains("today"))
            {
                return DateTime.Now.AddMinutes(1);
            }

            if (text.Contains("next week"))
            {
                return DateTime.Today.AddDays(7).AddHours(9);
            }

            Match daysMatch = Regex.Match(text, @"in\s+(\d+)\s+(day|days)");
            if (daysMatch.Success && int.TryParse(daysMatch.Groups[1].Value, out int days))
            {
                return DateTime.Now.AddDays(days);
            }

            Match weeksMatch = Regex.Match(text, @"in\s+(\d+)\s+(week|weeks)");
            if (weeksMatch.Success && int.TryParse(weeksMatch.Groups[1].Value, out int weeks))
            {
                return DateTime.Now.AddDays(weeks * 7);
            }

            Match dateMatch = Regex.Match(text, @"on\s+(\d{4}-\d{1,2}-\d{1,2})");
            if (dateMatch.Success && DateTime.TryParse(dateMatch.Groups[1].Value, out DateTime date))
            {
                return date.Date.AddHours(9);
            }

            return null;
        }
    }
}