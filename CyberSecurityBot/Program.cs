using System;

namespace CyberSecurityBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Initialize our custom classes
            BotUI ui = new BotUI();
            BotLogic logic = new BotLogic(ui);

            // 2. Run the startup sequence (Audio then Logo)
            ui.PlayGreetingAudio();
            ui.DisplayLogo();

            // 3. Hand control over to the logic loop
            logic.StartConversation();
        } // Closes Main method
    } // Closes Program class
} // Closes namespace (This is the one you were missing!)