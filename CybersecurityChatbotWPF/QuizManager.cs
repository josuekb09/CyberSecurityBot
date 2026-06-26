using System.Collections.Generic;

namespace CyberSecurityBot
{
    public class QuizManager
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex;

        public int Score { get; private set; }
        public int TotalQuestions => _questions.Count;
        public int CurrentQuestionNumber => _currentIndex + 1;
        public bool IsLastQuestion => _currentIndex >= _questions.Count - 1;

        public QuizManager()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete or report the email", "Forward it to friends", "Ignore it completely" },
                    CorrectIndex = 1,
                    Explanation = "Legitimate companies should never ask for your password through email."
                },
                new QuizQuestion
                {
                    Question = "True or False: Using the same password for every account is safe.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Reusing passwords is risky because one leaked password can expose many accounts."
                },
                new QuizQuestion
                {
                    Question = "Which password is strongest?",
                    Options = new List<string> { "john123", "Password1", "CapeTown2026", "Blue!River$Cloud92" },
                    CorrectIndex = 3,
                    Explanation = "Strong passwords use length, variety, and unpredictability."
                },
                new QuizQuestion
                {
                    Question = "What does 2FA mean?",
                    Options = new List<string> { "Two-Factor Authentication", "Two File Access", "Fast Firewall Access", "Two Form Application" },
                    CorrectIndex = 0,
                    Explanation = "2FA adds an extra security step after your password."
                },
                new QuizQuestion
                {
                    Question = "A website URL starts with HTTPS. What does this usually mean?",
                    Options = new List<string> { "The site is encrypted", "The site is always safe", "The site is free", "The site has no viruses" },
                    CorrectIndex = 0,
                    Explanation = "HTTPS means the connection is encrypted, but you must still check if the website is legitimate."
                },
                new QuizQuestion
                {
                    Question = "True or False: Public Wi-Fi is always safe for online banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Public Wi-Fi can expose sensitive activity, especially without extra protection."
                },
                new QuizQuestion
                {
                    Question = "What is phishing?",
                    Options = new List<string> { "A type of online game", "A scam that tricks people into giving information", "A safe login method", "A password manager" },
                    CorrectIndex = 1,
                    Explanation = "Phishing uses fake messages or websites to steal information."
                },
                new QuizQuestion
                {
                    Question = "What should you check before clicking a link in an email?",
                    Options = new List<string> { "The sender and the real URL", "The colour of the email", "The font size", "The time it arrived" },
                    CorrectIndex = 0,
                    Explanation = "Always check the sender address and the real link destination."
                },
                new QuizQuestion
                {
                    Question = "True or False: You should update your apps and operating system regularly.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "Updates often fix security vulnerabilities."
                },
                new QuizQuestion
                {
                    Question = "Which action helps protect your privacy?",
                    Options = new List<string> { "Sharing your ID number publicly", "Giving every app all permissions", "Reviewing app permissions", "Posting your password online" },
                    CorrectIndex = 2,
                    Explanation = "Reviewing app permissions limits unnecessary access to your data."
                },
                new QuizQuestion
                {
                    Question = "What is social engineering?",
                    Options = new List<string> { "Building social media apps", "Manipulating people to reveal information", "Creating computer networks", "Making websites faster" },
                    CorrectIndex = 1,
                    Explanation = "Social engineering attacks human trust, fear, or urgency."
                },
                new QuizQuestion
                {
                    Question = "True or False: A password manager can help you create and store strong unique passwords.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "Password managers help prevent password reuse and weak passwords."
                }
            };
        }

        public void Start()
        {
            Score = 0;
            _currentIndex = 0;
        }

        public QuizQuestion GetCurrentQuestion()
        {
            return _questions[_currentIndex];
        }

        public string SubmitAnswer(int selectedIndex)
        {
            QuizQuestion question = GetCurrentQuestion();

            if (selectedIndex == question.CorrectIndex)
            {
                Score++;
                return "Correct! " + question.Explanation;
            }

            return "Incorrect. " + question.Explanation;
        }

        public bool MoveNext()
        {
            if (_currentIndex + 1 >= _questions.Count)
            {
                return false;
            }

            _currentIndex++;
            return true;
        }

        public string GetFinalFeedback()
        {
            double percentage = (double)Score / TotalQuestions * 100;

            if (percentage >= 80)
            {
                return $"Great job! You scored {Score}/{TotalQuestions}. You are becoming a cybersecurity pro.";
            }

            if (percentage >= 50)
            {
                return $"Good effort! You scored {Score}/{TotalQuestions}. Keep practising to improve your cyber safety.";
            }

            return $"You scored {Score}/{TotalQuestions}. Keep learning and reviewing the chatbot tips to stay safe online.";
        }
    }
}