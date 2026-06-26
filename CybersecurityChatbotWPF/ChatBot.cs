using System;
using System.Linq;

namespace CyberSecurityBot
{
    public class ChatBot
    {
        private readonly KeywordResponder _keywords;
        private readonly SentimentDetector _sentiment;
        private readonly MemoryStore _memory;
        private readonly TaskRepository _tasks;
        private readonly ActivityLog _activityLog;
        private readonly NlpProcessor _nlp;

        private bool _awaitingName = true;
        private string _lastTopic = "";

        public ChatBot(TaskRepository tasks, ActivityLog activityLog)
        {
            _keywords = new KeywordResponder();
            _sentiment = new SentimentDetector();
            _memory = new MemoryStore();
            _tasks = tasks;
            _activityLog = activityLog;
            _nlp = new NlpProcessor();
        }

        public string GetGreeting()
        {
            return "Welcome to the Cybersecurity Awareness Assistant.\n\nBefore we begin learning about online safety, what is your name?";
        }

        public string ProcessInput(string input)
        {
            string cleanInput = input?.Trim() ?? "";

            if (string.IsNullOrEmpty(cleanInput))
            {
                return "I did not receive any input. Please type a question or command.";
            }

            if (_awaitingName)
            {
                _memory.Store("name", cleanInput);
                _awaitingName = false;

                _activityLog.Add($"User introduced themselves as {cleanInput}");

                return $"Nice to meet you, {_memory.UserName}. How can I help you stay safe online today?\n\n" +
                       "You can ask about passwords, phishing, privacy, scams, browsing, tasks, reminders, quiz, or activity log.";
            }

            string lowerInput = cleanInput.ToLower();

            if (lowerInput == "exit" || lowerInput == "quit")
            {
                _activityLog.Add("User ended the chat session");
                return "Stay secure online. You can now close the application.";
            }

            if (_nlp.IsActivityLogCommand(lowerInput))
            {
                _activityLog.Add("Activity log requested through NLP command");

                var recentActions = _activityLog.GetRecent(10);

                if (recentActions.Count == 0)
                {
                    return "No activity has been recorded yet.";
                }

                return "Here is a summary of recent actions:\n" +
                       string.Join("\n", recentActions.Select((action, index) => $"{index + 1}. {action}"));
            }

            if (_nlp.IsViewTasksCommand(lowerInput))
            {
                _activityLog.Add("Task list requested through NLP command");

                var allTasks = _tasks.GetAllTasks();

                if (allTasks.Count == 0)
                {
                    return "You currently have no cybersecurity tasks saved.";
                }

                return "Here are your cybersecurity tasks:\n" +
                       string.Join("\n", allTasks.Select((task, index) => $"{index + 1}. {task}"));
            }

            if (_nlp.IsQuizCommand(lowerInput))
            {
                _activityLog.Add("Quiz requested through NLP command");
                return "Open the Cyber Quiz tab and click Start Quiz to test your cybersecurity knowledge.";
            }

            if (_nlp.IsTaskCommand(lowerInput))
            {
                string title = _nlp.ExtractTaskTitle(cleanInput);
                DateTime? reminderDate = _nlp.ExtractReminderDate(cleanInput);

                string description = $"Cybersecurity task created from chat request: \"{cleanInput}\"";

                int taskId = _tasks.AddTask(title, description, reminderDate);

                if (taskId == -1)
                {
                    return "I understood the task request, but the database is not connected. Please make sure MySQL is running in XAMPP and restart the application.";
                }

                string reminderText = reminderDate.HasValue
                    ? $" Reminder set for {reminderDate.Value:dd MMM yyyy HH:mm}."
                    : " No reminder was set.";

                _activityLog.Add($"Task added through NLP: '{title}'.{reminderText}");

                return $"Task added: '{title}'.{reminderText}";
            }

            if (lowerInput.Contains("how are you"))
            {
                return $"I am running normally, {_memory.UserName}, and ready to assist with cybersecurity awareness.";
            }

            if (lowerInput.Contains("purpose") || lowerInput.Contains("what can i ask"))
            {
                return "My purpose is to educate users about online safety. You can ask about passwords, phishing, browsing, scams, privacy, tasks, reminders, quiz, or activity log.";
            }

            if (lowerInput.Contains("tell me more") || lowerInput.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                {
                    return _keywords.GetResponse(_lastTopic, out _);
                }

                return "We have not started a cybersecurity topic yet. Which area would you like me to explain?";
            }

            Sentiment detectedMood = _sentiment.Detect(cleanInput);
            string moodOpener = _sentiment.GetSentimentResponse(detectedMood);

            string botTip = _keywords.GetResponse(cleanInput, out string matchedKeyword);

            if (!string.IsNullOrEmpty(matchedKeyword))
            {
                _lastTopic = matchedKeyword;

                if (detectedMood == Sentiment.Curious || string.IsNullOrEmpty(_memory.FavouriteTopic))
                {
                    _memory.Store("topic", matchedKeyword);
                }

                _activityLog.Add($"Cybersecurity response given for topic: {matchedKeyword}");

                return $"{moodOpener}{_memory.GetPersonalisedOpener()}{botTip}";
            }

            if (!string.IsNullOrEmpty(moodOpener))
            {
                return moodOpener + "Please tell me more specifically. Are you asking about passwords, browsing safety, scams, privacy, or phishing?";
            }

            return $"I could not understand that command clearly, {_memory.UserName}. Try asking about passwords, phishing, privacy, scams, browsing, tasks, reminders, quiz, or activity log.";
        }
    }
}