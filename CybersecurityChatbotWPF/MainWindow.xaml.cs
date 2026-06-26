using System;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CyberSecurityBot
{
    public partial class MainWindow : Window
    {
        private readonly ChatBot _bot;
        private readonly TaskRepository _taskRepository;
        private readonly ActivityLog _activityLog;
        private readonly QuizManager _quizManager;
        private readonly DispatcherTimer _reminderTimer;

        private bool _answerSubmitted;

        public MainWindow()
        {
            InitializeComponent();

            _activityLog = new ActivityLog();
            _taskRepository = new TaskRepository();
            _quizManager = new QuizManager();
            _bot = new ChatBot(_taskRepository, _activityLog);

            PlayGreetingAudio();

            AppendBotMessage(_bot.GetGreeting());

            if (!_taskRepository.IsDatabaseReady)
            {
                AppendBotMessage(
                    "Database warning: MySQL is not connected yet.\n\n" +
                    "Technical error: " + _taskRepository.LastError + "\n\n" +
                    "Make sure XAMPP MySQL is running on port 3306, then restart the application."
                );

                _activityLog.Add("Database connection failed: " + _taskRepository.LastError);
            }
            else
            {
                AppendBotMessage("Database connected successfully. Your tasks and reminders will be saved.");
                _activityLog.Add("Application started and MySQL database connected successfully");
            }

            RefreshTaskList();
            RefreshActivityLog();

            _reminderTimer = new DispatcherTimer();
            _reminderTimer.Interval = TimeSpan.FromMinutes(1);
            _reminderTimer.Tick += ReminderTimer_Tick;
            _reminderTimer.Start();

            CheckReminders();
        }

        private void PlayGreetingAudio()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("greeting.wav");
                player.Play();
            }
            catch
            {
                // The app continues even if the audio file is missing.
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendChatMessage();
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendChatMessage();
            }
        }

        private void SendChatMessage()
        {
            string userInput = TxtInput.Text.Trim();

            if (string.IsNullOrEmpty(userInput))
            {
                return;
            }

            AppendUserMessage(userInput);

            string response = _bot.ProcessInput(userInput);
            AppendBotMessage(response);

            TxtInput.Clear();

            RefreshTaskList();
            RefreshActivityLog();
        }

        private void AppendUserMessage(string message)
        {
            ChatDisplay.Text += $"You: {message}\n";
            ChatScroll.ScrollToEnd();
        }

        private void AppendBotMessage(string message)
        {
            ChatDisplay.Text += $"Chatbot: {message}\n\n";
            ChatScroll.ScrollToEnd();
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text.Trim();
            string description = TaskDescriptionBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show(
                    "Please enter a task title.",
                    "Missing Title",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                description = "No description provided.";
            }

            DateTime? reminderDate = null;

            if (ReminderDatePicker.SelectedDate.HasValue)
            {
                reminderDate = ReminderDatePicker.SelectedDate.Value.Date.AddHours(9);
            }

            if (int.TryParse(ReminderDaysBox.Text.Trim(), out int days) && days >= 0)
            {
                reminderDate = DateTime.Now.AddDays(days);
            }

            int id = _taskRepository.AddTask(title, description, reminderDate);

            if (id == -1)
            {
                MessageBox.Show(
                    "Task could not be saved because the database is not connected.\n\n" +
                    "Technical error:\n" + _taskRepository.LastError + "\n\n" +
                    "Make sure XAMPP MySQL is running on port 3306.",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                _activityLog.Add("Task save failed: " + _taskRepository.LastError);
                RefreshActivityLog();

                return;
            }

            string reminderInfo = reminderDate.HasValue
                ? $" Reminder set for {reminderDate.Value:dd MMM yyyy HH:mm}."
                : " No reminder set.";

            _activityLog.Add($"Task added from GUI: '{title}'.{reminderInfo}");

            TaskTitleBox.Clear();
            TaskDescriptionBox.Clear();
            ReminderDaysBox.Clear();
            ReminderDatePicker.SelectedDate = null;

            RefreshTaskList();
            RefreshActivityLog();

            MessageBox.Show(
                "Task added successfully.",
                "Task Added",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void BtnRefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            RefreshTaskList();
            _activityLog.Add("Task list refreshed");
            RefreshActivityLog();
        }

        private void BtnCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is not CyberTask selectedTask)
            {
                MessageBox.Show(
                    "Please select a task first.",
                    "No Task Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (_taskRepository.MarkTaskCompleted(selectedTask.Id))
            {
                _activityLog.Add($"Task marked as completed: '{selectedTask.Title}'");
                RefreshTaskList();
                RefreshActivityLog();
            }
            else
            {
                MessageBox.Show(
                    "Task could not be updated.\n\n" + _taskRepository.LastError,
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                _activityLog.Add("Task update failed: " + _taskRepository.LastError);
                RefreshActivityLog();
            }
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskList.SelectedItem is not CyberTask selectedTask)
            {
                MessageBox.Show(
                    "Please select a task first.",
                    "No Task Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete '{selectedTask.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            if (_taskRepository.DeleteTask(selectedTask.Id))
            {
                _activityLog.Add($"Task deleted: '{selectedTask.Title}'");
                RefreshTaskList();
                RefreshActivityLog();
            }
            else
            {
                MessageBox.Show(
                    "Task could not be deleted.\n\n" + _taskRepository.LastError,
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                _activityLog.Add("Task delete failed: " + _taskRepository.LastError);
                RefreshActivityLog();
            }
        }

        private void RefreshTaskList()
        {
            TaskList.ItemsSource = null;
            TaskList.ItemsSource = _taskRepository.GetAllTasks();
        }

        private void BtnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _quizManager.Start();
            _activityLog.Add("Cybersecurity quiz started");

            BtnNextQuestion.IsEnabled = true;
            ShowCurrentQuizQuestion();
            RefreshActivityLog();
        }

        private void BtnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (!_answerSubmitted)
            {
                RadioButton? selectedOption = QuizOptionsPanel.Children
                    .OfType<RadioButton>()
                    .FirstOrDefault(option => option.IsChecked == true);

                if (selectedOption == null)
                {
                    MessageBox.Show(
                        "Please select an answer first.",
                        "No Answer Selected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                int selectedIndex = Convert.ToInt32(selectedOption.Tag);

                string feedback = _quizManager.SubmitAnswer(selectedIndex);

                QuizFeedbackText.Text = feedback;
                QuizScoreText.Text = $"Score: {_quizManager.Score}/{_quizManager.TotalQuestions}";

                _activityLog.Add($"Quiz question {_quizManager.CurrentQuestionNumber} answered");

                _answerSubmitted = true;
                BtnNextQuestion.Content = _quizManager.IsLastQuestion ? "Finish Quiz" : "Next Question";

                RefreshActivityLog();
                return;
            }

            if (_quizManager.MoveNext())
            {
                ShowCurrentQuizQuestion();
            }
            else
            {
                ShowFinalQuizScore();
            }
        }

        private void ShowCurrentQuizQuestion()
        {
            QuizQuestion question = _quizManager.GetCurrentQuestion();

            QuizQuestionText.Text =
                $"Question {_quizManager.CurrentQuestionNumber} of {_quizManager.TotalQuestions}\n\n{question.Question}";

            QuizOptionsPanel.Children.Clear();

            for (int i = 0; i < question.Options.Count; i++)
            {
                RadioButton option = new RadioButton
                {
                    Content = question.Options[i],
                    Tag = i,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                    FontSize = 13,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                QuizOptionsPanel.Children.Add(option);
            }

            QuizFeedbackText.Text = "";
            QuizScoreText.Text = $"Score: {_quizManager.Score}/{_quizManager.TotalQuestions}";

            _answerSubmitted = false;
            BtnNextQuestion.Content = "Submit Answer";
        }

        private void ShowFinalQuizScore()
        {
            string finalFeedback = _quizManager.GetFinalFeedback();

            QuizQuestionText.Text = finalFeedback;
            QuizOptionsPanel.Children.Clear();
            QuizFeedbackText.Text = "Quiz completed.";
            QuizScoreText.Text = $"Final Score: {_quizManager.Score}/{_quizManager.TotalQuestions}";
            BtnNextQuestion.IsEnabled = false;
            BtnNextQuestion.Content = "Submit Answer";

            _activityLog.Add($"Quiz completed with score {_quizManager.Score}/{_quizManager.TotalQuestions}");
            RefreshActivityLog();
        }

        private void BtnShowLog_Click(object sender, RoutedEventArgs e)
        {
            _activityLog.Add("Activity log viewed from GUI");
            RefreshActivityLog();
        }

        private void RefreshActivityLog()
        {
            ActivityLogList.ItemsSource = null;
            ActivityLogList.ItemsSource = _activityLog.GetRecent(10);
        }

        private void ReminderTimer_Tick(object? sender, EventArgs e)
        {
            CheckReminders();
        }

        private void CheckReminders()
        {
            if (!_taskRepository.IsDatabaseReady)
            {
                return;
            }

            var dueReminders = _taskRepository.GetDueRemindersAndMarkShown();

            foreach (var task in dueReminders)
            {
                _activityLog.Add($"Reminder triggered: '{task.Title}'");

                MessageBox.Show(
                    $"Reminder: {task.Title}\n\n{task.Description}",
                    "Cybersecurity Reminder",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }

            if (dueReminders.Count > 0)
            {
                RefreshTaskList();
                RefreshActivityLog();
            }
        }
    }
}