using System;
using System.Media;
using System.Threading;

namespace CyberSecurityBot
{
    public class BotUI
    {
        // 1. Method to display the ASCII Art Logo
        public void DisplayLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
      _____       _               ____        _   
     / ___|   _  | |__   ___ _ __| __ )  ___ | |_ 
    | |  | | | | | '_ \ / _ \ '__|  _ \ / _ \| __|
    | |__| |_| | | |_) |  __/ |  | |_) | (_) | |_ 
     \____\__, | |_.__/ \___|_|  |____/ \___/ \__|
          |___/                                   
    ");
            Console.WriteLine("     🛡️  Your Digital Security Assistant  🛡️\n");
            Console.ResetColor();
        }

        // 2. Method to print text with a typing effect
        public void PrintWithTypingEffect(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(30); // 30 milliseconds delay between characters
            }
            Console.WriteLine(); // Move to the next line after finishing
            Console.ResetColor();
        }

        // 3. Method to play the greeting audio
        public void PlayGreetingAudio()
        {
            try
            {
                // It will look for a file named "greeting.wav" in your debug folder
                SoundPlayer player = new SoundPlayer("greeting.wav");
                player.Play();
            }
            catch (Exception)
            {
                // If the file isn't there yet, we just silently catch the error so the app doesn't crash
                Console.WriteLine("[System: Audio file 'greeting.wav' not found. We will add this later!]");
            }
        }
    }
}