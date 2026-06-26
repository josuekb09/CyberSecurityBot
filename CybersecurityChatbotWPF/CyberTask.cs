using System;

namespace CyberSecurityBot
{
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Status => IsCompleted ? "Completed" : "Pending";

        public override string ToString()
        {
            string reminder = ReminderDate.HasValue
                ? ReminderDate.Value.ToString("dd MMM yyyy HH:mm")
                : "No reminder";

            return $"[{Status}] #{Id} {Title} | {Description} | Reminder: {reminder}";
        }
    }
}