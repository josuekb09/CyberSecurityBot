using System;

namespace CyberSecurityBot
{
    public class BotLogic
    {
        private BotUI _ui;

        // We pass the BotUI class into the logic class so we can use your cool typing effect
        public BotLogic(BotUI ui)
        {
            _ui = ui;
        }

        public void StartConversation()
        {
            // Initial Greeting
            _ui.PrintWithTypingEffect("Hello! I am your Cybersecurity Assistant.", ConsoleColor.Green);
            _ui.PrintWithTypingEffect("What is your name?", ConsoleColor.Green);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("> ");
            string userName = Console.ReadLine();

            _ui.PrintWithTypingEffect($"\nNice to meet you, {userName}! How can I help you stay safe online today?", ConsoleColor.Green);
            _ui.PrintWithTypingEffect("(Type 'exit' to quit)\n", ConsoleColor.DarkGray);

            bool isRunning = true;

            // The main conversation loop
            while (isRunning)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("> ");
                string input = Console.ReadLine()?.ToLower().Trim();

                Console.ForegroundColor = ConsoleColor.Cyan; // Bot's response color

                // Input Validation & Responses
                if (string.IsNullOrEmpty(input))
                {
                    _ui.PrintWithTypingEffect("I didn't quite catch that. Could you say it again?");
                }
                else if (input == "exit" || input == "quit")
                {
                    _ui.PrintWithTypingEffect("Stay safe out there! Goodbye.");
                    isRunning = false;
                }
                else if (input.Contains("how are you"))
                {
                    _ui.PrintWithTypingEffect("I'm functioning at 100% capacity and ready to talk about cybersecurity!");
                }
                else if (input.Contains("purpose") || input.Contains("what can i ask"))
                {
                    _ui.PrintWithTypingEffect("My purpose is to educate you on online safety. You can ask me about 'passwords', 'phishing', or 'safe browsing'.");
                }
                else if (input.Contains("password"))
                {
                    _ui.PrintWithTypingEffect("Password Safety: Always use a strong, unique password for each account. Combine uppercase, lowercase, numbers, and symbols. Consider using a password manager!");
                }
                else if (input.Contains("phishing"))
                {
                    _ui.PrintWithTypingEffect("Phishing: Be cautious of unsolicited emails asking for personal info. Always verify the sender's email address and never click suspicious links.");
                }
                else if (input.Contains("browsing") || input.Contains("safe browsing"))
                {
                    _ui.PrintWithTypingEffect("Safe Browsing: Ensure websites use HTTPS (look for the padlock icon). Avoid using public Wi-Fi for sensitive transactions like banking.");
                }
                else
                {
                    // Default Fallback
                    _ui.PrintWithTypingEffect("I didn't quite understand that. Could you rephrase? (Try asking about passwords, phishing, or safe browsing)");
                }

                Console.WriteLine(); // Add a blank line for readability
            }
        }
    }
}